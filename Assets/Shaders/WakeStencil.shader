Shader "Boat Game/Wake Stencil"
{
    Properties
    {
        _FoamColor ("Foam Color", Color) = (0.9, 0.97, 1, 1)
        _Opacity ("Opacity", Range(0, 1)) = 0.8
        _EdgeSoftness ("Edge Softness", Range(0.01, 1)) = 0.5
        _Stripes ("Foam Stripes", Float) = 12
        _StripeSpeed ("Stripe Speed", Float) = 0.5
        _StencilRef ("Stencil Ref", Range(0, 255)) = 1
    }

    SubShader
    {
        Tags
        {
            "RenderType" = "Transparent"
            "RenderPipeline" = "UniversalPipeline"
            "Queue" = "Transparent"
        }

        Pass
        {
            Name "Wake"
            Tags { "LightMode" = "UniversalForward" }

            // Each pixel is written once, so overlapping trail segments don't stack and darken/brighten.
            Stencil
            {
                Ref [_StencilRef]
                Comp NotEqual
                Pass Replace
            }

            Blend SrcAlpha OneMinusSrcAlpha
            ZWrite Off
            Cull Off
            Offset -1, -1

            HLSLPROGRAM
            #pragma vertex Vert
            #pragma fragment Frag

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            CBUFFER_START(UnityPerMaterial)
                half4 _FoamColor;
                float _Opacity;
                float _EdgeSoftness;
                float _Stripes;
                float _StripeSpeed;
                float _StencilRef;
            CBUFFER_END

            struct Attributes
            {
                float4 positionOS : POSITION;
                float2 uv : TEXCOORD0;
                half4 color : COLOR;
            };

            struct Varyings
            {
                float4 positionHCS : SV_POSITION;
                float2 uv : TEXCOORD0;
                half4 color : COLOR;
            };

            Varyings Vert(Attributes input)
            {
                Varyings output;
                output.positionHCS = TransformObjectToHClip(input.positionOS.xyz);
                output.uv = input.uv;
                output.color = input.color;
                return output;
            }

            half4 Frag(Varyings input) : SV_Target
            {
                // TrailRenderer: uv.x runs along the trail (Stretch mode), uv.y across it.
                float across = abs(input.uv.y * 2.0 - 1.0);
                float edge = 1.0 - smoothstep(1.0 - _EdgeSoftness, 1.0, across);
                float stripes = 0.65 + 0.35 * sin(6.28318 * (input.uv.x * _Stripes - _Time.y * _StripeSpeed));
                // Vertex color alpha carries the trail's fade-out over its lifetime.
                float alpha = edge * stripes * input.color.a * _Opacity;
                return half4(_FoamColor.rgb, alpha);
            }
            ENDHLSL
        }
    }
}
