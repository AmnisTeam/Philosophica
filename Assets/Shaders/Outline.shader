Shader "Custom/Outline"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 100

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            // make fog work
            #pragma multi_compile_fog

            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                UNITY_FOG_COORDS(1)
                float4 vertex : SV_POSITION;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;
            float4 _MainTex_TexelSize;

            half3 Sample(float2 uv) 
            {
			    return tex2D(_MainTex, uv).rgb;
		    }

            half3 SampleBox (float2 uv, float delta) 
            {
                float4 o = _MainTex_TexelSize.xyxy * float2(-delta, delta).xxyy;
                half3 s =
                    Sample(uv + o.xy) + Sample(uv + o.zy) +
                    Sample(uv + o.xw) + Sample(uv + o.zw);
                return s * 0.25f;
		    }

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

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                UNITY_TRANSFER_FOG(o,o.vertex);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                fixed4 col = tex2D(_MainTex, i.uv);
                UNITY_APPLY_FOG(i.fogCoord, col);
                return get_outlines(i.uv, _MainTex, 15);
            }
            ENDCG
        }
    }
}
