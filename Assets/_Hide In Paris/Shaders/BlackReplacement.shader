Shader "Custom/BlackReplacement"
{
    Properties
    {
        _MainTex            ("Source", 2D) = "white" {}
        _ReplacementColor   ("Replacement Color", Color) = (1, 0, 0, 1)
        _BrightnessThreshold("Brightness Threshold", Range(0,1)) = 0.1
        _EdgeSoftness       ("Edge Softness", Range(0,1)) = 0.05
        _MaxSaturation      ("Max Saturation", Range(0,1)) = 0.2
    }

    SubShader
    {
        Tags { "RenderType"="Opaque" "RenderPipeline"="UniversalPipeline" }
        ZWrite Off ZTest Always Cull Off

        Pass
        {
            HLSLPROGRAM
            #pragma vertex Vert
            #pragma fragment Frag
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            #include "Packages/com.unity.render-pipelines.core/Runtime/Utilities/Blit.hlsl"

            float4 _ReplacementColor;
            float  _BrightnessThreshold;
            float  _EdgeSoftness;
            float  _MaxSaturation;

            // Returns HSV saturation (0-1) of an RGB color
            float GetSaturation(float3 rgb)
            {
                float maxC = max(rgb.r, max(rgb.g, rgb.b));
                float minC = min(rgb.r, min(rgb.g, rgb.b));
                return (maxC < 0.0001) ? 0.0 : (maxC - minC) / maxC;
            }

            float4 Frag(Varyings input) : SV_Target
            {
                float4 col = SAMPLE_TEXTURE2D(_BlitTexture, sampler_LinearClamp, input.texcoord);

                float brightness  = dot(col.rgb, float3(0.299, 0.587, 0.114));
                float saturation  = GetSaturation(col.rgb);

                // How "black" is this pixel?
                // 1 = fully black, 0 = not black
                float brightnessMatch = 1.0 - smoothstep(
                    _BrightnessThreshold - _EdgeSoftness,
                    _BrightnessThreshold + _EdgeSoftness,
                    brightness
                );

                // Exclude pixels that are dark but saturated (e.g. deep reds/blues)
                float saturationMask = 1.0 - smoothstep(0.0, _MaxSaturation, saturation);

                float blendFactor = brightnessMatch * saturationMask;

                float4 result;
                result.rgb = lerp(col.rgb, _ReplacementColor.rgb, blendFactor * _ReplacementColor.a);
                result.a   = col.a;
                return result;
            }
            ENDHLSL
        }
    }
}