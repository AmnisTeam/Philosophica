Shader "Unlit/SpaceBackground"
{
    Properties
    {
        _BackgroundColor ("Background color", Color) = (0,0,0,1)
        _Offset ("Offset", Vector) = (0,0,0,1)
        _GridSize ("Grid size", Range(0,0.3)) = 0.2
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


            fixed4 frag (v2f i) : SV_Target
            {
                float m = n22(i.uv).x;
                float4 color = _BackgroundColor;
                float2 uv = float2(1 - i.uv.x, 1 - i.uv.y);

                color += DrawLayer(uv, 1 , 0.01);
                color += (DrawLayer(uv, 5, 0.003) * 0.5f);
                color += (DrawLayer(uv, 5, 0.001) * 0.2f);
                
                return color;
            }         
            ENDCG
        }
    }
}