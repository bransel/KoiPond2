Shader "Unlit/Test"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
    }
    SubShader
    {
        Tags { "Queue"="Transparent" "RenderType"="Transparent" }
        blend SrcAlpha OneMinusSrcAlpha
        LOD 100

        Pass
        {
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
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                return o;
            }

            float fract (float x)
            {
                return x - floor(x);
            }

            float normalizedTime()
            {
                float PI = 3.14159;
                return (sin(PI * _Time.y) + 1.0) / 2.0;
            }

            float circle(float2 uv, float radius)
            {
                return 1 - step(radius / 2, distance(uv, fixed2(0.5, 0.5)));
            }

            float ring(float2 uv, float startRadius, float endRadius)
            {
                startRadius = clamp(startRadius, 0, 1);
                endRadius = clamp(endRadius, 0, 1);
                
                return (step(startRadius / 2, distance(uv, fixed2(0.5, 0.5))) - step(endRadius / 2, distance(uv, fixed2(0.5, 0.5))));
            }

            fixed4 frag (v2f i) : SV_Target
            {
                //fixed4 col = fixed4(1, 1, 1, circle(i.uv, sqrt(2)) - circle(i.uv, 0.8));
                //fixed4 col = fixed4(1, 1, 1, ring(i.uv, normalizedTime(), clamp(normalizedTime() - 0.2, 0, 1)));
                fixed4 col = fixed4(1, 1, 1, ring(i.uv, normalizedTime() - 0.2, normalizedTime()));
                return col;

                //float pct = distance(i.uv, fixed2(0.5, 0.5));
                //return fixed4(1, 1, 1, clamp(1 - (pct * 2), 0, 1));
            }
            ENDCG
        }
    }
}
