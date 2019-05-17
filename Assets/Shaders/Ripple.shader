Shader "GrabPassInvert"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
    }
    SubShader
    {
        // Draw ourselves after all opaque geometry
        Tags { "Queue" = "Transparent" "RenderType" = "Transparent" }
        blend SrcAlpha OneMinusSrcAlpha

        // Grab the screen behind the object into _BackgroundTexture
        GrabPass
        {
            "_BackgroundTexture"
        }

        // Render the object with the texture generated above, and invert the colors
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
                float4 grabPos : TEXCOORD1;
                float4 pos : SV_POSITION;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;

            v2f vert(appdata v) {
                v2f o;
                // use UnityObjectToClipPos from UnityCG.cginc to calculate 
                // the clip-space of the vertex
                o.pos = UnityObjectToClipPos(v.vertex);
                // use ComputeGrabScreenPos function from UnityCG.cginc
                // to get the correct texture coordinate
                o.grabPos = ComputeGrabScreenPos(o.pos);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                return o;
            }

            sampler2D _BackgroundTexture;

            float fract (float x)
            {
                return x - floor(x);
            }

            half4 frag(v2f i) : SV_Target
            {
                fixed4 col = tex2D(_MainTex, i.uv);
                //half4 bgcolor = tex2Dproj(_BackgroundTexture, i.grabPos);

                //output.grabPos = ComputeScreenGrabPos(output.pos);
                //float noise = tex2Dlod(_Noise, float4(input.texCoord, 0)).rgb;
                //output.grabPos.x += noise * _Strength;
                //output.grabPos.y += noise * _Strength;

                float PI = 3.14159;
                fixed2 uv = i.uv - .5;
    
                float dt = fract(_Time.y / 2.0) * 2.0;
                float time = PI * dt * 2.0;

                float uvLength = length(uv);
                float offset = uv.y * cos(uvLength * PI * 2.0 * 4.0 -time) * 0.5;
            
                float amplitude = 1.0 - distance(i.uv, fixed2(0.5, 0.5));
                offset *= pow(amplitude, 4.0);
            
                fixed4 fragColor = fixed4(0.96, 0.96, 0.96, 1.0) + offset * 5;
                
                fixed4 bgcolor = tex2Dproj(_BackgroundTexture, i.grabPos + offset * 2);
                
                return col * bgcolor * fragColor;
            }
            ENDCG
        }

    }
}