Shader "Custom/Position Ripple"
{
    Properties
    {
        _Frequency ("Frequency", Range(0,100)) = 10
        _Amplitude ("Amplitude", Range(0,2)) = 0.1
        _Falloff ("Falloff", Range(0,30)) = 2
        _Speed ("Speed", Range(-3,3)) = 0.5
    }
    SubShader
    {
        Tags { "Queue" = "Transparent" "RenderType" = "Transparent" }
        blend SrcAlpha OneMinusSrcAlpha

        GrabPass
        {
            "_BackgroundTexture"
        }

        Pass
        {
            CGPROGRAM
            #pragma target 4.5
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
                float4 grabPos : TEXCOORD1;
                float4 worldPos : TEXCOORD2;
                float4 pos : SV_POSITION;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;

            v2f vert(appdata v) {
                v2f o;
                o.worldPos = mul(unity_ObjectToWorld,v.vertex);
                o.pos = UnityObjectToClipPos(v.vertex);
                o.grabPos = ComputeGrabScreenPos(o.pos);
                return o;
            }

            sampler2D _BackgroundTexture;
            float _Frequency;
            float _Amplitude;
            float _Falloff;
            float _Speed;

            struct RippleInfo{
                float3 position;
                float falloff;
            };
            struct polar{
                float radius;
                float theta;
            };

            polar ToPolar(float2 xy){
                polar pol;
                pol.radius = length(xy);
                pol.theta = atan2(xy.y,xy.x);
                return pol;
            }

            StructuredBuffer<RippleInfo> rippleObjects : register(t1);

            float GetRippleAmount(polar pol,float falloff){

                float amp = clamp(1-(pol.radius / (falloff * _Falloff)),0,1);
                float freq = sin((pol.radius - _Time.y * _Speed) * _Frequency);
                return freq *amp;
            }


            half4 frag(v2f IN) : SV_Target
            {
            uint numStructs;
            uint stride;
            rippleObjects.GetDimensions(numStructs,stride);

            // if(numStructs == 0)
            // return fixed4(1,0,0,1);
            float4 offsetPos= float4(0,0,0,0);
            for(uint i = 0; i < numStructs; i++){
                float3 dpos = rippleObjects[i].position - IN.worldPos;
                polar pol = ToPolar(dpos);
                float val = GetRippleAmount(pol,rippleObjects[i].falloff);
                // valSum += val * 0.5 + 0.5;
                offsetPos.xy += float2(cos(pol.theta),sin(pol.theta)) * val * _Amplitude;
            }

            fixed4 bgcolor = tex2Dproj(_BackgroundTexture, IN.grabPos + offsetPos);
            return bgcolor;
            }

            ENDCG
        }

    }
}
