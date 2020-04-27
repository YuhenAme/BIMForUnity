Shader "Unlit/NormalRender"
{
    Properties{
		_Color("Color", Color) = (1,1,1,1)
		_CrossColor("Cross Section Color", Color) = (1,1,1,1)
		_Specular("Specular",Color) = (1,1,1,1)
		_MainTex("Albedo (RGB)", 2D) = "white" {}
		_Glossiness("Smoothness", Range(0,1)) = 0.5
		_Metallic("Metallic", Range(0,1)) = 0.0
		
		_StencilMask("Stencil Mask", Range(0, 255)) = 255
		[HideInInspector] _Mode ("__mode", Float) = 0.0
		[HideInInspector] _SrcBlend ("__src", Float) = 1.0
		[HideInInspector] _DstBlend ("__dst", Float) = 0.0
		[HideInInspector] _ZWrite ("__zw", Float) = 1.0
	}
	SubShader {
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
		
		Pass
		{
			Tags { "LightMode"="ForwardBase" }
			ZWrite [_ZWrite]
			Blend [_SrcBlend] [_DstBlend]
			
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
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#include "Lighting.cginc"
			half _Glossiness;
			half _Metallic;
			fixed4 _Color;
			fixed4 _CrossColor;
			fixed4 _Specular;
			
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
