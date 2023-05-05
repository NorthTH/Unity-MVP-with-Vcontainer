Shader "Custom/Blur"
{
    Properties {
        _BlurSize ("BlurSize", Range(0.0, 20.0)) = 3.0
    }
    SubShader {
        Tags { "RenderType"="Transparent" }
        LOD 100

        GrabPass { }

        Pass {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma fragmentoption ARB_precision_hint_fastest

            #include "UnityCG.cginc"

            struct appdata {
                half4 pos : POSITION;
                half2 uv : TEXCOORD0;
                float4 color : COLOR;
            };

            struct v2f
            {
                float4 pos  : SV_POSITION;
                half2  uv  : TEXCOORD0;
                float4 color : COLOR;
            };

            float4 _MainTex_TexelSize;
            sampler2D _GrabTexture;
            half _BlurSize;

            v2f vert(appdata v)
            {
                v2f o;
                o.pos = UnityObjectToClipPos(v.pos);
                o.uv = ComputeGrabScreenPos(o.pos);
                o.color = v.color;
                return o;
            }

            fixed4 frag(v2f i) : COLOR
            {
                float2 scale = _BlurSize / 1000;
                scale.xy *= _MainTex_TexelSize.x / _MainTex_TexelSize.y;

                fixed4 color = tex2D(_GrabTexture, i.uv) * 0.2;
                float2 size = _BlurSize / 1000;
                size.xy *= _MainTex_TexelSize.x / _MainTex_TexelSize.y;
                color += tex2D(_GrabTexture, i.uv + float2(_BlurSize, _BlurSize) * size) * 0.03;
                color += tex2D(_GrabTexture, i.uv + float2(_BlurSize, -_BlurSize) * size) * 0.03;
                color += tex2D(_GrabTexture, i.uv + float2(-_BlurSize, _BlurSize) * size) * 0.03;
                color += tex2D(_GrabTexture, i.uv + float2(-_BlurSize, -_BlurSize) * size) * 0.03;
                color += tex2D(_GrabTexture, i.uv + float2(_BlurSize, _BlurSize) * size * 0.7) * 0.07;
                color += tex2D(_GrabTexture, i.uv + float2(_BlurSize, -_BlurSize) * size * 0.7) * 0.07;
                color += tex2D(_GrabTexture, i.uv + float2(-_BlurSize, _BlurSize) * size * 0.7) * 0.07;
                color += tex2D(_GrabTexture, i.uv + float2(-_BlurSize, -_BlurSize) * size * 0.7) * 0.07;
                color += tex2D(_GrabTexture, i.uv + float2(_BlurSize, _BlurSize) * size * 0.4) * 0.1;
                color += tex2D(_GrabTexture, i.uv + float2(_BlurSize, -_BlurSize) * size * 0.4) * 0.1;
                color += tex2D(_GrabTexture, i.uv + float2(-_BlurSize, _BlurSize) * size * 0.4) * 0.1;
                color += tex2D(_GrabTexture, i.uv + float2(-_BlurSize, -_BlurSize) * size * 0.4) * 0.1;

                color *= i.color;
                return color;
            }
            ENDCG
        }
    }
}
