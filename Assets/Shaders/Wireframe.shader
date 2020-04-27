Shader "Unlit/Wireframe"
{
    Properties
    {
       _LineColor("Line Color",Color) = (1,1,1,1)
	   _BackColor("Back Color",Color) = (0,0,0,0)
	   _WireThickness ("Wire Thickness", RANGE(0, 800)) = 100
	   [Toggle(ENABLE_DRAWQUAD)]_DrawQuad("Draw Quad", Float) = 0
	   [Enum(UnityEngine.Rendering.CullMode)] _Cull ("Cull Mode", Float) = 2
    }
    SubShader
    {
		// http://developer.download.nvidia.com/SDK/10/direct3d/Source/SolidWireframe/Doc/SolidWireframe.pdf
		Blend SrcAlpha OneMinusSrcAlpha
		Cull [_Cull]
        Tags { "RenderType"="Opaque" }
        LOD 100

        Pass
        {
            CGPROGRAM
			#pragma target 4.0
			#pragma multi_compile __ ENABLE_DRAWQUAD
			#pragma vertex vert
			#pragma geometry geom
			#pragma fragment frag

			#include "UnityCG.cginc"
			float _WireThickness;
			half4 _LineColor;

			struct appdata
			{
				float4 vertex : POSITION;
				//在顶点着色器中使用此输入/输出结构以定义实例ID
				UNITY_VERTEX_INPUT_INSTANCE_ID
			};
			struct v2g
			{
				float4 projectionSpaceVertex : SV_POSITION;
				float4 worldSpacePosition : TEXCOORD1;
				float4 vertexPos : TEXCOORD2;
				//声明该顶点是否位于视线域中
				UNITY_VERTEX_OUTPUT_STEREO
			};
			struct g2f
			{
				float4 projectionSpaceVertex : SV_POSITION;
				float4 worldSpacePosition : TEXCOORD0;
				float4 dist : TEXCOORD1;
				int max : TEXCOORD2;
				//
				UNITY_VERTEX_OUTPUT_STEREO
			};
			v2g vert (appdata v)
			{
				v2g o;
				//使得shader可以访问实例ID
				UNITY_SETUP_INSTANCE_ID(v);
				UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);
				//裁剪空间的顶点
				o.projectionSpaceVertex = UnityObjectToClipPos(v.vertex);
				//世界空间的顶点位置
				o.worldSpacePosition = mul(unity_ObjectToWorld, v.vertex);
				o.vertexPos = v.vertex;
				return o;
			}
			//[maxvertexcount(N)]指定单词调用几何着色器输出的顶点的最大数量
			//几何着色器通过将顶点附加到输出流对象来输出一个顶点，TriangleStream,LineStream,PointStream
			[maxvertexcount(3)]
			void geom(triangle v2g i[3], inout TriangleStream<g2f> triangleStream)
			{
			    //转换到齐次坐标空间
				float2 _p0 = i[0].projectionSpaceVertex.xy / i[0].projectionSpaceVertex.w;
				float2 _p1 = i[1].projectionSpaceVertex.xy / i[1].projectionSpaceVertex.w;
				float2 _p2 = i[2].projectionSpaceVertex.xy / i[2].projectionSpaceVertex.w;

				//每个点的位置
				float3 p0 = i[0].vertexPos;
				float3 p1 = i[1].vertexPos;
				float3 p2 = i[2].vertexPos;

				//三条向量
				float2 edge0 = _p2 - _p1;
				float2 edge1 = _p2 - _p0;
				float2 edge2 = _p1 - _p0;

				//每条边的长度
				float s0 = length(p2 - p1);
				float s1 = length(p2 - p0);
				float s2 = length(p1 - p0);

				
				//为了找到一个点到相反边的距离，由面积 = 底/2 * 高
				//得到高 = （面积 * 2）/底
				//两边的叉乘 = 面积的两倍
				float area = abs(edge1.x * edge2.y - edge1.y * edge2.x);
				float wireThickness = 800 - _WireThickness;
				int max = 0;

				//画矩形线框时，直接把三角形最长边，排除在外即可
				#if ENABLE_DRAWQUAD
				if(s1 > s0)
				{
					if(s1 > s2)
						max = 1;
					else
						max = 2;
				}
				else if(s2 > s0)
				{
					max = 2;
				}				
				#endif

				g2f o;

				o.worldSpacePosition = i[0].worldSpacePosition;
				o.projectionSpaceVertex = i[0].projectionSpaceVertex;
				//计算顶点到对应边的距离
				o.dist.xyz = float3( (area / length(edge0)), 0.0, 0.0) * wireThickness * o.projectionSpaceVertex.w;
				o.dist.w = 1.0 / o.projectionSpaceVertex.w;
				o.max = max;
				UNITY_TRANSFER_VERTEX_OUTPUT_STEREO(i[0], o);
				//将一个顶点添加到输出流列表
				triangleStream.Append(o);

				o.worldSpacePosition = i[1].worldSpacePosition;
				o.projectionSpaceVertex = i[1].projectionSpaceVertex;
				o.dist.xyz = float3(0.0, (area / length(edge1)), 0.0) * wireThickness * o.projectionSpaceVertex.w;
				o.dist.w = 1.0 / o.projectionSpaceVertex.w;
				o.max = max;
				UNITY_TRANSFER_VERTEX_OUTPUT_STEREO(i[1], o);
				//将一个顶点添加到输出流列表
				triangleStream.Append(o);

				o.worldSpacePosition = i[2].worldSpacePosition;
				o.projectionSpaceVertex = i[2].projectionSpaceVertex;
				o.dist.xyz = float3(0.0, 0.0, (area / length(edge2))) * wireThickness * o.projectionSpaceVertex.w;
				o.dist.w = 1.0 / o.projectionSpaceVertex.w;
				o.max = max;
				UNITY_TRANSFER_VERTEX_OUTPUT_STEREO(i[2], o);
				//将一个顶点添加到输出流列表
				triangleStream.Append(o);
			}
			fixed4 frag (g2f i) : SV_Target
			{				
				float minDistanceToEdge;// = min(i.dist[0], min(i.dist[1], i.dist[2])) * i.dist[3];
				#if ENABLE_DRAWQUAD
				if(i.max == 0)
					minDistanceToEdge = min(i.dist[1], i.dist[2]);
				else if(i.max == 1)
					minDistanceToEdge = min(i.dist[0], i.dist[2]);
				else 
					minDistanceToEdge = min(i.dist[0], i.dist[1]);
				#else
				minDistanceToEdge = min(i.dist[0], min(i.dist[1], i.dist[2])) * i.dist[3];
				#endif
				// 如果不是接近线段边缘，提前退出
				if(minDistanceToEdge > 0.9)
				{
					return fixed4(0,0,0,0);
				}

				//对线段进行柔和处理
				float t = exp2(-2 * minDistanceToEdge * minDistanceToEdge);
				fixed4 wireColor = _LineColor;

				fixed4 finalColor = lerp(float4(0,0,0,0), wireColor, t);
				finalColor.a = t;

				return finalColor;
			}
			ENDCG
        }
    }
}
