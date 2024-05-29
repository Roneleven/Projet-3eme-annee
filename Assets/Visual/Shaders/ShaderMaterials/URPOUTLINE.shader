Shader "Unlit/URPOUTLINE"
{
    Properties
    {
        _MainTex("Texture", 2D) = "white" {}
        _Color("Main Color", Color) = (1,1,1,1)
        _OutlineColor("Outline Color", Color) = (0,0,0,1)
        _OutlineWidth("Outline Width", Range(.002, 0.03)) = .005
    }

        SubShader
        {
            Tags { "RenderType" = "Opaque" }

            // Render the object normally
            Pass
            {
                CGPROGRAM
                #pragma vertex vert
                #pragma fragment frag
                #include "UnityCG.cginc"

                struct appdata
                {
                    float4 vertex : POSITION;
                    float3 normal : NORMAL;
                };

                struct v2f
                {
                    float4 pos : SV_POSITION;
                    float4 color : COLOR;
                };

                float _OutlineWidth;
                float4 _OutlineColor;

                v2f vert(appdata v)
                {
                    // Extrude the vertex positions along their normals
                    v2f o;
                    o.pos = UnityObjectToClipPos(v.vertex + v.normal * _OutlineWidth);
                    o.color = _OutlineColor;
                    return o;
                }

                half4 frag(v2f i) : SV_Target
                {
                    return i.color;
                }
                ENDCG
            }

            // Render the object with its normal appearance
            Pass
            {
                CGPROGRAM
                #pragma vertex vert
                #pragma fragment frag
                #include "UnityCG.cginc"

                struct appdata
                {
                    float4 vertex : POSITION;
                    float2 uv : TEXCOORD0;
                };

                struct v2f
                {
                    float4 pos : SV_POSITION;
                    float2 uv : TEXCOORD0;
                };

                sampler2D _MainTex;
                float4 _Color;

                v2f vert(appdata v)
                {
                    v2f o;
                    o.pos = UnityObjectToClipPos(v.vertex);
                    o.uv = v.uv;
                    return o;
                }

                half4 frag(v2f i) : SV_Target
                {
                    half4 col = tex2D(_MainTex, i.uv) * _Color;
                    return col;
                }
                ENDCG
            }
        }

            FallBack "Diffuse"
}
