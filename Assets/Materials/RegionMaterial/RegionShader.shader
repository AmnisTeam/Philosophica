// Shader "Unlit/RegionShader"
// {
//     Properties
//     {
//         _MainTex ("Texture", 2D) = "white" {}
//         _RegionColor ("Region color", Color) = (1,1,1,1)
//         _ShadowColor ("Shadow color", Color) = (0.5, 0.5, 0.5, 0.5)
//     }
//     SubShader
//     {
//         Tags { "RenderType"="Opaque" }
//         LOD 100

//         Pass
//         {

//             CGPROGRAM


//             #pragma vertex vert
//             #pragma fragment frag
            
//             #pragma multi_compile_fog

//             #include "UnityCG.cginc"
            

//             struct appdata
//             {
//                 float4 vertex : POSITION;
//                 float4 color : COLOR;
//                 float2 uv : TEXCOORD0;
//             };

//             struct v2f
//             {
//                 float2 uv : TEXCOORD0;
//                 UNITY_FOG_COORDS(1)
//                 float4 vertex : SV_POSITION;
//                 float4 color : COLOR;
//                 float4 worldPos : COLOR2;
//             };

//             sampler2D _MainTex;
//             float4 _MainTex_ST;
//             float4 _RegionColor;
//             float4 _ShadowColor;

//             v2f vert (appdata v)
//             {
//                 v2f o;
//                 o.vertex = UnityObjectToClipPos(v.vertex);
//                 o.worldPos = mul(unity_ObjectToWorld, v.vertex);
//                 o.uv = TRANSFORM_TEX(v.uv, _MainTex);
//                 o.color = v.color;
//                 UNITY_TRANSFER_FOG(o,o.vertex);
//                 return o;
//             }

//             fixed4 frag (v2f i) : SV_Target
//             {
//                 fixed4 col = tex2D(_MainTex, i.uv);
//                 UNITY_APPLY_FOG(i.fogCoord, col);


//                 float3 vec1 = normalize(i.worldPos.xyz - _WorldSpaceCameraPos);
//                 //float3 vec1 = normalize(i.worldPos.xyz - _WorldSpaceCameraPos);
//                 float3 vec2 = normalize(float3(0, 0, 0) - i.worldPos.xyz);

//                 float3 lightPos = float3(0, 0, 0);
//                 float3 posOnSurface = i.worldPos.xyz;

//                 float3 rayOrigin = _WorldSpaceCameraPos;
//                 float3 rayDirection = normalize(posOnSurface - _WorldSpaceCameraPos);

//                 float3 cVec = lightPos - posOnSurface;

//                 float c = distance(posOnSurface, lightPos);
//                 float a = c * dot(normalize(cVec), rayDirection);
//                 float b = c * c - a * a;


//                 float3 closestPointToLightOnRay = b * rayDirection;

//                 //float dst = distance(i.worldPos, float3(0, 0, 0));
//                 float dst = distance(closestPointToLightOnRay, lightPos);
//                 //return float4(0.98, 0.78431, 0.086, 1) * pow((1 - dst), 1);
//                 return float4(251.0f / 255.0f, 145.0f / 255.0f, 22.0f / 255.0f, 1) * pow((1 - dst), 1) + float4(252.0f / 255.0f, 208.0f/ 255.0f, 85.0f/ 255.0f, 1) * pow((1 - dst), 5) * 0.4f;
//                 //return 1;
//             }
//             ENDCG
//         }
//     }
// }



Shader "Unlit/RegionShader"
{
    Properties
    {
        _RegionColor ("Region color", Color) = (1,0,0,1)
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
            #pragma multi_compile_fog

            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float4 color : COLOR;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
                float4 color : COLOR;
            };

            float4 _RegionColor;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.color = v.color;
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                //return _RegionColor;
                return _RegionColor;
            }
            ENDCG
        }
    }
}
