Shader "PostEffect/Dithering"
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
            #include "Packages/com.unity.render-pipelines.core/Runtime/Utilities/Blit.hlsl"

            CBUFFER_START(UnityPerMaterial)
                // for dither
                uint _PatternIndex;
                float _DitherThreshold;
                float _DitherStrength;
                float _DitherScale;
            CBUFFER_END

            float4x4 GetDitherPattern(uint index)
            {
                float4x4 pattern;

                if (index == 0)
                {
                    pattern = float4x4
                    (
                        0, 1, 0, 1,
                        1, 0, 1, 0,
                        0, 1, 0, 1,
                        1, 0, 1, 0
                    );
                }
                else if (index == 1)
                {
                    pattern = float4x4
                    (
                        0.23, 0.2, 0.6, 0.2,
                        0.2, 0.43, 0.2, 0.77,
                        0.88, 0.2, 0.87, 0.2,
                        0.2, 0.46, 0.2, 0
                    );
                }
                else if (index == 2)
                {
                    pattern = float4x4
                    (
                        -4.0, 0.0, -3.0, 1.0,
                        2.0, -2.0, 3.0, -1.0,
                        -3.0, 1.0, -4.0, 0.0,
                        3.0, -1.0, 2.0, -2.0
                    );
                }
                else if (index == 3)
                {
                    pattern = float4x4
                    (
                        1, 0, 0, 1,
                        0, 1, 1, 0,
                        0, 1, 1, 0,
                        1, 0, 0, 1
                    );
                }
                else
                {
                    pattern = float4x4
                    (
                        1, 1, 1, 1,
                        1, 1, 1, 1,
                        1, 1, 1, 1,
                        1, 1, 1, 1
                    );
                }

                return pattern;
            }

            float PixelBrightness(float3 col)
            {
                return col.r + col.g + col.b / 3.0; // can use averaging or the dot product to evaluate brightness
                //return dot(col, float3(0.34543, 0.65456, 0.287));
            }

            float Get4x4TexValue(float2 uv, float brightness, float4x4 pattern)
            {
                uint x = uv.x % 4;
                uint y = uv.y % 4;

                if ((brightness * _DitherThreshold) < pattern[x][y])
                    return 0;
                else
                    return 1;
            }

            float4 Frag(Varyings i) : SV_Target
            {
                // base render
                float4 color = SAMPLE_TEXTURE2D_X(_BlitTexture, sampler_LinearClamp, i.texcoord);

                // dithering
                uint2 ditherCoordinate = i.texcoord * _ScreenParams.xy;
                ditherCoordinate /= _DitherScale;

                float brightness = PixelBrightness(color.rgb);
                float4x4 ditherPattern = GetDitherPattern(_PatternIndex);
                float ditherPixel = Get4x4TexValue(ditherCoordinate.xy, brightness, ditherPattern);

                return color * ditherPixel;
            }
            ENDHLSL
        }
    }
}
