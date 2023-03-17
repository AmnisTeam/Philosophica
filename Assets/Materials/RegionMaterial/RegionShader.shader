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
        [HDR]
        _RegionColor ("Region color", Color) = (1,0,0,1)
        _DrawOnlyColor ("Draw only color", Range(0,1)) = 0
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
                float2 screenPos : TEXCOORD1;
                float4 vertex : SV_POSITION;
                float4 color : COLOR;              
            };
       
            float4 _RegionColor;
            float _DrawOnlyColor;

            v2f vert (appdata v)
            {
                v2f o;
                o.uv = v.uv;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.color = v.color;      
                o.screenPos = ComputeScreenPos(o.vertex);  
                return o;
            }

            float2 n22 (float2 p)
            {
                float3 a = frac(p.xyx * float3(123.34, 234.34, 345.65));
                a += dot(a, a + 34.45);
                return frac(float2(a.x * a.y, a.y * a.z));
            }

            float2 get_gradient(float2 pos)
            {
                float twoPi = 6.283185;
                float angle = n22(pos).x * twoPi;
                return float2(cos(angle), sin(angle));
            }

            float4 perlin_noise(float2 uv, float cells_count)
            {
                float2 pos_in_grid = uv * cells_count;
                float2 cell_pos_in_grid =  floor(pos_in_grid);
                float2 local_pos_in_cell = (pos_in_grid - cell_pos_in_grid);
                float2 blend = local_pos_in_cell * local_pos_in_cell * (3.0f - 2.0f * local_pos_in_cell);
                
                float2 left_top = cell_pos_in_grid + float2(0, 1);
                float2 right_top = cell_pos_in_grid + float2(1, 1);
                float2 left_bottom = cell_pos_in_grid + float2(0, 0);
                float2 right_bottom = cell_pos_in_grid + float2(1, 0);
                
                float left_top_dot = dot(pos_in_grid - left_top, get_gradient(left_top));
                float right_top_dot = dot(pos_in_grid - right_top,  get_gradient(right_top));
                float left_bottom_dot = dot(pos_in_grid - left_bottom, get_gradient(left_bottom));
                float right_bottom_dot = dot(pos_in_grid - right_bottom, get_gradient(right_bottom));
                
                float noise_value = lerp(
                                        lerp(left_bottom_dot, right_bottom_dot, blend.x), 
                                        lerp(left_top_dot, right_top_dot, blend.x), 
                                        blend.y);
            
                
                return (0.5 + 0.5 * (noise_value / 0.7));
            }

            float perlin_noise_extended(float2 uv, float2 offset, float speed, float cells_count, float sharpness = 1) 
            {
                return pow(perlin_noise(uv + offset * speed, cells_count), sharpness);
            }

            float4 draw_stars_layer(float2 uv, float2 offset, float speed, float cells_count, float stars_radius, float seed) 
            {
                uv += offset * speed;
                float4 color = 0;
                float2 cell_pos_in_grid = floor(uv * cells_count);
                float2 pos_in_grid = uv * cells_count;
                float2 local_pos_in_cell = pos_in_grid - cell_pos_in_grid;
                float2 star_local_pos_in_cell = n22(cell_pos_in_grid + seed);
                float area_for_star_in_cell = (1 - stars_radius * 2);
                float2 star_pos_in_grid = cell_pos_in_grid + stars_radius + star_local_pos_in_cell * area_for_star_in_cell;
                float dist = distance(pos_in_grid, star_pos_in_grid);
                if (dist <= stars_radius)
                    color = float4(1, 1, 1, 1);

                //color = float4(1, 1, 1, 1) / (dist / stars_radius);
                return color;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                float2 uv = float2(1 - i.uv.x, 1 - i.uv.y);
                float4 color = 0;

                if (_DrawOnlyColor >= 0.5)
                {
                    color = _RegionColor;
                }
                else
                {
                    float val = perlin_noise_extended(uv, 0, 0.003, 10, 1);
                    if (val > 0.5f)
                        val *= 20;
                    else
                        val = 0;

                    color += draw_stars_layer(uv, 0, 0.005, 4, 0.02, 0);
                    color += draw_stars_layer(uv, 0, 0.005, 5, 0.02, 1) * 0.8;
                    color += draw_stars_layer(uv, 0, 0.005, 5, 0.02, 2) * 0.5;

                    color *= _RegionColor * 4;
                }

                
                
                return color;
                //return _RegionColor;
            }
            ENDCG
        }
    }
}
