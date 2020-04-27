Shader "Wireframe/Cull"
{
    Properties
    {
        _LineColor ("Line color", Color) = (0, 0, 0, 1)
	    _LineSize ("Line size", float) = 0.3
		[Toggle(ENABLE_DRAWQUAD)]_DrawQuad("Draw Quad",Float) = 0.0
    }
    SubShader
    {
        Tags { "RenderType"="Transparent" "Queue"="Transparent" }
        LOD 100

        Pass
        {
            Blend SrcAlpha OneMinusSrcAlpha
			ZWrite Off
			CGPROGRAM
			#include "UnityCG.cginc"
			#include "WireframeBarycentricCoordinatesCore.cginc"
			#pragma multi_compile __ ENABLE_DRAWQUAD
			#pragma vertex vert
			#pragma fragment frag
			ENDCG
        }
    }
}
