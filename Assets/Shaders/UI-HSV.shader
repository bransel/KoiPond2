Shader "UI/HSV"
{
    Properties
    {
        [PerRendererData] _MainTex ("Sprite Texture", 2D) = "white" {}
        _Color ("Tint", Color) = (1,1,1,1)
        _Mode ("Mode (H=0, S=1, V=2)", int) = 0

        _StencilComp ("Stencil Comparison", Float) = 8
        _Stencil ("Stencil ID", Float) = 0
        _StencilOp ("Stencil Operation", Float) = 0
        _StencilWriteMask ("Stencil Write Mask", Float) = 255
        _StencilReadMask ("Stencil Read Mask", Float) = 255

        _ColorMask ("Color Mask", Float) = 15

        [Toggle(UNITY_UI_ALPHACLIP)] _UseUIAlphaClip ("Use Alpha Clip", Float) = 0
    }

    SubShader
    {
        Tags
        {
            "Queue"="Transparent"
            "IgnoreProjector"="True"
            "RenderType"="Transparent"
            "PreviewType"="Plane"
            "CanUseSpriteAtlas"="True"
        }

        Stencil
        {
            Ref [_Stencil]
            Comp [_StencilComp]
            Pass [_StencilOp]
            ReadMask [_StencilReadMask]
            WriteMask [_StencilWriteMask]
        }

        Cull Off
        Lighting Off
        ZWrite Off
        ZTest [unity_GUIZTestMode]
        Blend SrcAlpha OneMinusSrcAlpha
        ColorMask [_ColorMask]

        Pass
        {
            Name "Default"
        CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma target 2.0

            #include "UnityCG.cginc"
            #include "UnityUI.cginc"

            #pragma multi_compile_local _ UNITY_UI_CLIP_RECT
            #pragma multi_compile_local _ UNITY_UI_ALPHACLIP

            struct appdata_t
            {
                float4 vertex   : POSITION;
                float4 color    : COLOR;
                float2 texcoord : TEXCOORD0;
                UNITY_VERTEX_INPUT_INSTANCE_ID
            };

            struct v2f
            {
                float4 vertex   : SV_POSITION;
                fixed4 color    : COLOR;
                float2 texcoord  : TEXCOORD0;
                float4 worldPosition : TEXCOORD1;
                UNITY_VERTEX_OUTPUT_STEREO
            };

            sampler2D _MainTex;
            fixed4 _Color;
            fixed4 _TextureSampleAdd;
            float4 _ClipRect;
            float4 _MainTex_ST;

            int _Mode;

            v2f vert(appdata_t v)
            {
                v2f OUT;
                UNITY_SETUP_INSTANCE_ID(v);
                UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(OUT);
                OUT.worldPosition = v.vertex;
                OUT.vertex = UnityObjectToClipPos(OUT.worldPosition);

                OUT.texcoord = TRANSFORM_TEX(v.texcoord, _MainTex);

                OUT.color = v.color * _Color;
                return OUT;
            }

            float Epsilon = 1e-10;
 
            float3 RGBtoHCV(in float3 RGB)
            {
                // Based on work by Sam Hocevar and Emil Persson
                float4 P = (RGB.g < RGB.b) ? float4(RGB.bg, -1.0, 2.0/3.0) : float4(RGB.gb, 0.0, -1.0/3.0);
                float4 Q = (RGB.r < P.x) ? float4(P.xyw, RGB.r) : float4(RGB.r, P.yzx);
                float C = Q.x - min(Q.w, Q.y);
                float H = abs((Q.w - Q.y) / (6 * C + Epsilon) + Q.z);
                return float3(H, C, Q.x);
            }

            float3 RGBtoHSV(in float3 RGB)
            {
                float3 HCV = RGBtoHCV(RGB);
                float S = HCV.y / (HCV.z + Epsilon);
                return float3(HCV.x, S, HCV.z);
            }

            half4 GetHueGradient(half value)
            {
                if (value >= 0 && value < .165)
                {
                    return lerp(half4(1,0,0,1), half4(1,1,0,1), value * 6);
                }
                if (value >= .165 && value < .33)
                {
                    return lerp(half4(1,1,0,1), half4(0,1,0,1), (value - .165) * 6);
                }
                else if (value >= .33 && value < .495)
                {
                    return lerp(half4(0,1,0,1), half4(0,1,1,1), (value - .33) * 6);
                }
                else if (value >= .495 && value < .66)
                {
                    return lerp(half4(0,1,1,1), half4(0,0,1,1), (value - .495) * 6);
                }
                else if (value >= .66 && value < .825)
                {
                    return lerp(half4(0,0,1,1), half4(1,0,1,1), (value - .66) * 6);
                }
                else if (value >= .825 && value < 1)
                {
                    return lerp(half4(1,0,1,1), half4(1,0,0,1), (value - .825) * 6);
                }
                else
                {
                    return half4(1,0,0,1);
                }
            }

            fixed4 frag(v2f IN) : SV_Target
            {
                half4 color = (tex2D(_MainTex, IN.texcoord) + _TextureSampleAdd) * IN.color;

                float3 hsv = RGBtoHSV(half3(color.x, color.y, color.z));

                if (_Mode == 0)
                    color = GetHueGradient(IN.texcoord.x);
                else if (_Mode == 1)
                    color = lerp(half4(0,0,0,1), lerp(half4(1,1,1,1), GetHueGradient(hsv.x), IN.texcoord.x), hsv.z);
                else if (_Mode == 2)
                    color = lerp(half4(0,0,0,1), lerp(half4(1,1,1,1), GetHueGradient(hsv.x), hsv.y), IN.texcoord.x);

                color.a = 1;
                return color;
            }
        ENDCG
        }
    }
}
