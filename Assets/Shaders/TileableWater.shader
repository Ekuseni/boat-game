Shader "Boat Game/Tileable Water"
{
    Properties
    {
        _DeepColor ("Deep Color", Color) = (0.015, 0.16, 0.24, 1)
        _ShallowColor ("Shallow Color", Color) = (0.04, 0.48, 0.57, 1)
        _HighlightColor ("Highlight Color", Color) = (0.35, 0.82, 0.79, 1)
        _Tiling ("Tiling", Float) = 1
        _WaveSpeed ("Wave Speed", Float) = 0.35
        _WaveContrast ("Wave Contrast", Range(0.1, 4)) = 1
    }

    SubShader
    {
        Tags
        {
            "RenderType" = "Opaque"
            "RenderPipeline" = "UniversalPipeline"
            "Queue" = "Geometry"
        }

        Pass
        {
            Name "ForwardUnlit"
            Tags
            {
                "LightMode" = "UniversalForward"
            }

            HLSLPROGRAM
            #pragma vertex Vert
            #pragma fragment Frag

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            CBUFFER_START(UnityPerMaterial)
                half4 _DeepColor;
                half4 _ShallowColor;
                half4 _HighlightColor;
                float _Tiling;
                float _WaveSpeed;
                float _WaveContrast;
            CBUFFER_END

            struct Attributes
            {
                float4 positionOS : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct Varyings
            {
                float4 positionHCS : SV_POSITION;
                float2 uv : TEXCOORD0;
            };

            Varyings Vert(Attributes input)
            {
                Varyings output;
                output.positionHCS = TransformObjectToHClip(input.positionOS.xyz);
                output.uv = input.uv * _Tiling;
                return output;
            }

            half4 Frag(Varyings input) : SV_Target
            {
                const float tau = 6.28318530718;
                float time = _Time.y * _WaveSpeed;

                // Integer UV frequencies keep every wave continuous across tile edges.
                float broadWave = sin(tau * (input.uv.x * 2.0 + input.uv.y + time * 0.12));
                float crossWave = sin(tau * (input.uv.x - input.uv.y * 2.0 - time * 0.08));
                float fineWave = sin(tau * (input.uv.x * 3.0 + input.uv.y * 2.0 + time * 0.2));

                float surface = saturate(0.5 + (broadWave * 0.42 + crossWave * 0.3 + fineWave * 0.12) * _WaveContrast);
                float glint = smoothstep(0.82, 0.98, broadWave * 0.35 + crossWave * 0.3 + fineWave * 0.35);
                half3 water = lerp(_DeepColor.rgb, _ShallowColor.rgb, surface);
                water = lerp(water, _HighlightColor.rgb, glint * 0.35);

                return half4(water, 1.0);
            }
            ENDHLSL
        }
    }
}