Shader "PostEffect/Fog"
{
    SubShader
    {
        Tags { "RenderPipeline" = "UniversalPipeline" }
        Cull Off ZWrite Off ZTest Always

        Pass
        {
            HLSLPROGRAM
            #pragma vertex Vert
            #pragma fragment Frag

            #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Common.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/DeclareDepthTexture.hlsl"
            #include "Packages/com.unity.render-pipelines.core/Runtime/Utilities/Blit.hlsl"
            #include "Assets/Imports/URP-PSX/Shaders/cginc/voronoi.hlsl"

            CBUFFER_START(UnityPerMaterial)
                float _FogDensity;
                float _FogDistance;
                float4 _FogColor;

                float _FogNear;
                float _FogFar;
                float _FogAltScale;
                float _FogThinning;

                float _NoiseScale;
                float _NoiseStrength;
            CBUFFER_END

            float ComputeDistance(float depth)
            {
                float dist = depth * _ProjectionParams.z;
                dist -= _ProjectionParams.y * _FogDistance;
                return dist;
            }

            half ComputeFog(float z, float density)
            {
                half fog = exp2(density * z);
                return saturate(fog);
            }

            float4 Frag(Varyings i) : SV_Target
            {
                float2 uv = i.texcoord;
                float2 screenParam = _ScreenParams.xy;

                // base render
                float4 color = SAMPLE_TEXTURE2D_X(_BlitTexture, sampler_LinearClamp, uv);

                float4 ambientColor = float4(0.1, 0.1, 0.1, 0.1);

                // depth handling
                float depth = SampleSceneDepth(uv);
                float linearDepth = Linear01Depth(depth, _ZBufferParams);

                float dist = ComputeDistance(depth);
                float fog = 1.0 - ComputeFog(dist, _FogDensity);

                float screenNoise = cnoise(uv * screenParam / _NoiseScale);

                return lerp(color, _FogColor * ambientColor, saturate(fog + (screenNoise * _NoiseStrength)));
            }
            ENDHLSL
        }
    }
}
