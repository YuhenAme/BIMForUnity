Shader "Custom/TextShader"
{
    Properties
    {
        _Color ("Color", Color) = (1,1,1,1)
        _MainTex ("Albedo (RGB)", 2D) = "white" {}
        //_Glossiness ("Smoothness", Range(0,1)) = 0.5
        //_Metallic ("Metallic", Range(0,1)) = 0.0
		//_StartPos("Start Pos",Vector) = (0,0,0,0)
		_DisolveLeftToRight("Left to Right",Range(-2,0)) = -2
		_DisolveRightToLeft("Right to Left",Range(0,2)) = 2
		_DisolveDownToTop("Down to Top",Range(-2,0))= -2
		_DisolveTopToDown("Top to Down",Range(0,2))=2
		_DisolveFrontToBehind("Front to Behind",Range(-2,0))=-2
		_DisolveBehindToFront("Behind to Front",Range(0,2))=2
    }
    SubShader
    {
        Tags { "Queue"="Geometry""RenderType"="Opaque" }
        LOD 200
		Pass
		{
			Cull Off
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#include"Lighting.cginc"
			#include"UnityCG.cginc"
			sampler2D _MainTex;
			float4 _MainTex_ST;
			float4 _StartPos;
			float _DisolveLeftToRight;
			float _DisolveRightToLeft;
			float _DisolveTopToDown;
			float _DisolveDownToTop;
			float _DisolveFrontToBehind;
			float _DisolveBehindToFront;


			struct appdata
			{
				float4 vertex:POSITION;
				float2 uv:TEXCOORD0;
			};
			struct v2f
			{
				float4 vertex:SV_POSITION;
				float2 uvMainTex:TEXCOORD0;
				float3 objPos:TEXCOORD1;
			};

			v2f vert(appdata v)
			{
				v2f o;
				o.vertex=UnityObjectToClipPos(v.vertex);
				o.uvMainTex=TRANSFORM_TEX(v.uv,_MainTex);
				o.objPos = v.vertex.xyz;
				return o;
			}
			fixed4 frag(v2f i):SV_Target
			{
				float disX1 = length(i.objPos.x - _DisolveLeftToRight);
				float disX2 = length(i.objPos.x - _DisolveRightToLeft);
				float disY1 = length(i.objPos.y - _DisolveTopToDown);
				float disY2 = length(i.objPos.y - _DisolveDownToTop);
				float disZ1 = length(i.objPos.z - _DisolveFrontToBehind);
				float disZ2 = length(i.objPos.z - _DisolveBehindToFront);
				fixed3 albedo = tex2D(_MainTex,i.uvMainTex).rgb;
				clip(disX1-albedo.r);
				clip(disX2-albedo.r);
				clip(disY1-albedo.r);
				clip(disY2-albedo.r);
				clip(disZ1-albedo.r);
				clip(disZ2-albedo.r);
				//fixed3 albedo = tex2D(_MainTex,i.uvMainTex).rgb;
				return fixed4(albedo,1);
			}
			ENDCG

		}
        
    }
    FallBack "Diffuse"
}
