Shader "Hidden/Roystan/Outline Post Process"
{
	SubShader
	{
		Cull Off ZWrite Off ZTest Always

		Pass
		{
			// Custom post processing effects are written in HLSL blocks,
			// with lots of macros to aid with platform differences.
			// https://github.com/Unity-Technologies/PostProcessing/wiki/Writing-Custom-Effects#shader
			HLSLPROGRAM
			#pragma vertex VertDefault
			#pragma fragment Frag

			#include "Packages/com.unity.postprocessing/PostProcessing/Shaders/StdLib.hlsl"

			TEXTURE2D_SAMPLER2D(_MainTex, sampler_MainTex);
			// _CameraNormalsTexture contains the view space normals transformed
			// to be in the 0...1 range.
			TEXTURE2D_SAMPLER2D(_CameraNormalsTexture, sampler_CameraNormalsTexture);
			TEXTURE2D_SAMPLER2D(_CameraDepthTexture, sampler_CameraDepthTexture);
			
			// Data pertaining to _MainTex's dimensions.
			// https://docs.unity3d.com/Manual/SL-PropertiesInPrograms.html
			float4 _MainTex_TexelSize;
			float _Scale;
			float _DepthThreshold;
			float _Intensity;

			// Combines the top and bottom colors using normal blending.
			// https://en.wikipedia.org/wiki/Blend_modes#Normal_blend_mode
			// This performs the same operation as Blend SrcAlpha OneMinusSrcAlpha.
			float4 alphaBlend(float4 top, float4 bottom)
			{
				float3 color = (top.rgb * top.a) + (bottom.rgb * (1 - top.a));
				float alpha = top.a + bottom.a * (1 - top.a);

				return float4(color, alpha);
			}

			float4 Frag(VaryingsDefault i) : SV_Target
			{
				float4 color = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, i.texcoord);

				float halfScaleFloor = floor(_Scale * 0.5);
				float halfScaleCeil = ceil(_Scale * 0.5);

				float2 texelSize = float2(_MainTex_TexelSize.x, _MainTex_TexelSize.y);
				float2 uvOrigin = i.texcoord;
				float depthOrigin = SAMPLE_DEPTH_TEXTURE(_CameraDepthTexture, sampler_CameraDepthTexture, i.texcoord).r;

				float2 rightTop = texelSize;
				float2 rightBottom = float2(texelSize.x, -texelSize.y);
				float2 leftBottom = float2(-texelSize.x, -texelSize.y);
				float2 leftTop = float2(-texelSize.x, texelSize.y);

				float2 uv0 = uvOrigin;
				float2 uv1 = uvOrigin;
				float2 uv2 = uvOrigin;
				float2 uv3 = uvOrigin;

				float dst0 = 0;
				float dst1 = 0;
				float dst2 = 0;
				float dst3 = 0;

				float depth0 = 0;
				float depth1 = 0;
				float depth2 = 0;
				float depth3 = 0;

				float diff0 = 0;
				float diff1 = 0;
				float diff2 = 0;
				float diff3 = 0;


				bool found0 = false;
				bool found1 = false;
				bool found2 = false;
				bool found3 = false;

				int k = _Scale;

				for (int x = 0; x < 100; x++)
				{
					if (!found0 && x < k)
					{
						uv0 += rightTop;
						dst0 = distance(i.texcoord, uv0);
						depth0 = SAMPLE_DEPTH_TEXTURE(_CameraDepthTexture, sampler_CameraDepthTexture, uv0).r;
						diff0 = abs(depthOrigin - depth0) * 100;
						found0 = diff0 > _DepthThreshold;
					}


					if (!found1 && x < k)
					{
						uv1 += rightBottom;
						dst1 = distance(i.texcoord, uv1);
						depth1 = SAMPLE_DEPTH_TEXTURE(_CameraDepthTexture, sampler_CameraDepthTexture, uv1).r;
						diff1 = abs(depthOrigin - depth1) * 100;
						found1 = diff1 > _DepthThreshold;
					}

					if (!found2 && x < k)
					{
						uv2 += leftBottom;
						dst2 = distance(i.texcoord, uv2);
						depth2 = SAMPLE_DEPTH_TEXTURE(_CameraDepthTexture, sampler_CameraDepthTexture, uv2).r;
						diff2 = abs(depthOrigin - depth2) * 100;
						found2 = diff2 > _DepthThreshold;
					}

					if (!found3 && x < k)
					{
						uv3 += leftTop;			
						dst3 = distance(i.texcoord, uv3);
						depth3 = SAMPLE_DEPTH_TEXTURE(_CameraDepthTexture, sampler_CameraDepthTexture, uv3).r;
						diff3 = abs(depthOrigin - depth3) * 100;
						found3 = diff3 > _DepthThreshold;
					}

					if (found0 && found1 && found2 && found3)
						break;
				}

				float output0 = 0;
				float output1 = 0;
				float output2 = 0;
				float output3 = 0;

				if(found0 && depthOrigin > 0)
					output0 = 1.0 - (dst0 * _Intensity);

				if(found1 && depthOrigin > 0)
					output1 = 1.0 - (dst1 * _Intensity);

				if(found2 && depthOrigin > 0)
					output2 = 1.0 - (dst2 * _Intensity);

				if(found3 && depthOrigin > 0)
					output3 = 1.0 - (dst3 * _Intensity);
				
				
				

				//float outputDst = 

				//float output = 0;
				//return color - max(max(output0, output1), max(output2, output3));
				return depthOrigin;
				//return color - (output0 + output1 + output2 + output3 / k * 4);
			}


			// float4 Frag(VaryingsDefault i) : SV_Target
			// {
				// 	float4 color = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, i.texcoord);

				// 	float halfScaleFloor = floor(_Scale * 0.5);
				// 	float halfScaleCeil = ceil(_Scale * 0.5);

				// 	float2 bottomLeftUV = i.texcoord - float2(_MainTex_TexelSize.x, _MainTex_TexelSize.y) * halfScaleFloor;
				// 	float2 topRightUV = i.texcoord + float2(_MainTex_TexelSize.x, _MainTex_TexelSize.y) * halfScaleCeil;  
				// 	float2 bottomRightUV = i.texcoord + float2(_MainTex_TexelSize.x * halfScaleCeil, -_MainTex_TexelSize.y * halfScaleFloor);
				// 	float2 topLeftUV = i.texcoord + float2(-_MainTex_TexelSize.x * halfScaleFloor, _MainTex_TexelSize.y * halfScaleCeil);

				// 	float ceterDepth = SAMPLE_DEPTH_TEXTURE(_CameraDepthTexture, sampler_CameraDepthTexture, i.texcoord).r;
				// 	float depth0 = SAMPLE_DEPTH_TEXTURE(_CameraDepthTexture, sampler_CameraDepthTexture, bottomLeftUV).r;
				// 	float depth1 = SAMPLE_DEPTH_TEXTURE(_CameraDepthTexture, sampler_CameraDepthTexture, topRightUV).r;
				// 	float depth2 = SAMPLE_DEPTH_TEXTURE(_CameraDepthTexture, sampler_CameraDepthTexture, bottomRightUV).r;
				// 	float depth3 = SAMPLE_DEPTH_TEXTURE(_CameraDepthTexture, sampler_CameraDepthTexture, topLeftUV).r;

				// 	float3 normal0 = SAMPLE_TEXTURE2D(_CameraNormalsTexture, sampler_CameraNormalsTexture, bottomLeftUV).rgb;
				// 	float3 normal1 = SAMPLE_TEXTURE2D(_CameraNormalsTexture, sampler_CameraNormalsTexture, topRightUV).rgb;
				// 	float3 normal2 = SAMPLE_TEXTURE2D(_CameraNormalsTexture, sampler_CameraNormalsTexture, bottomRightUV).rgb;
				// 	float3 normal3 = SAMPLE_TEXTURE2D(_CameraNormalsTexture, sampler_CameraNormalsTexture, topLeftUV).rgb;


				// 	float depthFiniteDifference0 = depth1 - depth0;
				// 	float depthFiniteDifference1 = depth3 - depth2;

				// 	// float depthFiniteDifference0 = ceterDepth - depth0;
				// 	// float depthFiniteDifference1 = ceterDepth - depth1;
				// 	// float depthFiniteDifference2 = ceterDepth - depth2;
				// 	// float depthFiniteDifference3 = ceterDepth - depth3;
				
				// 	//return SAMPLE_DEPTH_TEXTURE(_CameraDepthTexture, sampler_CameraDepthTexture, i.texcoord).r;

				// 	float edgeDepth = sqrt(pow(depthFiniteDifference0, 2) + pow(depthFiniteDifference1, 2)) * 100;
				// 	float depthThreshold = _DepthThreshold * depth0;
				// 	edgeDepth = edgeDepth > depthThreshold ? 1.0f : 0.0f;

				
				

				

				// 	return color + edgeDepth;
			// }
			ENDHLSL
		}
	}
}