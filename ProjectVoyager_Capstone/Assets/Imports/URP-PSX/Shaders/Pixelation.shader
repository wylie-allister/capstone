Shader "PostEffect/Pixelation"
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
                // for pixelation
                float _WidthPixelation;
                float _HeightPixelation;

                // for color precision
                float _ColorPrecision;
            CBUFFER_END

            float4 Frag(Varyings i) : SV_Target
            {
                // pixelation
                float2 uv = i.texcoord;
                uv.x = floor(uv.x * _WidthPixelation) / _WidthPixelation;
                uv.y = floor(uv.y * _HeightPixelation) / _HeightPixelation;

                float4 color = SAMPLE_TEXTURE2D_X(_BlitTexture, sampler_LinearClamp, uv);

                // color precision
                color = floor(color * _ColorPrecision) / _ColorPrecision;
                return color;
            }
            ENDHLSL
        }
    }
}
