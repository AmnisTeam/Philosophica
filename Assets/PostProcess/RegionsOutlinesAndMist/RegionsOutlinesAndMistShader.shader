Shader "Custom/RegionsOutlinesAndMistShader"
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

            TEXTURE2D_SAMPLER2D(_MainTex, sampler_MainTex);
            TEXTURE2D_SAMPLER2D(_CameraDepthTexture, sampler_CameraDepthTexture);
            TEXTURE2D_SAMPLER2D(_CameraNormalsTexture, sampler_CameraNormalsTexture);

            sampler2D _NoShaderTexture;
            sampler2D _RegionsColorsTexture;
            sampler2D _OutlineTexture;
            sampler2D _InnerGlowTexture;
            sampler2D _SelectionTexture;
            float2 _MainTex_TexelSize;


            float4 get_outlines(float2 uv, sampler2D regionsTexture, float thickness)
            {
                float halfScaleFloor = floor(thickness * 0.5);
                float halfScaleCeil = ceil(thickness * 0.5);

                float2 bottomLeftUV = uv - float2(_MainTex_TexelSize.x, _MainTex_TexelSize.y) * halfScaleFloor;
                float2 topRightUV = uv + float2(_MainTex_TexelSize.x, _MainTex_TexelSize.y) * halfScaleCeil;  
                float2 bottomRightUV = uv + float2(_MainTex_TexelSize.x * halfScaleCeil, -_MainTex_TexelSize.y * halfScaleFloor);
                float2 topLeftUV = uv + float2(-_MainTex_TexelSize.x * halfScaleFloor, _MainTex_TexelSize.y * halfScaleCeil);

                float4 ceterDepth = tex2D(regionsTexture, uv);
                float4 depth0 = tex2D(regionsTexture, bottomLeftUV);
                float4 depth1 = tex2D(regionsTexture, topRightUV);
                float4 depth2 = tex2D(regionsTexture, bottomRightUV);
                float4 depth3 = tex2D(regionsTexture, topLeftUV);

                float4 depthFiniteDifference0 = depth1 - depth0;
                float4 depthFiniteDifference1 = depth3 - depth2;

                float4 edgeDepth = sqrt(pow(depthFiniteDifference0, 2) + pow(depthFiniteDifference1, 2));

                return edgeDepth;
            }

            float4 Frag(VaryingsDefault i) : SV_Target
            {
                float4 mainColor = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, i.texcoord);
                float4 regionsOnlyColor = tex2D(_NoShaderTexture, i.texcoord);
                float4 regionsColor = tex2D(_RegionsColorsTexture, i.texcoord);
                float4 outlineColor = tex2D(_OutlineTexture, i.texcoord);
                float4 outline = get_outlines(i.texcoord, _OutlineTexture, 2);
                float4 selectionColor = tex2D(_SelectionTexture, i.texcoord);
                float4 color = 0;


                // float4 blurColor = (tex2D(_NoShaderTexture, i.texcoord) + 
                //                    tex2D(_NoShaderTexture, i.texcoord + _MainTex_TexelSize.x) + 
                //                    tex2D(_NoShaderTexture, i.texcoord + _MainTex_TexelSize.y) +
                //                    tex2D(_NoShaderTexture, i.texcoord + _MainTex_TexelSize)) / 4;
                
                float4 whiteAndBlackColor = 0;
                if (outlineColor.x >= 0.01 || outlineColor.y >= 0.01 || outlineColor.z >= 0.01)
                    whiteAndBlackColor = 1;

                color += mainColor;
                color += regionsOnlyColor * 0.2f;
                color += outline;
                color += whiteAndBlackColor * tex2D(_InnerGlowTexture, i.texcoord) * 2;

                if (outline.x + outline.y + outline.z > 0.1)
                    return outline;

                if (selectionColor.x + selectionColor.y + selectionColor.z > 0.1)
                    return selectionColor;
                
                return color;

                //if (selectionColor.a != 0)
                //    return selectionColor;
                //else
                //    return mainColor + regionsOnlyColor * 0.2f + get_outlines(i.texcoord, _OutlineTexture, 2) + whiteAndBlackColor * tex2D(_InnerGlowTexture, i.texcoord) * 2;
                //return outlineColor;
                //return tex2D(_SelectionTexture, i.texcoord);
            }
            ENDHLSL
        }
    }
}
