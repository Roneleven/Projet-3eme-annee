// Made with Amplify Shader Editor v1.9.2
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "OutlineAMPLIFY"
{
	Properties
	{
		_MainTexture1("Albedo", 2D) = "white" {}
		_Smoothness("Smoothness", Range( 0 , 1)) = 0.9096047
		_Color1("Color 0", Color) = (0.9622642,0.9622642,0.9622642,0)
		[HDR]_Specular("Specular", Color) = (0,0,0,0)
		_FresnelScale("FresnelScale", Float) = 1.99
		_Highlight("Highlight", Float) = 0
		_RimStrengh("RimStrengh", Float) = 0.63
		[HideInInspector] _texcoord( "", 2D ) = "white" {}
		[HideInInspector] __dirty( "", Int ) = 1
	}

	SubShader
	{
		Tags{ "RenderType" = "Opaque"  "Queue" = "Geometry+0" }
		Cull Back
		CGINCLUDE
		#include "UnityPBSLighting.cginc"
		#include "UnityShaderVariables.cginc"
		#include "UnityCG.cginc"
		#include "Lighting.cginc"
		#pragma target 3.0
		#ifdef UNITY_PASS_SHADOWCASTER
			#undef INTERNAL_DATA
			#undef WorldReflectionVector
			#undef WorldNormalVector
			#define INTERNAL_DATA half3 internalSurfaceTtoW0; half3 internalSurfaceTtoW1; half3 internalSurfaceTtoW2;
			#define WorldReflectionVector(data,normal) reflect (data.worldRefl, half3(dot(data.internalSurfaceTtoW0,normal), dot(data.internalSurfaceTtoW1,normal), dot(data.internalSurfaceTtoW2,normal)))
			#define WorldNormalVector(data,normal) half3(dot(data.internalSurfaceTtoW0,normal), dot(data.internalSurfaceTtoW1,normal), dot(data.internalSurfaceTtoW2,normal))
		#endif
		struct Input
		{
			float3 worldPos;
			float3 worldNormal;
			INTERNAL_DATA
			float2 uv_texcoord;
		};

		struct SurfaceOutputCustomLightingCustom
		{
			half3 Albedo;
			half3 Normal;
			half3 Emission;
			half Metallic;
			half Smoothness;
			half Occlusion;
			half Alpha;
			Input SurfInput;
			UnityGIInput GIData;
		};

		uniform sampler2D _MainTexture1;
		uniform float4 _MainTexture1_ST;
		uniform float4 _Color1;
		uniform float _Smoothness;
		uniform float4 _Specular;
		uniform float _FresnelScale;
		uniform float _Highlight;
		uniform float _RimStrengh;

		inline half4 LightingStandardCustomLighting( inout SurfaceOutputCustomLightingCustom s, half3 viewDir, UnityGI gi )
		{
			UnityGIInput data = s.GIData;
			Input i = s.SurfInput;
			half4 c = 0;
			#ifdef UNITY_PASS_FORWARDBASE
			float ase_lightAtten = data.atten;
			if( _LightColor0.a == 0)
			ase_lightAtten = 0;
			#else
			float3 ase_lightAttenRGB = gi.light.color / ( ( _LightColor0.rgb ) + 0.000001 );
			float ase_lightAtten = max( max( ase_lightAttenRGB.r, ase_lightAttenRGB.g ), ase_lightAttenRGB.b );
			#endif
			#if defined(HANDLE_SHADOWS_BLENDING_IN_GI)
			half bakedAtten = UnitySampleBakedOcclusion(data.lightmapUV.xy, data.worldPos);
			float zDist = dot(_WorldSpaceCameraPos - data.worldPos, UNITY_MATRIX_V[2].xyz);
			float fadeDist = UnityComputeShadowFadeDistance(data.worldPos, zDist);
			ase_lightAtten = UnityMixRealtimeAndBakedShadows(data.atten, bakedAtten, UnityComputeShadowFade(fadeDist));
			#endif
			float3 ase_worldPos = i.worldPos;
			#if defined(LIGHTMAP_ON) && UNITY_VERSION < 560 //aseld
			float3 ase_worldlightDir = 0;
			#else //aseld
			float3 ase_worldlightDir = Unity_SafeNormalize( UnityWorldSpaceLightDir( ase_worldPos ) );
			#endif //aseld
			float dotResult136 = dot( _WorldSpaceCameraPos , ase_worldlightDir );
			float temp_output_3_0_g12 = ( min( saturate( dotResult136 ) , ase_lightAtten ) - 0.1 );
			float temp_output_143_0 = saturate( ( temp_output_3_0_g12 / fwidth( temp_output_3_0_g12 ) ) );
			float3 ase_worldNormal = WorldNormalVector( i, float3( 0, 0, 1 ) );
			float3 ase_normWorldNormal = normalize( ase_worldNormal );
			UnityGI gi148 = gi;
			float3 diffNorm148 = ase_normWorldNormal;
			gi148 = UnityGI_Base( data, 1, diffNorm148 );
			float3 indirectDiffuse148 = gi148.indirect.diffuse + diffNorm148 * 0.0001;
			float2 uv_MainTexture1 = i.uv_texcoord * _MainTexture1_ST.xy + _MainTexture1_ST.zw;
			float4 Diffuse147 = ( float4( ( temp_output_143_0 + ( ( 1.0 - temp_output_143_0 ) * indirectDiffuse148 ) ) , 0.0 ) * ( tex2D( _MainTexture1, uv_MainTexture1 ) * _Color1 ) );
			float3 ase_worldViewDir = Unity_SafeNormalize( UnityWorldSpaceViewDir( ase_worldPos ) );
			float3 normalizeResult70 = normalize( ( ase_worldViewDir + ase_worldlightDir ) );
			float dotResult71 = dot( normalizeResult70 , ase_normWorldNormal );
			float temp_output_3_0_g11 = ( pow( dotResult71 , exp2( ( ( _Smoothness * 10.0 ) + 4.72 ) ) ) - 0.1 );
			float4 Specular79 = ( saturate( ( temp_output_3_0_g11 / fwidth( temp_output_3_0_g11 ) ) ) * _Specular * _Smoothness );
			#if defined(LIGHTMAP_ON) && ( UNITY_VERSION < 560 || ( defined(LIGHTMAP_SHADOW_MIXING) && !defined(SHADOWS_SHADOWMASK) && defined(SHADOWS_SCREEN) ) )//aselc
			float4 ase_lightColor = 0;
			#else //aselc
			float4 ase_lightColor = _LightColor0;
			#endif //aselc
			float fresnelNdotV91 = dot( ase_worldNormal, ase_worldViewDir );
			float fresnelNode91 = ( 0.0 + _FresnelScale * pow( 1.0 - fresnelNdotV91, 5.0 ) );
			float temp_output_3_0_g9 = ( fresnelNode91 - 0.1 );
			float temp_output_96_0 = saturate( ( temp_output_3_0_g9 / fwidth( temp_output_3_0_g9 ) ) );
			float4 Highlig90 = ( ( (0) * ase_lightColor * temp_output_96_0 * _Highlight ) + ( ( 1.0 - 0.0 ) * temp_output_96_0 * _RimStrengh ) );
			c.rgb = ( Diffuse147 + Specular79 + Highlig90 ).rgb;
			c.a = 1;
			return c;
		}

		inline void LightingStandardCustomLighting_GI( inout SurfaceOutputCustomLightingCustom s, UnityGIInput data, inout UnityGI gi )
		{
			s.GIData = data;
		}

		void surf( Input i , inout SurfaceOutputCustomLightingCustom o )
		{
			o.SurfInput = i;
			o.Normal = float3(0,0,1);
		}

		ENDCG
		CGPROGRAM
		#pragma surface surf StandardCustomLighting keepalpha fullforwardshadows 

		ENDCG
		Pass
		{
			Name "ShadowCaster"
			Tags{ "LightMode" = "ShadowCaster" }
			ZWrite On
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#pragma target 3.0
			#pragma multi_compile_shadowcaster
			#pragma multi_compile UNITY_PASS_SHADOWCASTER
			#pragma skip_variants FOG_LINEAR FOG_EXP FOG_EXP2
			#include "HLSLSupport.cginc"
			#if ( SHADER_API_D3D11 || SHADER_API_GLCORE || SHADER_API_GLES || SHADER_API_GLES3 || SHADER_API_METAL || SHADER_API_VULKAN )
				#define CAN_SKIP_VPOS
			#endif
			#include "UnityCG.cginc"
			#include "Lighting.cginc"
			#include "UnityPBSLighting.cginc"
			struct v2f
			{
				V2F_SHADOW_CASTER;
				float2 customPack1 : TEXCOORD1;
				float4 tSpace0 : TEXCOORD2;
				float4 tSpace1 : TEXCOORD3;
				float4 tSpace2 : TEXCOORD4;
				UNITY_VERTEX_INPUT_INSTANCE_ID
				UNITY_VERTEX_OUTPUT_STEREO
			};
			v2f vert( appdata_full v )
			{
				v2f o;
				UNITY_SETUP_INSTANCE_ID( v );
				UNITY_INITIALIZE_OUTPUT( v2f, o );
				UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO( o );
				UNITY_TRANSFER_INSTANCE_ID( v, o );
				Input customInputData;
				float3 worldPos = mul( unity_ObjectToWorld, v.vertex ).xyz;
				half3 worldNormal = UnityObjectToWorldNormal( v.normal );
				half3 worldTangent = UnityObjectToWorldDir( v.tangent.xyz );
				half tangentSign = v.tangent.w * unity_WorldTransformParams.w;
				half3 worldBinormal = cross( worldNormal, worldTangent ) * tangentSign;
				o.tSpace0 = float4( worldTangent.x, worldBinormal.x, worldNormal.x, worldPos.x );
				o.tSpace1 = float4( worldTangent.y, worldBinormal.y, worldNormal.y, worldPos.y );
				o.tSpace2 = float4( worldTangent.z, worldBinormal.z, worldNormal.z, worldPos.z );
				o.customPack1.xy = customInputData.uv_texcoord;
				o.customPack1.xy = v.texcoord;
				TRANSFER_SHADOW_CASTER_NORMALOFFSET( o )
				return o;
			}
			half4 frag( v2f IN
			#if !defined( CAN_SKIP_VPOS )
			, UNITY_VPOS_TYPE vpos : VPOS
			#endif
			) : SV_Target
			{
				UNITY_SETUP_INSTANCE_ID( IN );
				Input surfIN;
				UNITY_INITIALIZE_OUTPUT( Input, surfIN );
				surfIN.uv_texcoord = IN.customPack1.xy;
				float3 worldPos = float3( IN.tSpace0.w, IN.tSpace1.w, IN.tSpace2.w );
				half3 worldViewDir = normalize( UnityWorldSpaceViewDir( worldPos ) );
				surfIN.worldPos = worldPos;
				surfIN.worldNormal = float3( IN.tSpace0.z, IN.tSpace1.z, IN.tSpace2.z );
				surfIN.internalSurfaceTtoW0 = IN.tSpace0.xyz;
				surfIN.internalSurfaceTtoW1 = IN.tSpace1.xyz;
				surfIN.internalSurfaceTtoW2 = IN.tSpace2.xyz;
				SurfaceOutputCustomLightingCustom o;
				UNITY_INITIALIZE_OUTPUT( SurfaceOutputCustomLightingCustom, o )
				surf( surfIN, o );
				#if defined( CAN_SKIP_VPOS )
				float2 vpos = IN.pos;
				#endif
				SHADOW_CASTER_FRAGMENT( IN )
			}
			ENDCG
		}
	}
	Fallback "Diffuse"
	CustomEditor "ASEMaterialInspector"
}
/*ASEBEGIN
Version=19200
Node;AmplifyShaderEditor.StandardSurfaceOutputNode;0;0,0;Float;False;True;-1;2;ASEMaterialInspector;0;0;CustomLighting;OutlineAMPLIFY;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;Back;0;False;;0;False;;False;0;False;;0;False;;False;0;Opaque;0.5;True;True;0;False;Opaque;;Geometry;All;12;all;True;True;True;True;0;False;;False;0;False;;255;False;;255;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;False;2;15;10;25;False;0.5;True;0;0;False;;0;False;;0;0;False;;0;False;;0;False;;0;False;;0;False;0;0,0,0,0;VertexScale;True;False;Cylindrical;False;True;Relative;0;;-1;-1;-1;-1;0;False;0;0;False;;-1;0;False;;0;0;0;False;0.1;False;;0;False;;False;15;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT3;0,0,0;False;3;FLOAT3;0,0,0;False;4;FLOAT;0;False;6;FLOAT3;0,0,0;False;7;FLOAT3;0,0,0;False;8;FLOAT;0;False;9;FLOAT;0;False;10;FLOAT;0;False;13;FLOAT3;0,0,0;False;11;FLOAT3;0,0,0;False;12;FLOAT3;0,0,0;False;14;FLOAT4;0,0,0,0;False;15;FLOAT3;0,0,0;False;0
Node;AmplifyShaderEditor.StickyNoteNode;73;-2480.297,-75.96423;Inherit;False;443.674;122.8641;New Note;;1,1,1,1;On combine ici direction de vue avec la direction de la lumière dans l'espace mondial en les additionnant. Cela peut être utilisé pour ajuster l'éclairage en fonction de l'angle entre la vue de la caméra et la direction de la lumière.;0;0
Node;AmplifyShaderEditor.StickyNoteNode;75;-1932.296,-10.68133;Inherit;False;576.4318;115.3293;New Note;;1,1,1,1;On normalise le résultat de l'addition (afin de le rendre unitaire) et on le relie à la normale du monde. Ensuite, vous prenez le produit scalaire entre le résultat normalisé et la normale du monde. Pour calculer des effets d'éclairage en fonction de l'orientation de la surface par rapport à la lumière et à la caméra.;0;0
Node;AmplifyShaderEditor.StickyNoteNode;76;-2009.18,642.8849;Inherit;False;420.3521;100;New Note;;1,1,1,1;On peut ajuster la courbe de luminosité ou d'intensité de l'éclairage en fonction de la valeur de "Smoothness";0;0
Node;AmplifyShaderEditor.StickyNoteNode;77;-1224.529,206.0238;Inherit;False;486.5151;100;New Note;;1,1,1,1;étape d'ajustement de la couleur en fonction de la netteté de l'éclairage.;0;0
Node;AmplifyShaderEditor.StickyNoteNode;84;-4911.475,382.282;Inherit;False;571.2896;139.3203;New Note;;1,1,1,1;On ajuste l'effet Fresnel qui est une propriété optique qui décrit la manière dont la réflexion de la lumière varie en fonction de l'angle entre la surface et la vue de la caméra.$On en fait un masque grâce au Step Antialiasing.;0;0
Node;AmplifyShaderEditor.StickyNoteNode;85;-4184.697,-335.1953;Inherit;False;325.257;115.1455;New Note;;1,1,1,1;On accentue certains aspects de l'éclairage en fonction de l'effet Fresnel.;0;0
Node;AmplifyShaderEditor.StickyNoteNode;86;-3579.025,477.5428;Inherit;False;347.3879;100;New Note;;1,1,1,1;On utilise l'effet Fresnel pour ajuster la luminosité et l'intensité des reflets de lumière indirecte diffuse et des effets de contour;0;0
Node;AmplifyShaderEditor.StickyNoteNode;87;-380.6781,-206.9424;Inherit;False;364.4021;100;New Note;;1,1,1,1;Pour que le Shader fonctionne, on combine 3 effets;0;0
Node;AmplifyShaderEditor.NormalizeNode;70;-1824.509,129.5856;Inherit;False;False;1;0;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.DotProductOpNode;71;-1625.845,149.2725;Inherit;False;2;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;1;FLOAT;0
Node;AmplifyShaderEditor.WorldNormalVector;72;-1839.07,240.3358;Inherit;False;True;1;0;FLOAT3;0,0,1;False;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.SimpleAddOpNode;74;-2171.722,142.7587;Inherit;False;2;2;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;78;-936.3508,416.4279;Inherit;False;3;3;0;FLOAT;0;False;1;COLOR;0,0,0,0;False;2;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;79;-737.4171,419.4808;Inherit;False;Specular;-1;True;1;0;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.RangedFloatNode;80;-4989.835,274.119;Inherit;False;Property;_FresnelScale;FresnelScale;4;0;Create;True;0;0;0;False;0;False;1.99;3;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.LightColorNode;81;-4507.35,-184.2013;Inherit;False;0;3;COLOR;0;FLOAT3;1;FLOAT;2
Node;AmplifyShaderEditor.RangedFloatNode;82;-4515.212,-28.60522;Inherit;False;Property;_Highlight;Highlight;5;0;Create;True;0;0;0;False;0;False;0;1;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;83;-4524.475,-353.3242;Inherit;False;-1;;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.Exp2OpNode;89;-1606.095,485.6298;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;90;-3154.88,57.39163;Inherit;False;Highlig;-1;True;1;0;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.FresnelNode;91;-4629.278,202.3387;Inherit;True;Standard;WorldNormal;ViewDir;False;False;5;0;FLOAT3;0,0,1;False;4;FLOAT3;0,0,0;False;1;FLOAT;0;False;2;FLOAT;1;False;3;FLOAT;5;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;94;-3652.645,179.9717;Inherit;True;3;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;95;-3844.788,429.465;Inherit;True;Property;_RimStrengh;RimStrengh;6;0;Create;True;0;0;0;False;0;False;0.63;15.29;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.FunctionNode;96;-4309.942,194.6597;Inherit;True;Step Antialiasing;-1;;9;2a825e80dfb3290468194f83380797bd;0;2;1;FLOAT;0.1;False;2;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;97;-3899.322,-206.9164;Inherit;False;4;4;0;FLOAT;0;False;1;COLOR;0,0,0,0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.GetLocalVarNode;98;-4105.965,34.61466;Inherit;False;-1;;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;99;-3411.668,-161.5782;Inherit;True;2;2;0;COLOR;0,0,0,0;False;1;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.OneMinusNode;100;-3838.693,-23.22528;Inherit;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;110;-207.143,114.7981;Inherit;False;3;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.GetLocalVarNode;111;-461.7406,96.71825;Inherit;False;79;Specular;1;0;OBJECT;;False;1;COLOR;0
Node;AmplifyShaderEditor.ViewDirInputsCoordNode;112;-2449.753,60.76663;Inherit;False;World;True;0;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.WorldSpaceLightDirHlpNode;113;-2469.136,253.8219;Inherit;False;True;1;0;FLOAT;0;False;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.RangedFloatNode;114;-2214.504,547.6529;Inherit;False;Property;_Smoothness;Smoothness;1;0;Create;True;0;0;0;False;0;False;0.9096047;0;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;115;-1884.371,484.6759;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;10;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;116;-1737.657,490.1078;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;4.72;False;1;FLOAT;0
Node;AmplifyShaderEditor.PowerNode;117;-1413.269,404.0658;Inherit;False;False;2;0;FLOAT;0;False;1;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.FunctionNode;118;-1184.685,336.7679;Inherit;False;Step Antialiasing;-1;;11;2a825e80dfb3290468194f83380797bd;0;2;1;FLOAT;0.1;False;2;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;125;-457.4208,211.6193;Inherit;False;90;Highlig;1;0;OBJECT;;False;1;COLOR;0
Node;AmplifyShaderEditor.ColorNode;119;-1222.028,508.5128;Inherit;False;Property;_Specular;Specular;3;1;[HDR];Create;True;0;0;0;False;0;False;0,0,0,0;1,1,1,0;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.StickyNoteNode;126;-3305.521,-1397.612;Inherit;False;348.9391;202.8075;New Note;;1,1,1,1;Ces nodes servent à avoir une influence sur le shader selon la light du monde;0;0
Node;AmplifyShaderEditor.StickyNoteNode;127;-3204.606,-848.124;Inherit;False;263.92;115.36;New Note;;1,1,1,1;Ces nodes servent à souligner les ombres, en prenant en compte la définition de la lumière du monde.;0;0
Node;AmplifyShaderEditor.StickyNoteNode;128;-2781.991,-674.9793;Inherit;False;309.2;100;New Note;;1,1,1,1;On utilise ensuite l'antialiasing pour réduire l'effet "escalier" de l'ombre. On utilise les pixels adjacents pour lisser.;0;0
Node;AmplifyShaderEditor.StickyNoteNode;129;-2081.868,-1450.079;Inherit;False;411.5281;128.2422;New Note;;1,1,1,1;Ensuite, la node indirect diffuse light fait référence à la lumière qui n'arrive pas directement d'une source lumineuse, mais qui est plutôt diffusée ou réfléchie par les surfaces environnantes.;0;0
Node;AmplifyShaderEditor.StickyNoteNode;130;-1853.669,-900.5151;Inherit;False;177.8485;113.9243;New Note;;1,1,1,1;Ici, on permet de choisir la couleur du Shader grace à une property;0;0
Node;AmplifyShaderEditor.SimpleAddOpNode;131;-1780.799,-1292.574;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;133;-1481.668,-933.7673;Inherit;True;2;2;0;FLOAT3;0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.ColorNode;134;-2122.011,-616.3419;Inherit;False;Property;_Color1;Color 0;2;0;Create;True;0;0;0;False;0;False;0.9622642,0.9622642,0.9622642,0;0.7486564,1,0,0;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SamplerNode;135;-2158.004,-812.566;Inherit;True;Property;_MainTexture1;Albedo;0;0;Create;False;0;0;0;False;0;False;-1;None;None;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.DotProductOpNode;136;-3265.109,-1144.293;Inherit;True;2;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;1;FLOAT;0
Node;AmplifyShaderEditor.LightAttenuation;137;-3152.587,-710.1241;Inherit;False;0;1;FLOAT;0
Node;AmplifyShaderEditor.SaturateNode;138;-3069.443,-1166.566;Inherit;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.WorldSpaceCameraPos;139;-3694.872,-1332.001;Inherit;False;0;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.WorldNormalVector;140;-3592.049,-1549.297;Inherit;False;True;1;0;FLOAT3;0,0,1;False;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.WorldSpaceLightDirHlpNode;141;-3542.145,-1079.775;Inherit;False;True;1;0;FLOAT;0;False;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.SimpleMinOpNode;142;-2826.013,-981.716;Inherit;True;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;144;-2417.952,-565.764;Inherit;False;NdotL;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.OneMinusNode;145;-2193.492,-1332.248;Inherit;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;146;-1986.593,-1185.484;Inherit;True;2;2;0;FLOAT;0;False;1;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.FunctionNode;143;-2730.956,-560.649;Inherit;True;Step Antialiasing;-1;;12;2a825e80dfb3290468194f83380797bd;0;2;1;FLOAT;0.1;False;2;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.IndirectDiffuseLighting;148;-2285.903,-983.8408;Inherit;False;Tangent;1;0;FLOAT3;0,0,1;False;1;FLOAT3;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;132;-1719.734,-759.2283;Inherit;True;2;2;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;147;-1233.901,-858.4328;Inherit;False;Diffuse;-1;True;1;0;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.GetLocalVarNode;120;-455.6347,-9.129728;Inherit;False;147;Diffuse;1;0;OBJECT;;False;1;COLOR;0
WireConnection;0;13;110;0
WireConnection;70;0;74;0
WireConnection;71;0;70;0
WireConnection;71;1;72;0
WireConnection;74;0;112;0
WireConnection;74;1;113;0
WireConnection;78;0;118;0
WireConnection;78;1;119;0
WireConnection;78;2;114;0
WireConnection;79;0;78;0
WireConnection;89;0;116;0
WireConnection;90;0;99;0
WireConnection;91;2;80;0
WireConnection;94;0;100;0
WireConnection;94;1;96;0
WireConnection;94;2;95;0
WireConnection;96;2;91;0
WireConnection;97;0;83;0
WireConnection;97;1;81;0
WireConnection;97;2;96;0
WireConnection;97;3;82;0
WireConnection;99;0;97;0
WireConnection;99;1;94;0
WireConnection;110;0;120;0
WireConnection;110;1;111;0
WireConnection;110;2;125;0
WireConnection;115;0;114;0
WireConnection;116;0;115;0
WireConnection;117;0;71;0
WireConnection;117;1;89;0
WireConnection;118;2;117;0
WireConnection;131;0;143;0
WireConnection;131;1;146;0
WireConnection;133;0;131;0
WireConnection;133;1;132;0
WireConnection;136;0;139;0
WireConnection;136;1;141;0
WireConnection;138;0;136;0
WireConnection;142;0;138;0
WireConnection;142;1;137;0
WireConnection;144;0;143;0
WireConnection;145;0;143;0
WireConnection;146;0;145;0
WireConnection;146;1;148;0
WireConnection;143;2;142;0
WireConnection;132;0;135;0
WireConnection;132;1;134;0
WireConnection;147;0;133;0
ASEEND*/
//CHKSM=33C61BBF61C18C349F58FFE73542B63A0E40BE4C