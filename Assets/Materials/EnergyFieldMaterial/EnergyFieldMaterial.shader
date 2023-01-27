Shader "Unlit/EnergyFieldMaterial"
{
    Properties
    {
        _Albedo ("Albedo", 2D) = "white" {}
        _FieldColor("Field color", Color) = (0, 0.909, 0.988, 1)
    }
    SubShader
    {
        Tags {
            "RenderType"="Transparent"
            "Queue"="Transparent"
        }
        LOD 100

        ZWrite Off
        Blend SrcAlpha OneMinusSrcAlpha
        
        Pass
        {
            CGPROGRAM
            #pragma vertex vert alpha
            #pragma fragment frag alpha
            // make fog work
            #pragma multi_compile_fog

            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float4 normal : NORMAL;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                UNITY_FOG_COORDS(1)
                float4 vertex : SV_POSITION;
                float4 worldPos : COLOR0;
                float3 normal : NORMAL;
            };

            sampler2D _Albedo;
            float4 _MainTex_ST;
            float4 _FieldColor;


            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                o.worldPos = mul(unity_ObjectToWorld, v.vertex);
                o.normal = v.normal;
                UNITY_TRANSFER_FOG(o,o.vertex);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                UNITY_APPLY_FOG(i.fogCoord, col);
                float3 viewDir = normalize(_WorldSpaceCameraPos - i.worldPos.xyz);
                float normalViewDot = dot(i.normal.xyz, viewDir);
                float lerpCoof = pow(1 - normalViewDot, 1.9f);
                float4 albedo = tex2D(_Albedo, i.uv);
                return lerp(float4(1, 1, 1, 1), float4(0, 0, 0, 0), 1 - lerpCoof) * albedo * _FieldColor;
            }
            ENDCG
        }
    }
}
