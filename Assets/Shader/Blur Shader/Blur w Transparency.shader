Shader "Custom/Blur png support" {
    Properties{
        _Color("Main Color", Color) = (1, 1, 1, 1)
        _BumpAmt("Distortion", Range(0, 128)) = 10
        _MainTex("Albedo (RGB)", 2D) = "white" {}
        _BumpMap("Normalmap", 2D) = "bump" {}
        _Size("Size", Range(0, 20)) = 1
    }

        SubShader{
            Tags { "Queue" = "Transparent" "RenderType" = "Transparent" }

            Pass {
                CGPROGRAM
                #pragma vertex vert
                #pragma fragment frag
                #include "UnityCG.cginc"

                struct appdata_t {
                    float4 vertex : POSITION;
                    float2 uv : TEXCOORD0;
                };

                struct v2f {
                    float4 vertex : SV_POSITION;
                    float2 uv : TEXCOORD0;
                };

                float4 _Color;
                float _BumpAmt;
                float4 _BumpMap_ST;
                float4 _MainTex_ST;
                float _Size;

                sampler2D _MainTex;
                sampler2D _BumpMap;

                v2f vert(appdata_t v) {
                    v2f o;
                    o.vertex = UnityObjectToClipPos(v.vertex);
                    o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                    return o;
                }

                half4 frag(v2f i) : SV_Target {
                    half4 col = tex2D(_MainTex, i.uv);
                    half2 bump = UnpackNormal(tex2D(_BumpMap, i.uv)).rg;
                    float2 offset = bump * _BumpAmt * _MainTex_ST.xy * _Size;
                    i.uv += offset;
                    half4 finalColor = col * _Color;
                    return finalColor;
                }
                ENDCG
            }
        }
}
