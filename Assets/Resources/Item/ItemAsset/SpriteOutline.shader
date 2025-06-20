Shader "Custom/SpriteOutline" {
    Properties {
        _MainTex ("Sprite", 2D) = "white" {}
        _OutlineColor ("Outline Color", Color) = (1,1,1,1)
        _OutlineSize ("Outline Size", Float) = 1
    }

    SubShader {
        Tags {"Queue"="Transparent" "RenderType"="Transparent"}
        LOD 100

        Pass {
            Name "OUTLINE"
            Blend SrcAlpha OneMinusSrcAlpha
            Cull Off
            ZWrite Off

            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            struct appdata_t {
                float4 vertex : POSITION;
                float2 texcoord : TEXCOORD0;
            };

            struct v2f {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;
            float4 _OutlineColor;
            float _OutlineSize;

            v2f vert (appdata_t v) {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.texcoord, _MainTex);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target {
                float2 uv = i.uv;
                float alpha = 0.0;
                float2 offset[8] = {
                    float2(-_OutlineSize, 0),
                    float2(_OutlineSize, 0),
                    float2(0, -_OutlineSize),
                    float2(0, _OutlineSize),
                    float2(-_OutlineSize, -_OutlineSize),
                    float2(-_OutlineSize, _OutlineSize),
                    float2(_OutlineSize, -_OutlineSize),
                    float2(_OutlineSize, _OutlineSize)
                };

                for (int j = 0; j < 8; j++) {
                    fixed4 col = tex2D(_MainTex, uv + offset[j] / _ScreenParams.xy);
                    alpha = max(alpha, col.a);
                }

                fixed4 texCol = tex2D(_MainTex, uv);
                fixed4 result = texCol;

                if (texCol.a == 0 && alpha > 0) {
                    result.rgb = _OutlineColor.rgb;
                    result.a = _OutlineColor.a;
                }

                return result;
            }
            ENDCG
        }
    }
}
