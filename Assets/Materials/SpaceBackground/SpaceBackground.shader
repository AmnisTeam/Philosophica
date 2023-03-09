Shader "Unlit/SpaceBackground"
{
    Properties
    {
        _BackgroundColor ("Background color", Color) = (0,0,0,1)
        _Offset ("Offset", Vector) = (0,0,0,1)
        _GridSize ("Grid size", Range(0,1)) = 0.2
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
            float _GridSize;

            float2 n22 (float2 p)
            {
                float3 a = frac(p.xyx * float3(123.34, 234.34, 345.65));
                a += dot(a, a + 34.45);
                return frac(float2(a.x * a.y, a.y * a.z));
            }

            v2f vert (appdata v)
            {
                v2f o;
                o.uv = v.uv;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.color = v.color;
                return o;
            }


            float4 DrawLayer(float2 uv, float seed, float speed) 
            {
                uv += _Offset * speed;

                float4 color = 0;
                float cellsCount = 1 / _GridSize;

                float2 gridPos = float2((int)(uv.x / _GridSize),(int)(uv.y / _GridSize) );

                float2 localPos = n22(float2(gridPos.x + seed, gridPos.y + seed));

                float areaSize = _GridSize - _StarsRadius * 2;

                float2 pointPos = float2(gridPos.x, gridPos.y) * _GridSize + _StarsRadius + areaSize * localPos;

                float dist = distance(pointPos, uv);

                if (dist < _StarsRadius)
                    color = 1;
                return color;
            }

            float plane(float x, float y, float a, float b, float c, float centerX, float centerY) 
            {
                return (x - centerX) * a + (y - centerY) * b + c;
            }

            float2 get_vec_by_angle(float angle)
            {
                return float2(cos(angle), sin(angle));
            }

            float rand1(float2 p)
            {
                return sin(p.x * p.y);
            }

            float4 perlin_noise(v2f i)
            {
                float4 color = 0;
                float2 uv = float2(1 - i.uv.x, 1 - i.uv.y);
                //float2 uv = float2(1, 1);
                float PI = 3.14159265359f;

                int2 cell_pos_in_grid = int2((int)(uv.x / _GridSize), (int)(uv.y / _GridSize));
                float2 cell_pos = cell_pos_in_grid * _GridSize;
                float2 local_pos_in_cell = (uv - cell_pos) / _GridSize;
                

                int2 left_top_in_grid = cell_pos_in_grid + int2(0, 1);
                int2 right_top_in_grid = cell_pos_in_grid + int2(1, 1);
                int2 left_bottom_in_grid = cell_pos_in_grid + int2(0, 0);
                int2 right_bottom_in_grid = cell_pos_in_grid + int2(1, 0);
                

                float2 left_top = left_top_in_grid * _GridSize;
                float2 right_top = right_top_in_grid * _GridSize;
                float2 left_bottom = left_bottom_in_grid * _GridSize;
                float2 right_bottom = right_bottom_in_grid * _GridSize;
                
                // float2 left_top_vec = get_vec_by_angle(PI / 4);
                // float2 right_top_vec = get_vec_by_angle((5 * PI) / 4);
                // float2 left_bottom_vec = get_vec_by_angle((2 * PI) / 3);
                // float2 right_bottom_vec = get_vec_by_angle((3 * PI) / 2);

                float2 left_top_vec = get_vec_by_angle(n22(left_top_in_grid) * 2 * PI);
                float2 right_top_vec = get_vec_by_angle(n22(right_top_in_grid) * 2 * PI);
                float2 left_bottom_vec = get_vec_by_angle(n22(left_bottom_in_grid) * 2 * PI);
                float2 right_bottom_vec = get_vec_by_angle(n22(right_bottom_in_grid) * 2 * PI);

                float2 left_top_delta = (uv - left_top) / _GridSize;
                float2 right_top_delta = (uv - right_top) / _GridSize;
                float2 left_bottom_delta = (uv - left_bottom) / _GridSize;
                float2 right_bottom_delta = (uv - right_bottom) / _GridSize;

                float left_top_dot = dot(left_top_vec, left_top_delta);
                float right_top_dot = dot(right_top_vec, right_top_delta);
                float left_bottom_dot = dot(left_bottom_vec, left_bottom_delta);
                float right_bottom_dot = dot(right_bottom_vec, right_bottom_delta);

                float top_dot = lerp(left_top_dot, right_top_dot, local_pos_in_cell.x);
                float bottom_dot = lerp(left_bottom_dot, right_bottom_dot, local_pos_in_cell.x);
                float total_dot = lerp(bottom_dot, top_dot, local_pos_in_cell.y);

                //return total_dot; 
                return total_dot + float4(1, 0, 0, 0) * local_pos_in_cell.x * 0.2 + float4(1, 0, 0, 0) * local_pos_in_cell.y * 0.2; 
            }

            fixed4 frag (v2f i) : SV_Target
            {
                float m = n22(i.uv).x;
                float4 color = _BackgroundColor;
                float2 uv = float2(1 - i.uv.x, 1 - i.uv.y);

                color += DrawLayer(uv, 1 , 0.01);
                color += (DrawLayer(uv, 5, 0.003) * 0.5f);
                color += (DrawLayer(uv, 5, 0.001) * 0.2f);

                float cellsCount = 1 / _GridSize;
                float2 gridPos = float2(floor(uv.x / _GridSize),floor(uv.y / _GridSize) );
                float2 localPos = (uv - gridPos * _GridSize) / _GridSize;

                // float2 lefttop = float2(gridPos.x, gridPos.y) * _GridSize;
                // float2 righttop = float2(gridPos.x, gridPos.y) * _GridSize + float2(_GridSize, 0);
                // float2 rightbottom = float2(gridPos.x, gridPos.y) * _GridSize + _GridSize;
                // float2 leftbottom = float2(gridPos.x, gridPos.y) * _GridSize + float2(0, _GridSize);

                float2 lefttop = (gridPos + float2(0, 1)) * _GridSize;
                float2 righttop = (gridPos + float2(1, 1)) * _GridSize;
                float2 rightbottom = (gridPos + float2(1, 0)) * _GridSize;
                float2 leftbottom = (gridPos + float2(0, 0)) * _GridSize;

                float PI = 3.14f;

                float lefttopRandom = n22(lefttop) * 2 * PI;
                float righttopRandom = n22(righttop) * 2 * PI;
                float rightbottomRandom = n22(rightbottom) * 2 * PI;
                float leftbottomRandom = n22(leftbottom) * 2 * PI;

                float2 lefttopRandVec = float2(cos(lefttopRandom), sin(lefttopRandom));
                float2 righttopRandVec = float2(cos(righttopRandom), sin(righttopRandom));
                float2 rightbottomRandVec = float2(cos(rightbottomRandom), sin(rightbottomRandom));
                float2 leftbottomRandVec = float2(cos(leftbottomRandom), sin(leftbottomRandom));

                float lefttopPlane = dot(lefttopRandVec, (uv - lefttop) / _GridSize);
                float righttopPlane = dot(righttopRandVec, (uv - righttop) / _GridSize);
                float rightbottomPlane = dot(rightbottomRandVec, (uv - rightbottom) / _GridSize);
                float leftbottomPlane = dot(leftbottomRandVec, (uv - leftbottom) / _GridSize);

                // float lefttopPlaneValue = plane(uv.x, uv.y, lefttopRand.x, lefttopRand.y, 0, lefttop.x, lefttop.y);
                // float righttopPlaneValue = plane(uv.x, uv.y, righttopRand.x, righttopRand.y, 0, righttop.x, righttop.y);
                // float rightbottomPlaneValue = plane(uv.x, uv.y, rightbottomRand.x, rightbottomRand.y, 0, rightbottom.x, rightbottom.y);
                // float leftbottomPlaneValue = plane(uv.x, uv.y, leftbottomRand.x, leftbottomRand.y, 0, leftbottom.x, leftbottom.y);
                

                float topT = localPos.x;
                float topValue = lerp(lefttopPlane, righttopPlane, topT);

                float bottomT = localPos.x;
                float bottomValue = lerp(leftbottomPlane, rightbottomPlane, bottomT);

                float resultT = localPos.y;
                float resultValue = lerp(bottomValue, topValue, resultT);
                
                return perlin_noise(i);
            }         
            ENDCG
        }
    }
}