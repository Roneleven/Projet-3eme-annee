// Made with Amplify Shader Editor v1.9.2
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "Outline"
{
	Properties
	{
		_Color0("Color 0", Color) = (1,0,0,0)
		_ScaleBias("Scale Bias", Float) = 0
		[HideInInspector] _texcoord( "", 2D ) = "white" {}

	}

	SubShader
	{
		LOD 0

		Cull Off
		ZWrite Off
		ZTest Always
		
		Pass
		{
			CGPROGRAM

			

			#pragma vertex Vert
			#pragma fragment Frag
			#pragma target 3.0

			#include "UnityCG.cginc"
			#define ASE_NEEDS_FRAG_SCREEN_POSITION_NORMALIZED

		
			struct ASEAttributesDefault
			{
				float3 vertex : POSITION;
				float2 texcoord : TEXCOORD0;
				float3 ase_normal : NORMAL;
			};

			struct ASEVaryingsDefault
			{
				float4 vertex : SV_POSITION;
				float2 texcoord : TEXCOORD0;
				float2 texcoordStereo : TEXCOORD1;
			#if STEREO_INSTANCING_ENABLED
				uint stereoTargetEyeIndex : SV_RenderTargetArrayIndex;
			#endif
				float4 ase_texcoord2 : TEXCOORD2;
				float4 ase_texcoord3 : TEXCOORD3;
			};

			uniform sampler2D _MainTex;
			uniform half4 _MainTex_TexelSize;
			uniform half4 _MainTex_ST;
			
			uniform float _ScaleBias;
			uniform float4 _Color0;
			UNITY_DECLARE_DEPTH_TEXTURE( _CameraDepthTexture );
			uniform float4 _CameraDepthTexture_TexelSize;


			
			float2 TransformTriangleVertexToUV (float2 vertex)
			{
				float2 uv = (vertex + 1.0) * 0.5;
				return uv;
			}

			ASEVaryingsDefault Vert( ASEAttributesDefault v  )
			{
				ASEVaryingsDefault o;
				o.vertex = float4(v.vertex.xy, 0.0, 1.0);
				o.texcoord = TransformTriangleVertexToUV (v.vertex.xy);
#if UNITY_UV_STARTS_AT_TOP
				o.texcoord = o.texcoord * float2(1.0, -1.0) + float2(0.0, 1.0);
#endif
				o.texcoordStereo = TransformStereoScreenSpaceTex (o.texcoord, 1.0);

				v.texcoord = o.texcoordStereo;
				float4 ase_ppsScreenPosVertexNorm = float4(o.texcoordStereo,0,1);

				float3 ase_worldPos = mul(unity_ObjectToWorld, float4( (v.vertex).xyz, 1 )).xyz;
				o.ase_texcoord2.xyz = ase_worldPos;
				float3 ase_worldNormal = UnityObjectToWorldNormal(v.ase_normal);
				o.ase_texcoord3.xyz = ase_worldNormal;
				
				
				//setting value to unused interpolator channels and avoid initialization warnings
				o.ase_texcoord2.w = 0;
				o.ase_texcoord3.w = 0;

				return o;
			}

			float4 Frag (ASEVaryingsDefault i  ) : SV_Target
			{
				float4 ase_ppsScreenPosFragNorm = float4(i.texcoordStereo,0,1);

				float2 uv_MainTex = i.texcoord.xy * _MainTex_ST.xy + _MainTex_ST.zw;
				float3 ase_worldPos = i.ase_texcoord2.xyz;
				float3 ase_worldViewDir = UnityWorldSpaceViewDir(ase_worldPos);
				ase_worldViewDir = normalize(ase_worldViewDir);
				float3 ase_worldNormal = i.ase_texcoord3.xyz;
				float fresnelNdotV29 = dot( ase_worldNormal, ase_worldViewDir );
				float fresnelNode29 = ( 0.0 + _ScaleBias * pow( 1.0 - fresnelNdotV29, 1.0 ) );
				float2 appendResult17 = (float2(( ase_ppsScreenPosFragNorm.x - _MainTex_TexelSize.x ) , ase_ppsScreenPosFragNorm.y));
				float eyeDepth16 = LinearEyeDepth(SAMPLE_DEPTH_TEXTURE( _CameraDepthTexture, float4( appendResult17, 0.0 , 0.0 ).xy ));
				float2 appendResult23 = (float2(( ase_ppsScreenPosFragNorm.x + _MainTex_TexelSize.x ) , ase_ppsScreenPosFragNorm.y));
				float eyeDepth19 = LinearEyeDepth(SAMPLE_DEPTH_TEXTURE( _CameraDepthTexture, float4( appendResult23, 0.0 , 0.0 ).xy ));
				float2 appendResult9 = (float2(ase_ppsScreenPosFragNorm.x , ( ase_ppsScreenPosFragNorm.y + _MainTex_TexelSize.y )));
				float eyeDepth7 = LinearEyeDepth(SAMPLE_DEPTH_TEXTURE( _CameraDepthTexture, float4( appendResult9, 0.0 , 0.0 ).xy ));
				float2 appendResult12 = (float2(ase_ppsScreenPosFragNorm.x , ( ase_ppsScreenPosFragNorm.y - _MainTex_TexelSize.y )));
				float eyeDepth15 = LinearEyeDepth(SAMPLE_DEPTH_TEXTURE( _CameraDepthTexture, float4( appendResult12, 0.0 , 0.0 ).xy ));
				float4 lerpResult26 = lerp( float4( 0,0,0,0 ) , _Color0 , ( distance( eyeDepth16 , eyeDepth19 ) + distance( eyeDepth7 , eyeDepth15 ) ));
				float4 temp_cast_4 = (1.689049).xxxx;
				float4 clampResult38 = clamp( ( ( fresnelNode29 - 0.0 ) * lerpResult26 ) , float4( 0,0,0,0 ) , temp_cast_4 );
				

				float4 color = (tex2D( _MainTex, uv_MainTex )*1.0 + clampResult38);
				
				return color;
			}
			ENDCG
		}
	}
	CustomEditor "ASEMaterialInspector"
	
	Fallback Off
}
/*ASEBEGIN
Version=19200
Node;AmplifyShaderEditor.StickyNoteNode;1;-2560.457,-639.5518;Inherit;False;150;100;New Note;;1,1,1,1;Sur l'axe Y ...;0;0
Node;AmplifyShaderEditor.StickyNoteNode;2;-2062.789,-366.4786;Inherit;False;268.084;100;New Note;;1,1,1,1;...On détecte les pixels voisins...;0;0
Node;AmplifyShaderEditor.StickyNoteNode;3;-1566.714,-360.0743;Inherit;False;253.291;100;New Note;;1,1,1,1;...Et  la position selon la caméra ;0;0
Node;AmplifyShaderEditor.StickyNoteNode;4;-769.4465,99.68674;Inherit;False;263.3795;100;New Note;;1,1,1,1;On permet de choisir la couleur de l'outline;0;0
Node;AmplifyShaderEditor.StickyNoteNode;5;-876.8745,-400.1373;Inherit;False;356.9177;121.2587;New Note;;1,1,1,1;Et on change la "forme" de l'ouline grace a une fresnel que l'on peut modifier selon la taille;0;0
Node;AmplifyShaderEditor.StickyNoteNode;6;-2422.696,54.74362;Inherit;False;429.2153;100;New Note;;1,1,1,1;Même fonctionnement mais avec l'axe X;0;0
Node;AmplifyShaderEditor.ScreenDepthNode;7;-1604.393,291.3795;Inherit;False;0;True;1;0;FLOAT4;0,0,0,0;False;1;FLOAT;0
Node;AmplifyShaderEditor.TemplateShaderPropertyNode;8;-2559.388,447.4806;Inherit;False;0;0;_MainTex_TexelSize;Shader;False;0;5;FLOAT4;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.DynamicAppendNode;9;-1996.893,195.2795;Inherit;False;FLOAT2;4;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.SimpleAddOpNode;10;-2226.692,216.6796;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleSubtractOpNode;11;-2236.687,490.7795;Inherit;False;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.DynamicAppendNode;12;-1990.987,472.5796;Inherit;False;FLOAT2;4;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.ScreenPosInputsNode;13;-2543.491,199.2785;Float;False;0;False;0;5;FLOAT4;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.DistanceOpNode;14;-1308.916,445.1985;Inherit;False;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.ScreenDepthNode;15;-1651.986,477.4785;Inherit;False;0;True;1;0;FLOAT4;0,0,0,0;False;1;FLOAT;0
Node;AmplifyShaderEditor.ScreenDepthNode;16;-1530.196,-450.3747;Inherit;False;0;True;1;0;FLOAT4;0,0,0,0;False;1;FLOAT;0
Node;AmplifyShaderEditor.DynamicAppendNode;17;-1910.004,-521.2947;Inherit;False;FLOAT2;4;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.DistanceOpNode;18;-1262.809,-340.8533;Inherit;False;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.ScreenDepthNode;19;-1527.842,-237.5088;Inherit;False;0;True;1;0;FLOAT4;0,0,0,0;False;1;FLOAT;0
Node;AmplifyShaderEditor.ScreenPosInputsNode;20;-2563.727,-501.7581;Float;False;0;False;0;5;FLOAT4;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleSubtractOpNode;21;-2090.419,-527.1687;Inherit;False;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;22;-2089.225,-216.66;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.DynamicAppendNode;23;-1953.624,-215.3343;Inherit;False;FLOAT2;4;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.TemplateShaderPropertyNode;24;-2543.892,-291.2249;Inherit;False;0;0;_MainTex_TexelSize;Shader;False;0;5;FLOAT4;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleAddOpNode;25;-981.9014,324.9506;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.LerpOp;26;-731.2255,241.7865;Inherit;False;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.ColorNode;27;-1050.296,60.5928;Inherit;False;Property;_Color0;Color 0;0;0;Create;True;0;0;0;False;0;False;1,0,0,0;0,0.8867924,0.7119738,0;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RangedFloatNode;28;-1087.174,-204.6068;Inherit;False;Property;_ScaleBias;Scale Bias;1;0;Create;True;0;0;0;False;0;False;0;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.FresnelNode;29;-887.4974,-256.7965;Inherit;False;Standard;WorldNormal;ViewDir;False;False;5;0;FLOAT3;0,0,1;False;4;FLOAT3;0,0,0;False;1;FLOAT;0;False;2;FLOAT;1.5;False;3;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleSubtractOpNode;30;-615.8085,-194.8448;Inherit;False;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;31;-424.1046,-70.37148;Inherit;True;2;2;0;FLOAT;0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.TemplateShaderPropertyNode;32;-744.2581,-594.3221;Inherit;False;0;0;_MainTex;Shader;False;0;5;SAMPLER2D;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SamplerNode;33;-494.838,-495.5365;Inherit;True;Property;_TextureSample0;Texture Sample 0;2;0;Create;True;0;0;0;False;0;False;-1;None;None;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RangedFloatNode;37;-419.9361,-175.0012;Inherit;False;Constant;_Float0;Float 0;2;0;Create;True;0;0;0;False;0;False;1;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;40;-219.5782,100.7073;Inherit;False;Constant;_Float2;Float 1;2;0;Create;True;0;0;0;False;0;False;1.689049;0;0;5;0;1;FLOAT;0
Node;AmplifyShaderEditor.TemplateMultiPassMasterNode;0;407.6993,-261.1988;Float;False;True;-1;2;ASEMaterialInspector;0;8;Outline;32139be9c1eb75640a847f011acf3bcf;True;SubShader 0 Pass 0;0;0;SubShader 0 Pass 0;1;False;False;False;False;False;False;False;False;False;False;False;False;False;False;True;2;False;;False;False;False;False;False;False;False;False;False;False;False;True;2;False;;True;7;False;;False;False;False;False;0;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;True;2;False;0;;0;0;Standard;0;0;1;True;False;;False;0
Node;AmplifyShaderEditor.ScaleAndOffsetNode;35;139.5314,-281.4134;Inherit;False;3;0;COLOR;0,0,0,0;False;1;FLOAT;1;False;2;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.ClampOpNode;38;33.72514,-78.07483;Inherit;False;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;COLOR;1,1,1,0;False;1;COLOR;0
WireConnection;7;0;9;0
WireConnection;9;0;13;1
WireConnection;9;1;10;0
WireConnection;10;0;13;2
WireConnection;10;1;8;2
WireConnection;11;0;13;2
WireConnection;11;1;8;2
WireConnection;12;0;13;1
WireConnection;12;1;11;0
WireConnection;14;0;7;0
WireConnection;14;1;15;0
WireConnection;15;0;12;0
WireConnection;16;0;17;0
WireConnection;17;0;21;0
WireConnection;17;1;20;2
WireConnection;18;0;16;0
WireConnection;18;1;19;0
WireConnection;19;0;23;0
WireConnection;21;0;20;1
WireConnection;21;1;24;1
WireConnection;22;0;20;1
WireConnection;22;1;24;1
WireConnection;23;0;22;0
WireConnection;23;1;20;2
WireConnection;25;0;18;0
WireConnection;25;1;14;0
WireConnection;26;1;27;0
WireConnection;26;2;25;0
WireConnection;29;2;28;0
WireConnection;30;0;29;0
WireConnection;31;0;30;0
WireConnection;31;1;26;0
WireConnection;33;0;32;0
WireConnection;0;0;35;0
WireConnection;35;0;33;0
WireConnection;35;1;37;0
WireConnection;35;2;38;0
WireConnection;38;0;31;0
WireConnection;38;2;40;0
ASEEND*/
//CHKSM=C95BE5502E30D7FA9E99506AF079E5100CD383E9