Shader "Hidden/SRP_PartialBlur"
{
    Properties
    {
        _Factor ("Factor X", Range(0, 5)) = 1.0
        _Factor_Y ("Factor Y", Range(0, 5)) = 1.0
        _Range ("Range X", Range(0,10)) = 0
        _Range_Y ("Range Y", Range(0,10)) = 0
        _Darkness ("Darkness", Range(0,1)) = 0.6
        _MainTex ("Main Texture", 2D) = "white" {}
        _Color ("Color", Color) = (0, 0, 0, 0)
    }
    SubShader
    {
        Tags { "Queue"="Transparent" "IgnoreProjector"="True" "RenderType"="Transparent" }

        Pass
        {
            ZTest Always Cull Off ZWrite Off
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma fragmentoption ARB_precision_hint_fastest

            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float4 pos : SV_POSITION;
                float4 uv : TEXCOORD0;
            };

            v2f vert (appdata v)
            {
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);
                o.uv = ComputeGrabScreenPos(o.pos);
                return o;
            }

            sampler2D _MainTex;
            float4 _MainTex_TexelSize;
            float _Factor;
            float _Range;

            half4 frag (v2f i) : SV_Target
            {

                half4 pixelCol = half4(0, 0, 0, 0);
				float dist = clamp(1 - _Range * distance(float2(0.5, 0.5), i.uv), 0, 1);
                #define ADDPIXEL(weight,kernelX) tex2Dproj(_MainTex, UNITY_PROJ_COORD(float4(i.uv.x + _MainTex_TexelSize.x * kernelX * _Factor * dist, i.uv.y, i.uv.z, i.uv.w))) * weight
                
                pixelCol += ADDPIXEL(0.05, 4.0);
                pixelCol += ADDPIXEL(0.09, 3.0);
                pixelCol += ADDPIXEL(0.12, 2.0);
                pixelCol += ADDPIXEL(0.15, 1.0);
                pixelCol += ADDPIXEL(0.18, 0.0);
                pixelCol += ADDPIXEL(0.15, -1.0);
                pixelCol += ADDPIXEL(0.12, -2.0);
                pixelCol += ADDPIXEL(0.09, -3.0);
                pixelCol += ADDPIXEL(0.05, -4.0);
                return pixelCol;
            }
            ENDCG
        }

        Pass
        {
            ZTest Always Cull Off ZWrite Off
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma fragmentoption ARB_precision_hint_fastest

            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float4 pos : SV_POSITION;
                float4 uv : TEXCOORD0;
            };

            v2f vert (appdata v)
            {
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);
                o.uv = ComputeGrabScreenPos(o.pos);
                return o;
            }

            sampler2D _MainTex;
            float4 _MainTex_TexelSize;
            float _Factor_Y;
            float _Range_Y;
            float _Darkness;

            fixed4 frag (v2f i) : SV_Target
            {

                fixed4 pixelCol = fixed4(0, 0, 0, 0);
				float dist = clamp(1 - _Range_Y * distance(float2(0.5, 0.5), i.uv), 0, 1);

                #define ADDPIXEL_Y(weight,kernelY) tex2Dproj(_MainTex, UNITY_PROJ_COORD(float4(i.uv.x, i.uv.y + _MainTex_TexelSize.y * kernelY * _Factor_Y * dist, i.uv.z, i.uv.w))) * weight
                
                pixelCol += ADDPIXEL_Y(0.05, 4.0);
                pixelCol += ADDPIXEL_Y(0.09, 3.0);
                pixelCol += ADDPIXEL_Y(0.12, 2.0);
                pixelCol += ADDPIXEL_Y(0.15, 1.0);
                pixelCol += ADDPIXEL_Y(0.18, 0.0);
                pixelCol += ADDPIXEL_Y(0.15, -1.0);
                pixelCol += ADDPIXEL_Y(0.12, -2.0);
                pixelCol += ADDPIXEL_Y(0.09, -3.0);
                pixelCol += ADDPIXEL_Y(0.05, -4.0);
                pixelCol.x *= 1 - _Darkness;
                pixelCol.y *= 1 - _Darkness;
                pixelCol.z *= 1 - _Darkness;
                return pixelCol;
            }
            ENDCG
        }
    }
}