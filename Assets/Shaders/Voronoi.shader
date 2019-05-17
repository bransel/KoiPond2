Shader "Custom/Voronoi"
{
    Properties
    {
        [HDR]_Color ("Color", Color) = (1,1,1,1)
        _MainTex ("Albedo (RGB)", 2D) = "white" {}
        _Glossiness ("Smoothness", Range(0,1)) = 0.5
        _Metallic ("Metallic", Range(0,1)) = 0.0

        _RippleSpeed ("Ripple Speed", float) = 0.75
        [HDR]_RippleColor ("Ripple Color", Color) = (0.196, 0.741, 1, 1)
        _RippleScale ("Ripple Scale", float) = 5
        _RippleDissolve ("Ripple Dissolve", float) = 5
        _NormalStrength ("Normal Strength", float) = 1
    }
    SubShader
    {
        Tags { "Queue"="Transparent" "RenderType"="Transparent" }
        blend SrcAlpha OneMinusSrcAlpha
        LOD 200

        CGPROGRAM
        #pragma surface surf Standard fullforwardshadows vertex:vert alpha
        #pragma target 3.5

        sampler2D _MainTex;

        struct Input
        {
            float2 uv_MainTex;

            float3 worldPos;
            float3 worldNormal; INTERNAL_DATA

            float3 worldTangent;
            float3 worldBiTangent;
        };

        void vert (inout appdata_full v, out Input o)
        {
            UNITY_INITIALIZE_OUTPUT(Input, o);

            o.worldNormal = mul((float3x3)unity_ObjectToWorld, v.normal);
            o.worldTangent = normalize(mul((float3x3)UNITY_MATRIX_M, v.tangent.xyz));
            o.worldBiTangent = cross(o.worldNormal, o.worldTangent.xyz) * v.tangent.w;
        }

        half _Glossiness;
        half _Metallic;
        fixed4 _Color;

        half _RippleScale;
        half _RippleSpeed;
        fixed4 _RippleColor;
        half _RippleDissolve;
        half _NormalStrength;

        inline float2 unity_voronoi_noise_randomVector (float2 UV, float offset)
        {
            float2x2 m = float2x2(15.27, 47.63, 99.41, 89.98);
            UV = frac(sin(mul(UV, m)) * 46839.32);
            return float2(sin(UV.y * +offset) * 0.5+  0.5, cos(UV.x * offset) * 0.5 + 0.5);
        }

        void Unity_Voronoi_float(float2 UV, float AngleOffset, float CellDensity, out float Out, out float voronoiCellsOut)
        {
            float2 g = floor(UV * CellDensity);
            float2 f = frac(UV * CellDensity);
            float t = 8.0;
            float3 res = float3(8.0, 0.0, 0.0);

            for (int y = -1; y <= 1; y++)
            {
                for (int x = -1; x <= 1; x++)
                {
                    float2 lattice = float2(x,y);
                    float2 offset = unity_voronoi_noise_randomVector(lattice + g, AngleOffset);
                    float d = distance(lattice + offset, f);

                    if (d < res.x)
                    {
                        res = float3(d, offset.x, offset.y);
                        Out = res.x;
                        voronoiCellsOut = res.y;
                    }
                }
            }
        }

        void Unity_RadialShear_float(float2 UV, float2 Center, float2 Strength, float2 Offset, out float2 Out)
        {
            float2 delta = UV - Center;
            float delta2 = dot(delta.xy, delta.xy);
            float2 delta_offset = delta2 * Strength;
            Out = UV + float2(delta.y, -delta.x) * delta_offset + Offset;
        }

        float3 TransformWorldToTangent(float3 dirWS, float3x3 worldToTangent)
        {
            return mul(worldToTangent, dirWS);
        }

        void Unity_NormalFromHeight_Tangent(float In, float3 Position, float3x3 TangentMatrix, out float3 Out)
        {
            float3 worldDirivativeX = ddx(Position * 100);
            float3 worldDirivativeY = ddy(Position * 100);

            float3 crossX = cross(TangentMatrix[2].xyz, worldDirivativeX);
            float3 crossY = cross(TangentMatrix[2].xyz, worldDirivativeY);
            float3 d = abs(dot(crossY, worldDirivativeX));
            float3 inToNormal = ((((In + ddx(In)) - In) * crossY) + (((In + ddy(In)) - In) * crossX)) * sign(d);
            inToNormal.y *= -1.0;

            Out = normalize((d * TangentMatrix[2].xyz) - inToNormal);
            Out = TransformWorldToTangent(Out, TangentMatrix);
        }

        void Unity_NormalStrength_float(float3 In, float Strength, out float3 Out)
        {
            Out = float3(In.rg * Strength, lerp(1, In.b, saturate(Strength)));
        }

        // Add instancing support for this shader. You need to check 'Enable Instancing' on materials that use the shader.
        // See https://docs.unity3d.com/Manual/GPUInstancing.html for more information about instancing.
        // #pragma instancing_options assumeuniformscaling
        UNITY_INSTANCING_BUFFER_START(Props)
            // put more per-instance properties here
        UNITY_INSTANCING_BUFFER_END(Props)

        void surf (Input IN, inout SurfaceOutputStandard o)
        {
            fixed4 c = tex2D (_MainTex, IN.uv_MainTex) * _Color;

            half angle = (_Time.y + 5) * _RippleSpeed;

            fixed2 radialOut;
            Unity_RadialShear_float(IN.uv_MainTex, float2(0.5, 0.5), float2(1, 1), float2(0, 0), radialOut);

            half voronoiOut;
            half voronoiCellsOut;
            Unity_Voronoi_float(radialOut, angle, _RippleScale, voronoiOut, voronoiCellsOut);

            fixed3 voronoiPow = pow(voronoiOut, _RippleDissolve);

            fixed3 voronoiMult = _RippleColor * voronoiPow.xxx;

            fixed3 heightNormalOut;
            float3 worldNormal = WorldNormalVector (IN, o.Normal);
            float3x3 tangentMatrix = float3x3(IN.worldTangent, IN.worldBiTangent, worldNormal);
            Unity_NormalFromHeight_Tangent(voronoiMult, IN.worldPos, tangentMatrix, heightNormalOut);

            fixed3 normalStrengthOut;
            Unity_NormalStrength_float(heightNormalOut, _NormalStrength, normalStrengthOut);

            o.Albedo = c.rgb;
            o.Emission = voronoiMult;
            o.Normal = normalStrengthOut;
            o.Metallic = _Metallic;
            o.Smoothness = _Glossiness;
            o.Alpha = c.a;
        }
        ENDCG
    }
    FallBack "Diffuse"
}
