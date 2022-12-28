Shader "Unlit/BackgroundGridShader"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _ResolutionX ("Resolution x", Float) = 1920
        _ResolutionY ("Resolution y", Float) = 1080
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
            float _ResolutionX;
            float _ResolutionY;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                UNITY_TRANSFER_FOG(o,o.vertex);
                return o;
            }

            float4 GridTest(float2 uv)
            {
                return float4(0, 0, 0, 1);
            }

            fixed4 frag (v2f i) : SV_Target
            {
                float aspect = _ResolutionY / _ResolutionX;
                float2 uv = float2(i.uv.x * aspect, i.uv.y);
                fixed4 col = tex2D(_MainTex, i.uv);
                UNITY_APPLY_FOG(i.fogCoord, col);
                return GridTest(uv);
                //return float4(uv.x, uv.y, 0, 1);
                //return float4(i.uv.x, i.uv.y, 0, 1);
            }
            ENDCG
        }
    }
}
