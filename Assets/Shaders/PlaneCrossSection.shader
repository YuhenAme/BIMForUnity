Shader "CrossSection/OnePlaneBSP" {
	Properties{
	   //[Enum(UnityEngine.Rendering.RenderQueue)]_RenderingMode("Rendering Mode",Float) =5
		_Color("Color", Color) = (1,1,1,1)
		_CrossColor("Cross Section Color", Color) = (1,1,1,1)
		_Specular("Specular",Color) = (1,1,1,1)
		_MainTex("Albedo (RGB)", 2D) = "white" {}
		_Glossiness("Smoothness", Range(0,1)) = 0.5
		_Metallic("Metallic", Range(0,1)) = 0.0
		[HideInInspector]_PlaneNormalX("PlaneNormalX",Vector) = (0,1,0,0)
		[HideInInspector]_PlanePositionX("PlanePositionX",Vector) = (0,0,0,1)
		[HideInInspector]_PlaneNormalY("PlaneNormalY",Vector) = (0,1,0,0)
		[HideInInspector]_PlanePositionY("PlanePositionY",Vector) = (0,0,0,1)
		[HideInInspector]_PlaneNormalZ("PlaneNormalZ",Vector) = (0,1,0,0)
		[HideInInspector]_PlanePositionZ("PlanePositionZ",Vector) = (0,0,0,1)
		_StencilMask("Stencil Mask", Range(0, 255)) = 255
		

		[HideInInspector] _Mode ("__mode", Float) = 0.0
		[HideInInspector] _SrcBlend ("__src", Float) = 1.0
		[HideInInspector] _DstBlend ("__dst", Float) = 0.0
		[HideInInspector] _ZWrite ("__zw", Float) = 1.0
	}
	SubShader {	
		//Tags { "RenderType"="Transparent" "Queue"="Transparent" }	
		Tags { "RenderType"="Opaque" }
		

		LOD 200
		Stencil
		{
			Ref [_StencilMask]
			CompBack Always
			PassBack Replace

			CompFront Always
			PassFront Zero
		}
		//-----------------------
		//开启深度的Pass
		//Pass
		//{
		//	ZWrite on
		//	ColorMask 0
		//}
		Pass
		{
			Tags { "LightMode"="ForwardBase" }
			
			ZWrite [_ZWrite]
			Blend [_SrcBlend] [_DstBlend]
			

			//ZWrite Off
			//Blend SrcAlpha OneMinusSrcAlpha
			

			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#include "Lighting.cginc"
			#include "AutoLight.cginc"
			half _Glossiness;
			half _Metallic;
			fixed4 _Color;
			fixed4 _CrossColor;
			fixed4 _Specular;
			fixed3 _PlaneNormalX;
			fixed3 _PlanePositionX;
			fixed3 _PlaneNormalY;
			fixed3 _PlanePositionY;
			fixed3 _PlaneNormalZ;
			fixed3 _PlanePositionZ;
			sampler2D _MainTex;
			float4 _MainTex_ST;
			struct a2v
			{
				float4 vertex :POSITION;
				float4 texcoord : TEXCOORD0;
				float3 normal : NORMAL;
				//float3 worldPos : TEXCOORD1;
				
			};
			struct v2f
			{
				float4 pos :SV_POSITION;
				float3 worldNormal : TEXCOORD0;
				float3 worldPos : TEXCOORD1;
				float2 uv : TEXCOORD2;
			};
			bool checkVisabilityX(fixed3 worldPos)
			{
				float dotProd1 = dot(worldPos - _PlanePositionX, _PlaneNormalX);
				return dotProd1 > 0  ;
			}
			bool checkVisabilityY(fixed3 worldPos)
			{
				float dotProd1 = dot(worldPos - _PlanePositionY, _PlaneNormalY);
				return dotProd1 > 0  ;
			}
			bool checkVisabilityZ(fixed3 worldPos)
			{
				float dotProd1 = dot(worldPos - _PlanePositionZ, _PlaneNormalZ);
				return dotProd1 > 0  ;
			}

			v2f vert(a2v v)
			{
				v2f o;
				o.pos = UnityObjectToClipPos(v.vertex);
				o.worldNormal = UnityObjectToWorldNormal(v.normal);
				o.worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;
				o.uv = TRANSFORM_TEX(v.texcoord, _MainTex);
				return o;
			}
			fixed4 frag(v2f i):SV_Target
			{
				if (checkVisabilityX(i.worldPos))discard;
				if (checkVisabilityY(i.worldPos))discard;
				if (checkVisabilityZ(i.worldPos))discard;

				fixed3 worldNormal = normalize(i.worldNormal);
				fixed3 worldLightDir = normalize(UnityWorldSpaceLightDir(i.worldPos));
				fixed3 worldViewDir = normalize(UnityWorldSpaceViewDir(i.worldPos));
				fixed4 texColor = tex2D(_MainTex, i.uv);

				
				fixed3 albedo = texColor.rgb * _Color.rgb;
				fixed3 ambient = UNITY_LIGHTMODEL_AMBIENT.xyz * albedo;
				fixed3 diffuse = _LightColor0.rgb * albedo * max(0, dot(worldNormal, worldLightDir));
				return fixed4(ambient+diffuse,_Color.a);
			}
			ENDCG
		}
		
		Pass
		{
			Tags { "LightMode"="ForwardAdd" }
			
			ZWrite Off
			Blend [_SrcBlend] One

			//ZWrite Off
			//Blend SrcAlpha OneMinusSrcAlpha
			
			
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#include "Lighting.cginc"
			half _Glossiness;
			half _Metallic;
			fixed4 _Color;
			fixed4 _CrossColor;
			fixed4 _Specular;
			fixed3 _PlaneNormalX;
			fixed3 _PlanePositionX;
			fixed3 _PlaneNormalY;
			fixed3 _PlanePositionY;
			fixed3 _PlaneNormalZ;
			fixed3 _PlanePositionZ;
			sampler2D _MainTex;
			float4 _MainTex_ST;
			struct a2v
			{
				float4 vertex :POSITION;
				float4 texcoord : TEXCOORD0;
				float3 normal : NORMAL;
				//float3 worldPos : TEXCOORD1;
			};
			struct v2f
			{
				float4 pos :SV_POSITION;
				float3 worldNormal : TEXCOORD0;
				float3 worldPos : TEXCOORD1;
				float2 uv : TEXCOORD2;
			};
			bool checkVisabilityX(fixed3 worldPos)
			{
				float dotProd1 = dot(worldPos - _PlanePositionX, _PlaneNormalX);
				return dotProd1 > 0  ;
			}
			bool checkVisabilityY(fixed3 worldPos)
			{
				float dotProd1 = dot(worldPos - _PlanePositionY, _PlaneNormalY);
				return dotProd1 > 0  ;
			}
			bool checkVisabilityZ(fixed3 worldPos)
			{
				float dotProd1 = dot(worldPos - _PlanePositionZ, _PlaneNormalZ);
				return dotProd1 > 0  ;
			}

			v2f vert(a2v v)
			{
				v2f o;
				o.pos = UnityObjectToClipPos(v.vertex);
				o.worldNormal = UnityObjectToWorldNormal(v.normal);
				o.worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;
				o.uv = TRANSFORM_TEX(v.texcoord, _MainTex);
				return o;
			}
			fixed4 frag(v2f i):SV_Target
			{
				if (checkVisabilityX(i.worldPos))discard;
				if (checkVisabilityY(i.worldPos))discard;
				if (checkVisabilityZ(i.worldPos))discard;

				fixed3 worldNormal = normalize(i.worldNormal);
				fixed3 worldLightDir = normalize(UnityWorldSpaceLightDir(i.worldPos));
				fixed3 worldViewDir = normalize(UnityWorldSpaceViewDir(i.worldPos));
				fixed4 texColor = tex2D(_MainTex, i.uv);

				//fixed3 ambient = UNITY_LIGHTMODEL_AMBIENT.xyz;
				//fixed3 halfDir = normalize(worldLightDir+worldViewDir);
				//fixed3 specular = _LightColor0.rgb*_Specular.rgb*pow(max(0,dot(worldNormal,halfDir)),_Glossiness);
				//fixed3 diffuse = _LightColor0.rgb*texColor.rgb*_Color.rgb*max(0,dot(worldNormal,worldLightDir));

				
				fixed3 albedo = texColor.rgb * _Color.rgb;
				fixed3 ambient = UNITY_LIGHTMODEL_AMBIENT.xyz * albedo;
				fixed3 diffuse = _LightColor0.rgb * albedo * max(0, dot(worldNormal, worldLightDir));
				return fixed4(ambient+diffuse,_Color.a);
			}
			ENDCG
		}
	
		
		
		
	}
	//FallBack "Diffuse"
}
