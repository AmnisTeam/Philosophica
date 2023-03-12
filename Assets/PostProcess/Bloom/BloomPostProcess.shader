Shader "Custom/BloomPostProcess"
{
    SubShader
    {
        Cull off ZWrite off ZTest Always

        Pass
        {
            HLSLPROGRAM
            #pragma vertex VertDefault
            #pragma fragment Frag

            #include "Packages/com.unity.postprocessing/PostProcessing/Shaders/StdLib.hlsl"

            // TEXTURE2D_SAMPLER2D(_MainTex, sampler_MainTex);
            // TEXTURE2D_SAMPLER2D(_CameraDepthTexture, sampler_CameraDepthTexture);
            
            // float4 _MainTex_TexelSize;
            
            // float _DepthThreshold;
            // float _Thickness;
            // float _MinDepth;
            // float _MaxDepth;

            float4 Frag(VaryingsDefault i) : SV_Target
            {
                return float4(1, 0, 1, 1);
            }
            ENDHLSL
        }
    }
}
