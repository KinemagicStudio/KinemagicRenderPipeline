Shader "Kinemagic/Universal/FilmGrain"
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

            TEXTURE2D(_GrainTexture);
            SAMPLER(sampler_GrainTexture);

            CBUFFER_START(UnityPerMaterial)
            float3 _GrainParams;       // x: intensity, y: response, z: unused
            float4 _GrainTextureParams; // xy: tiling, zw: offset
            CBUFFER_END

            float4 Frag(Varyings input) : SV_Target
            {
                float4 color = SAMPLE_TEXTURE2D_X(_BlitTexture, sampler_LinearClamp, input.texcoord);
                float2 uv = input.texcoord;

                float intensity = _GrainParams.x;
                float response = _GrainParams.y;

                // Sample grain texture with tiling and random offset
                float2 grainUV = uv * _GrainTextureParams.xy + _GrainTextureParams.zw;
                float grain = SAMPLE_TEXTURE2D(_GrainTexture, sampler_GrainTexture, grainUV).w;

                // Remap grain from [0,1] to [-1,1]
                grain = (grain - 0.5) * 2.0;

                // Calculate luminance
                float luma = dot(color.rgb, float3(0.2126, 0.7152, 0.0722));

                // Apply response curve - less grain on bright areas
                luma = 1.0 - sqrt(luma);
                float grainAmount = intensity * lerp(1.0, luma, response);

                // Apply film grain
                color.rgb += color.rgb * grain * grainAmount;

                return color;
            }

            ENDHLSL
        }
    }
}
