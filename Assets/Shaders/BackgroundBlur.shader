Shader "Custom/BlurBehind"
{
    Properties
    {
        _BlurSize ("Blur Size", Range(0.0, 1.0)) = 0.1
        _DarkenAmount ("Darken Amount", Range(0.0, 1.0)) = 0.0
    }
    SubShader
    {
        Tags { "RenderType" = "Transparent" "Queue" = "Transparent" }

        Pass
        {
            Name "BlurPass"
            Blend SrcAlpha OneMinusSrcAlpha
            ZWrite Off
            Cull Off
            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

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

            Varyings vert(Attributes input)
            {
                Varyings output;
                output.positionHCS = TransformObjectToHClip(input.positionOS);
                output.uv = input.uv;
                return output;
            }

            sampler2D _CameraOpaqueTexture;
            float4 _CameraOpaqueTexture_TexelSize;
            float _BlurSize;
            float _DarkenAmount;

            half4 frag(Varyings input) : SV_Target
            {
                float2 uv = input.uv;

                // Sample neighbouring texels for blur effect
                half4 color = tex2D(_CameraOpaqueTexture, uv) * 0.36;
                color += tex2D(_CameraOpaqueTexture, uv + float2(_BlurSize * _CameraOpaqueTexture_TexelSize.x, 0)) * 0.22;
                color += tex2D(_CameraOpaqueTexture, uv - float2(_BlurSize * _CameraOpaqueTexture_TexelSize.x, 0)) * 0.22;
                color += tex2D(_CameraOpaqueTexture, uv + float2(0, _BlurSize * _CameraOpaqueTexture_TexelSize.y)) * 0.22;
                color += tex2D(_CameraOpaqueTexture, uv - float2(0, _BlurSize * _CameraOpaqueTexture_TexelSize.y)) * 0.22;

                // Apply darkening effect
                color.rgb *= (1.0 - _DarkenAmount);

                return color;
            }
            ENDHLSL
        }
    }
}
