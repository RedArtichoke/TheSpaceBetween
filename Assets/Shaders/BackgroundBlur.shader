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
            ZTest Always
            Cull Off
            HLSLPROGRAM
            #pragma target 3.0
            #pragma vertex vert
            #pragma fragment frag
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            struct Attributes
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct Varyings
            {
                float4 positionHCS : SV_POSITION;
                float2 uv : TEXCOORD0;
            };

            /// <summary>
            /// Transforms to clip space then computes screen space UVs (Y flipped).
            /// </summary>
            Varyings vert(Attributes input)
            {
                Varyings output;
                output.positionHCS = TransformObjectToHClip(input.vertex);
                float4 screenPos = output.positionHCS;
                screenPos /= screenPos.w; // perspective divide
                output.uv = float2(screenPos.x, -screenPos.y) * 0.5 + 0.5;
                return output;
            }

            sampler2D _CameraOpaqueTexture;
            float4 _CameraOpaqueTexture_TexelSize;
            float _BlurSize;
            float _DarkenAmount;

            /// <summary>
            /// Applies a 3x3 box blur and blends with a dark blue tint.
            /// </summary>
            half4 frag(Varyings input) : SV_Target
            {
                float2 uv = input.uv;
                float2 texelSize = _CameraOpaqueTexture_TexelSize.xy;
                float offset = _BlurSize;

                // 3x3 box blur samples
                half4 colour =
                      tex2D(_CameraOpaqueTexture, uv + float2(-offset, -offset) * texelSize)
                    + tex2D(_CameraOpaqueTexture, uv + float2( 0.0,    -offset) * texelSize)
                    + tex2D(_CameraOpaqueTexture, uv + float2( offset, -offset) * texelSize)
                    + tex2D(_CameraOpaqueTexture, uv + float2(-offset,  0.0)    * texelSize)
                    + tex2D(_CameraOpaqueTexture, uv)
                    + tex2D(_CameraOpaqueTexture, uv + float2( offset,  0.0)    * texelSize)
                    + tex2D(_CameraOpaqueTexture, uv + float2(-offset,  offset) * texelSize)
                    + tex2D(_CameraOpaqueTexture, uv + float2( 0.0,     offset) * texelSize)
                    + tex2D(_CameraOpaqueTexture, uv + float2( offset,  offset) * texelSize);
                colour /= 9.0;

                // Blend colour with dark blue to tint it
                float3 darkBlue = float3(0.0, 0.0, 0.01); // chosen dark blue tint
                colour.rgb = lerp(colour.rgb, darkBlue, _DarkenAmount);

                return colour;
            }
            ENDHLSL
        }
    }
}
