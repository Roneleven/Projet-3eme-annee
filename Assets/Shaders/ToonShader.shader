// Made with Amplify Shader Editor v1.9.2
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "ToonShader"
{
	Properties
	{
		
	}
	
	SubShader
	{
		
		
		Tags { "RenderType"="Opaque" }
	LOD 100

		CGINCLUDE
		#pragma target 3.0
		ENDCG
		Blend Off
		AlphaToMask Off
		Cull Back
		ColorMask RGBA
		ZWrite On
		ZTest LEqual
		Offset 0 , 0
		
		
		
		Pass
		{
			Name "Unlit"

			CGPROGRAM

			

			#ifndef UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX
			//only defining to not throw compilation error over Unity 5.5
			#define UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(input)
			#endif
			#pragma vertex vert
			#pragma fragment frag
			#pragma multi_compile_instancing
			#include "UnityCG.cginc"
			

			struct appdata
			{
				float4 vertex : POSITION;
				float4 color : COLOR;
				
				UNITY_VERTEX_INPUT_INSTANCE_ID
			};
			
			struct v2f
			{
				float4 vertex : SV_POSITION;
				#ifdef ASE_NEEDS_FRAG_WORLD_POSITION
				float3 worldPos : TEXCOORD0;
				#endif
				
				UNITY_VERTEX_INPUT_INSTANCE_ID
				UNITY_VERTEX_OUTPUT_STEREO
			};

			
			
			v2f vert ( appdata v )
			{
				v2f o;
				UNITY_SETUP_INSTANCE_ID(v);
				UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);
				UNITY_TRANSFER_INSTANCE_ID(v, o);

				
				float3 vertexValue = float3(0, 0, 0);
				#if ASE_ABSOLUTE_VERTEX_POS
				vertexValue = v.vertex.xyz;
				#endif
				vertexValue = vertexValue;
				#if ASE_ABSOLUTE_VERTEX_POS
				v.vertex.xyz = vertexValue;
				#else
				v.vertex.xyz += vertexValue;
				#endif
				o.vertex = UnityObjectToClipPos(v.vertex);

				#ifdef ASE_NEEDS_FRAG_WORLD_POSITION
				o.worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;
				#endif
				return o;
			}
			
			fixed4 frag (v2f i ) : SV_Target
			{
				UNITY_SETUP_INSTANCE_ID(i);
				UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(i);
				fixed4 finalColor;
				#ifdef ASE_NEEDS_FRAG_WORLD_POSITION
				float3 WorldPosition = i.worldPos;
				#endif
				
				
				finalColor = fixed4(1,1,1,1);
				return finalColor;
			}
			ENDCG
		}
	}
	CustomEditor "ASEMaterialInspector"
	
	Fallback Off
}
/*ASEBEGIN
Version=19200
Node;AmplifyShaderEditor.CommentaryNode;56;-1518.132,1093.711;Inherit;False;1863.283;734.9105;Comment;13;44;52;53;54;38;40;41;42;39;43;50;51;45;Rim Light;1,1,1,1;0;0
Node;AmplifyShaderEditor.CommentaryNode;37;-1200.727,384.8059;Inherit;False;1106.648;531.7162;Comment;8;30;29;33;34;35;36;31;32;Lighting;1,1,1,1;0;0
Node;AmplifyShaderEditor.CommentaryNode;26;-1043.883,-673.8999;Inherit;False;832.1627;466.9034;Comment;4;23;24;22;25;Albedo;1,1,1,1;0;0
Node;AmplifyShaderEditor.CommentaryNode;21;-1173.507,-73.562;Inherit;False;1199.585;317.5657;Comment;7;17;16;19;14;20;28;27;Shadow;1,1,1,1;0;0
Node;AmplifyShaderEditor.CommentaryNode;12;-2213.44,226.3291;Inherit;False;819.9753;440.4;Comment;4;5;4;6;8;Normal.ViewDir;1,1,1,1;0;0
Node;AmplifyShaderEditor.CommentaryNode;11;-2145.948,-571.7475;Inherit;False;775.7667;496;Comment;4;3;2;1;7;Normal.LightDir;1,1,1,1;0;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;7;-1612.182,-395.7596;Float;False;normal_lightdir;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.ScaleAndOffsetNode;19;-834.4451,14.23103;Inherit;False;3;0;FLOAT;0;False;1;FLOAT;1;False;2;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;14;-1123.507,-23.56198;Inherit;False;7;normal_lightdir;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;20;-1078.102,131.0037;Inherit;False;Constant;_Float0;Float 0;1;0;Create;True;0;0;0;False;0;False;0;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.ColorNode;23;-993.8828,-623.8999;Inherit;False;Property;_Tint;Tint;2;0;Create;True;0;0;0;False;0;False;1,1,1,0;0,0,0,0;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;24;-642.3237,-600.9721;Inherit;False;2;2;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SamplerNode;16;-611.3034,31.67308;Inherit;True;Property;_ToonRamp;ToonRamp;0;0;Create;True;0;0;0;False;0;False;-1;None;6b5f9694253bb044492a630552310d82;True;1;False;white;Auto;False;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.GetLocalVarNode;28;-441.706,-37.68402;Inherit;False;25;albedo;1;0;OBJECT;;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;27;-304.1954,47.22769;Inherit;False;2;2;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;17;-177.1674,168.6108;Float;False;shadow;-1;True;1;0;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.LightColorNode;30;-961.1643,512.2098;Inherit;False;0;3;COLOR;0;FLOAT3;1;FLOAT;2
Node;AmplifyShaderEditor.GetLocalVarNode;29;-992.3956,434.8059;Inherit;False;17;shadow;1;0;OBJECT;;False;1;COLOR;0
Node;AmplifyShaderEditor.IndirectDiffuseLighting;33;-1139.168,657.8821;Inherit;False;Tangent;1;0;FLOAT3;0,0,1;False;1;FLOAT3;0
Node;AmplifyShaderEditor.LightAttenuation;34;-1150.727,803.5222;Inherit;False;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;35;-862.9144,758.4431;Inherit;False;2;2;0;FLOAT3;0,0,0;False;1;FLOAT;0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;36;-702.2482,657.8822;Inherit;False;2;2;0;COLOR;0,0,0,0;False;1;FLOAT3;0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;31;-537.7145,600.9551;Inherit;False;2;2;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.GetLocalVarNode;52;-1037.141,1590.783;Inherit;False;7;normal_lightdir;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;38;-1467.839,1245.229;Inherit;False;8;normal_viewdir;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.SaturateNode;41;-1040.392,1207.664;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.OneMinusNode;42;-862.1797,1205.186;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;39;-1192.664,1206.802;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.PowerNode;43;-649.5613,1207.717;Inherit;False;False;2;0;FLOAT;0;False;1;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;51;-88.93892,1224.584;Inherit;False;2;2;0;FLOAT;0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;45;103.1506,1223.1;Inherit;False;rim;-1;True;1;0;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleAddOpNode;60;-1014.302,2049.508;Inherit;False;2;2;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.ViewDirInputsCoordNode;59;-1255.206,2013.699;Inherit;False;World;True;0;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.WorldSpaceLightPos;61;-1398.872,2179.242;Inherit;False;0;3;FLOAT4;0;FLOAT3;1;FLOAT;2
Node;AmplifyShaderEditor.DotProductOpNode;64;-846.1539,2060.593;Inherit;False;2;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;1;FLOAT;0
Node;AmplifyShaderEditor.PowerNode;65;-653.7537,2081.393;Inherit;False;False;2;0;FLOAT;0;False;1;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;66;-920.2536,2234.793;Inherit;False;Property;_SpecSomething;SpecSomething;6;0;Create;True;0;0;0;False;0;False;0;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SmoothstepOpNode;67;-452.2537,2141.193;Inherit;False;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;68;-655.0536,2275.093;Inherit;False;Property;_min;min;7;0;Create;True;0;0;0;False;0;False;0;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;69;-653.7535,2356.993;Inherit;False;Property;_max;max;8;0;Create;True;0;0;0;False;0;False;0;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;71;-428.8535,2312.793;Inherit;False;Property;_SpecIntensity;Spec Intensity;9;0;Create;True;0;0;0;False;0;False;0;0;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;70;-93.45358,2155.493;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;72;81.49565,2156.738;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.LightAttenuation;73;-158.4401,2039.293;Inherit;False;0;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;74;253.5446,2180.192;Inherit;False;spec;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;57;452.9205,528.0833;Inherit;False;2;2;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.RangedFloatNode;40;-1468.132,1143.711;Float;False;Property;_RimOffset;Rim Offset;3;0;Create;True;0;0;0;False;0;False;1;-0.18;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SamplerNode;22;-947.6391,-433.9965;Inherit;True;Property;_TextureSample0;Texture Sample 0;1;0;Create;True;0;0;0;False;0;False;-1;None;None;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.LightAttenuation;54;-1056.542,1711.594;Inherit;False;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;53;-464.9219,1207.335;Inherit;False;3;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.ColorNode;50;-483.7087,1380.054;Float;False;Property;_RimTint;Rim Tint;5;0;Create;True;0;0;0;False;0;False;0.2311321,0.9331002,1,0;0,0.9138588,1,0;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RangedFloatNode;44;-966.0825,1350.177;Inherit;False;Property;_RimPower;Rim Power;4;0;Create;True;0;0;0;False;0;False;0;2;0;2;0;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;32;-336.0789,582.4684;Inherit;False;lighting;-1;True;1;0;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.GetLocalVarNode;75;291.6616,767.0515;Inherit;False;74;spec;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;46;251.17,656.2158;Inherit;False;45;rim;1;0;OBJECT;;False;1;COLOR;0
Node;AmplifyShaderEditor.GetLocalVarNode;18;148.3378,441.2529;Inherit;False;32;lighting;1;0;OBJECT;;False;1;COLOR;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;25;-453.7201,-560.7743;Float;False;albedo;-1;True;1;0;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;8;-1634.464,372.2517;Float;False;normal_viewdir;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.WorldNormalVector;4;-2163.44,276.3291;Inherit;False;True;1;0;FLOAT3;0,0,1;False;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.ViewDirInputsCoordNode;6;-2134.539,480.7291;Inherit;False;World;True;0;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.DotProductOpNode;5;-1872.839,339.7291;Inherit;True;2;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;1;FLOAT;0
Node;AmplifyShaderEditor.DotProductOpNode;2;-1836.949,-373.7476;Inherit;True;2;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;1;FLOAT;0
Node;AmplifyShaderEditor.WorldNormalVector;1;-2112.948,-522.7476;Inherit;False;True;1;0;FLOAT3;0,0,1;False;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.WorldSpaceLightDirHlpNode;3;-2117.948,-260.7476;Inherit;False;True;1;0;FLOAT;0;False;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.TemplateMultiPassMasterNode;76;646.0631,321.011;Float;False;True;-1;2;ASEMaterialInspector;100;5;ToonShader;0770190933193b94aaa3065e307002fa;True;Unlit;0;0;Unlit;2;False;True;0;1;False;;0;False;;0;1;False;;0;False;;True;0;False;;0;False;;False;False;False;False;False;False;False;False;False;True;0;False;;False;True;0;False;;False;True;True;True;True;True;0;False;;False;False;False;False;False;False;False;True;False;0;False;;255;False;;255;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;False;True;1;False;;True;3;False;;True;True;0;False;;0;False;;True;1;RenderType=Opaque=RenderType;True;2;False;0;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;0;;0;0;Standard;1;Vertex Position,InvertActionOnDeselection;1;0;0;1;True;False;;False;0
WireConnection;7;0;2;0
WireConnection;19;0;14;0
WireConnection;19;1;20;0
WireConnection;19;2;20;0
WireConnection;24;0;23;0
WireConnection;24;1;22;0
WireConnection;16;1;19;0
WireConnection;27;0;28;0
WireConnection;27;1;16;0
WireConnection;17;0;27;0
WireConnection;35;0;33;0
WireConnection;35;1;34;0
WireConnection;36;0;30;0
WireConnection;36;1;35;0
WireConnection;31;0;29;0
WireConnection;31;1;36;0
WireConnection;41;0;39;0
WireConnection;42;0;41;0
WireConnection;39;0;40;0
WireConnection;39;1;38;0
WireConnection;43;0;42;0
WireConnection;43;1;44;0
WireConnection;51;0;53;0
WireConnection;51;1;50;0
WireConnection;45;0;51;0
WireConnection;60;0;59;0
WireConnection;60;1;61;1
WireConnection;64;0;60;0
WireConnection;65;0;64;0
WireConnection;65;1;66;0
WireConnection;67;0;65;0
WireConnection;67;1;68;0
WireConnection;67;2;69;0
WireConnection;70;0;67;0
WireConnection;70;1;71;0
WireConnection;72;0;73;0
WireConnection;72;1;70;0
WireConnection;74;0;72;0
WireConnection;57;0;18;0
WireConnection;57;1;46;0
WireConnection;53;0;43;0
WireConnection;53;1;52;0
WireConnection;53;2;54;0
WireConnection;32;0;31;0
WireConnection;25;0;24;0
WireConnection;8;0;5;0
WireConnection;5;0;4;0
WireConnection;5;1;6;0
WireConnection;2;0;1;0
WireConnection;2;1;3;0
ASEEND*/
//CHKSM=6615EB7E4ECEA12B690220391C1912F65F44C9D0