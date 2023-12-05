Shader"Blur/FirstDimensionLinearSamplingGaussianBlur"
{
	CGINCLUDE
	#include "UnityCG.cginc"
	#pragma multi_compile LITTLE_KERNEL MEDIUM_KERNEL BIG_KERNEL	
	#include "Blur.cginc"

	uniform sampler2D _MainTex;
	uniform float4 _MainTex_TexelSize;
	uniform float _Sigma;
    uniform float _BlurAmount;
    uniform float _KernelStep;

	half4 frag_horizontal(v2f_img i) : SV_Target
	{
		pixel_info pinfo;
		pinfo.tex = _MainTex;
		pinfo.uv = i.uv;
		pinfo.texelSize = _MainTex_TexelSize;
		return GaussianBlurLinearSampling(pinfo, _Sigma, half2(_BlurAmount,0), _KernelStep);
	}

    half4 frag_vertical(v2f_img i) : SV_Target
	{
		pixel_info pinfo;
		pinfo.tex = _MainTex;
		pinfo.uv = i.uv;
		pinfo.texelSize = _MainTex_TexelSize;
		return GaussianBlurLinearSampling(pinfo, _Sigma, half2(0,_BlurAmount), _KernelStep);
	}
    ENDCG

    Properties
	{
		_MainTex("Base (RGB)", 2D) = "white" {}
	}

	SubShader
	{
		Tags { "Queue"="Transparent" "IgnoreProjector"="True" "RenderType"="Transparent" }

		Lighting Off
		Cull Off
		ZWrite Off
		ZTest Always

		Pass
		{
			CGPROGRAM
			#pragma target 3.0
			#pragma vertex vert_img
			#pragma fragment frag_horizontal
			ENDCG
		}

		Pass
		{
			CGPROGRAM
			#pragma target 3.0
			#pragma vertex vert_img
			#pragma fragment frag_vertical
			ENDCG
		}
	}
}