Shader "Unlit/SpaceBackground"
{
    Properties
    {
        _BackgroundColor ("Background color", Color) = (0,0,0,1)
        _Offset ("Offset", Vector) = (0,0,0,1)
        _CellsCount ("Cells count", Range(0,100)) = 10
        _StarsRadius ("Stars radius", Range(0,0.3)) = 0.02
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

            

            float4 _BackgroundColor;
            float4 _Offset;
            float _StarsRadius;
            float _CellsCount;

            v2f vert (appdata v)
            {
                v2f o;
                o.uv = v.uv;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.color = v.color;
                return o;
            }

            float2 n22 (float2 p)
            {
                float3 a = frac(p.xyx * float3(123.34, 234.34, 345.65));
                a += dot(a, a + 34.45);
                return frac(float2(a.x * a.y, a.y * a.z));
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
                return color;
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

            fixed4 frag (v2f i) : SV_Target
            {
                float m = n22(i.uv).x;
                float4 color = _BackgroundColor;
                float2 uv = float2(1 - i.uv.x, 1 - i.uv.y);

                color += draw_stars_layer(uv, _Offset, 0.01, _CellsCount, _StarsRadius, 0);
                color += draw_stars_layer(uv, _Offset, 0.003, _CellsCount, _StarsRadius, 1) * 0.5f;
                color += draw_stars_layer(uv, _Offset, 0.001, _CellsCount, _StarsRadius, 2) * 0.2f;
                
                color += perlin_noise_extended(uv, _Offset, 0.003, 10) * 
                         perlin_noise_extended(uv, _Offset, 0.003, 7, 5) * 
                         float4(0.5, 0.5, 1, 1) * 0.3f;
                return color;
            }         
            ENDCG
        }
    }
}