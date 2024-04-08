Shader "Tutorial/NewSurfaceShader" {
    Properties{
        _Color("Color", Color) = (1, 1, 1, 1)
        _Glossiness("Smoothness", Range(0, 1)) = 0.5
        _Metallic("Metallic", Range(0, 1)) = 0
        _Emission("Emission", Color) = (0, 0, 0, 1) // Added emissive color property
        _OutlineColor("Outline Color", Color) = (0, 0, 0, 1)
        _OutlineWidth("Outline Width", Range(0, 5)) = 0.03
    }

        SubShader{
            Tags {
                "RenderType" = "Opaque"
            }

            LOD 200

            CGPROGRAM
            #pragma surface surf Standard fullforwardshadows

            fixed4 _Color;
            half _Glossiness;
            half _Metallic;
            fixed4 _Emission; // Emissive color property

            struct Input {
                float4 vertex : POSITION;
                float3 normal : NORMAL;
                float4 texcoord : TEXCOORD0; // Optional texture coordinates
            };

            void surf(Input IN, inout SurfaceOutputStandard o) {
                // Material properties
                o.Albedo = _Color.rgb;
                o.Smoothness = _Glossiness;
                o.Metallic = _Metallic;
                o.Alpha = _Color.a;

                // Emissive color
                o.Emission = _Emission;

                // Optional texture support (uncomment if needed)
                // tex2D _MainTex ("Texture", "white") = "white" {} // Replace "white" with your texture
                // o.Albedo *= tex2D (_MainTex, IN.texcoord.xy);
            }
            ENDCG

            Pass {
                Cull Front

                CGPROGRAM
                #pragma vertex vert
                #pragma fragment frag

                fixed4 _OutlineColor;
                half _OutlineWidth;

                struct appdata {
                    float4 vertex : POSITION;
                    float3 normal : NORMAL;
                    float4 texcoord : TEXCOORD0; // Optional texture coordinates
                };

                struct v2f {
                    float4 pos : SV_POSITION;
                    float3 worldNormal : TEXCOORD0; // Optional for lighting calculations
                };

                v2f vert(appdata v) {
                    v2f o;
                    o.pos = UnityObjectToClipPos(v.vertex);
                    o.worldNormal = mul((float3x3)UNITY_MATRIX_VP, mul((float3x3)UNITY_MATRIX_M, v.normal));

                    // Outline effect
                    float3 clipNormal = o.worldNormal;
                    float2 offset = normalize(clipNormal.xy) / _ScreenParams.xy * _OutlineWidth * o.pos.w * 2;
                    o.pos.xy += offset;

                    return o;
                }

                half4 frag(v2f i) : SV_TARGET {
                    // Outline color (consider using a separate pass for better performance)
                    return _OutlineColor;

                // Optional lighting calculations (uncomment if needed)
                // half3 worldLightDir = normalize(_WorldLight01.xyz);
                // half diffuse = dot(i.worldNormal, worldLightDir);
                // half specular = ... // Implement specular lighting calculation

                // return half4(diffuse * _Color.rgb, 1.0); // Combine diffuse and outline
            }
            ENDCG
        }
    }
}