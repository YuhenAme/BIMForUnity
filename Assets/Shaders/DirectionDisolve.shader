Shader "Custom/DirectionDisolve"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
		_Color ("Color", Color) = (1,1,1,1)
		_ThresholdX("ThresholdX", Range(0.0, 1.3)) = 0
		_ThresholdY("ThresholdY", Range(0.0, 1.3)) = 0
		_ThresholdZ("ThresholdZ", Range(0.0, 1.3)) = 0
		_DirectionX("DirectionX", Int) = 1 //1表示从X正方向开始，其他值则从负方向
		_DirectionY("DirectionY", Int) = 1
		_DirectionZ("DirectionZ", Int) = 1
		_MinBorderX("Min Border X", Float) = -0.5
		_MaxBorderX("Max Border X", Float) = 0.5 
		_MinBorderY("Min Border Y", Float) = -0.5
		_MaxBorderY("Max Border Y", Float) = 0.5
		_MinBorderZ("Min Border Z", Float) = -0.5
		_MaxBorderZ("Max Border Z", Float) = 0.5
	}
    SubShader
    {
        Tags { "Queue"="Geometry""RenderType"="Opaque" }
        LOD 100

        Pass
        {
			Cull Off
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            

            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float4 vertex:SV_POSITION;
				float2 uvMainTex:TEXCOORD0;
				float3 objPos:TEXCOORD1;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;
			float _ThresholdX;
			float _ThresholdY;
			float _ThresholdZ;
			int _DirectionX;
			int _DirectionY;
			int _DirectionZ;
			float _MinBorderX;
			float _MaxBorderX;
			float _MinBorderY;
			float _MaxBorderY;
			float _MinBorderZ;
			float _MaxBorderZ;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uvMainTex = TRANSFORM_TEX(v.uv, _MainTex);
                o.objPos = v.vertex.xyz;
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                // sample the texture
                fixed4 col = tex2D(_MainTex, i.uvMainTex);
				
				float rangeX = _MaxBorderX - _MinBorderX;
				float borderX = _MinBorderX;
				if(_DirectionX == 1) 
					borderX = _MaxBorderX;

				float rangeY = _MaxBorderY - _MinBorderY;
				float borderY = _MinBorderY;
				if(_DirectionY == 1) 
					borderY = _MaxBorderY;

				float rangeZ = _MaxBorderZ - _MinBorderZ;
				float borderZ = _MinBorderZ;
				if(_DirectionZ == 1) 
					borderZ = _MaxBorderZ;

				float distX = abs(i.objPos.x-borderX);
				float distY = abs(i.objPos.y-borderY);
				float distZ = abs(i.objPos.z-borderZ);
				float normalizeDistX = saturate(distX/rangeX);
				float normalizeDistY = saturate(distY/rangeY);
				float normalizeDistZ = saturate(distZ/rangeZ);

				clip(normalizeDistX - _ThresholdX);
				clip(normalizeDistY - _ThresholdY);
				clip(normalizeDistZ - _ThresholdZ);
				
                return col;
            }
            ENDCG
        }
    }
	 FallBack "Diffuse"
}
