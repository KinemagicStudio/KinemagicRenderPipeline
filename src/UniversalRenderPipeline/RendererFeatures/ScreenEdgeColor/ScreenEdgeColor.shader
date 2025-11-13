Shader "Kinemagic/Universal/ScreenEdgeColor"
{
    SubShader
    {
        Tags { "RenderPipeline" = "UniversalPipeline" }

        ZTest Always
        ZWrite Off
        Cull Off
        Blend Off

        Pass
        {
            HLSLPROGRAM
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            #include "Packages/com.unity.render-pipelines.core/Runtime/Utilities/Blit.hlsl"

            #pragma vertex Vert
            #pragma fragment Frag

            CBUFFER_START(UnityPerMaterial)
            float _Intensity;
            float4 _ColorArray[4];
            CBUFFER_END

            float4 Frag(Varyings input) : SV_Target
            {
                float4 output = SAMPLE_TEXTURE2D_X(_BlitTexture, sampler_LinearClamp, input.texcoord);
                float2 uv = input.texcoord;

                float4 colorTop = lerp(_ColorArray[0], _ColorArray[1], uv.x); // Top-left and Top-right
                float4 colorBottom = lerp(_ColorArray[2], _ColorArray[3], uv.x); // Bottom-left and Bottom-right
                float4 gradientColor = lerp(colorBottom, colorTop, uv.y);

                output.rgb = 1 - (1 - output.rgb) * (1 - gradientColor.rgb * gradientColor.a * _Intensity);
                return output;
            }

            ENDHLSL
        }
    }
}
