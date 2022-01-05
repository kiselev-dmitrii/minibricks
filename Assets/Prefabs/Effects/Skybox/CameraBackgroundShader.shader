Shader "Game/Camera Background" {
    Properties {
        _TopColor ("Top Color", Color) = (1,1,1,1)
        _BottomColor ("Bottom Color", Color) = (0,0,0,1)
    }
    SubShader {
        Tags { "RenderType"="Opaque" }
        LOD 100

        Pass {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            
            #include "UnityCG.cginc"

            struct appdata {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
            };

            v2f vert (appdata v) {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }

            uniform fixed4 _TopColor;
            uniform fixed4 _BottomColor;
            
            fixed4 frag (v2f i) : SV_Target {
                float t = i.uv.y;
                fixed4 color = lerp(_BottomColor, _TopColor, t);
                return color;
            }
            ENDCG
        }
    }
}