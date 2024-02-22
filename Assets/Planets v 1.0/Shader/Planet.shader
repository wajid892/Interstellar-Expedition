// Made with Amplify Shader Editor
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "Methexis Planet Shader"
{
	Properties
	{
		_PlanetAlbedo("Planet Albedo", 2D) = "white" {}
		_ColorVariationControl("Color Variation Control", Range( -3 , 3)) = 1
		[Normal]_PlanetNormal("Planet Normal", 2D) = "bump" {}
		_NormalIntensity("Normal Intensity", Range( 0 , 1)) = 0
		_Clouds_Shadow("Clouds_Shadow", 2D) = "white" {}
		_PlanetSpecular("Planet Specular", 2D) = "white" {}
		_RimColor("RimColor", Color) = (0,0.1686275,0.3215686,0)
		_RimPower("RimPower", Range( 0 , 10)) = 0
		_RimScale("Rim Scale", Range( 0 , 10)) = 0
		_LightsColor("Lights Color", Color) = (0.990566,0.8709589,0.7055447,0)
		[NoScaleOffset]_Emissivecities("Emissive (cities)", 2D) = "black" {}
		_LightControl("Light Control", Color) = (0,0,0,0)
		_SpecularIntensity("Specular Intensity", Range( 0 , 5)) = 0
		[HideInInspector] _texcoord( "", 2D ) = "white" {}
		[HideInInspector] __dirty( "", Int ) = 1
	}

	SubShader
	{
		Tags{ "RenderType" = "Opaque"  "Queue" = "Geometry+0" "IsEmissive" = "true"  }
		Cull Back
		CGINCLUDE
		#include "UnityStandardUtils.cginc"
		#include "UnityCG.cginc"
		#include "UnityPBSLighting.cginc"
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
			float2 uv_texcoord;
			float3 worldNormal;
			INTERNAL_DATA
			float3 worldPos;
		};

		uniform float _NormalIntensity;
		uniform sampler2D _PlanetNormal;
		uniform float4 _PlanetNormal_ST;
		uniform sampler2D _Clouds_Shadow;
		uniform float4 _Clouds_Shadow_ST;
		uniform sampler2D _PlanetAlbedo;
		uniform float4 _PlanetAlbedo_ST;
		uniform float _ColorVariationControl;
		uniform float _RimScale;
		uniform float _RimPower;
		uniform float4 _RimColor;
		uniform float4 _LightControl;
		uniform sampler2D _Emissivecities;
		uniform float4 _LightsColor;
		uniform float _SpecularIntensity;
		uniform sampler2D _PlanetSpecular;
		uniform float4 _PlanetSpecular_ST;


		float4 CalculateContrast( float contrastValue, float4 colorTarget )
		{
			float t = 0.5 * ( 1.0 - contrastValue );
			return mul( float4x4( contrastValue,0,0,t, 0,contrastValue,0,t, 0,0,contrastValue,t, 0,0,0,1 ), colorTarget );
		}

		void surf( Input i , inout SurfaceOutputStandardSpecular o )
		{
			float2 uv_PlanetNormal = i.uv_texcoord * _PlanetNormal_ST.xy + _PlanetNormal_ST.zw;
			float3 lerpResult91 = lerp( UnpackScaleNormal( tex2D( _PlanetNormal, uv_PlanetNormal ), _NormalIntensity ) , float3(0,0,1) , float3( 0,0,0 ));
			o.Normal = lerpResult91;
			float2 uv_Clouds_Shadow = i.uv_texcoord * _Clouds_Shadow_ST.xy + _Clouds_Shadow_ST.zw;
			float2 uv_PlanetAlbedo = i.uv_texcoord * _PlanetAlbedo_ST.xy + _PlanetAlbedo_ST.zw;
			float3 desaturateInitialColor94 = tex2D( _PlanetAlbedo, uv_PlanetAlbedo ).rgb;
			float desaturateDot94 = dot( desaturateInitialColor94, float3( 0.299, 0.587, 0.114 ));
			float3 desaturateVar94 = lerp( desaturateInitialColor94, desaturateDot94.xxx, _ColorVariationControl );
			float4 blendOpSrc89 = CalculateContrast(-0.5,tex2D( _Clouds_Shadow, uv_Clouds_Shadow ));
			float4 blendOpDest89 = float4( desaturateVar94 , 0.0 );
			o.Albedo = ( saturate( (( blendOpDest89 > 0.5 ) ? ( 1.0 - ( 1.0 - 2.0 * ( blendOpDest89 - 0.5 ) ) * ( 1.0 - blendOpSrc89 ) ) : ( 2.0 * blendOpDest89 * blendOpSrc89 ) ) )).rgb;
			float3 ase_worldNormal = WorldNormalVector( i, float3( 0, 0, 1 ) );
			float3 ase_worldPos = i.worldPos;
			#if defined(LIGHTMAP_ON) && UNITY_VERSION < 560 //aseld
			float3 ase_worldlightDir = 0;
			#else //aseld
			float3 ase_worldlightDir = normalize( UnityWorldSpaceLightDir( ase_worldPos ) );
			#endif //aseld
			float dotResult98 = dot( ase_worldNormal , ase_worldlightDir );
			float3 ase_worldViewDir = normalize( UnityWorldSpaceViewDir( ase_worldPos ) );
			float fresnelNdotV32 = dot( ase_worldNormal, ase_worldViewDir );
			float fresnelNode32 = ( 0.0 + _RimScale * pow( 1.0 - fresnelNdotV32, _RimPower ) );
			float smoothstepResult99 = smoothstep( 0.0 , 10.0 , fresnelNode32);
			float dotResult43 = dot( ase_worldNormal , ase_worldlightDir );
			float clampResult53 = clamp( -( dotResult43 - 0.65 ) , 0.0 , 1.0 );
			float smoothstepResult54 = smoothstep( 0.3 , 0.7 , clampResult53);
			o.Emission = ( ( ( dotResult98 * smoothstepResult99 ) * _RimColor ) + ( ( ( 1.0 - _LightControl ) * ( smoothstepResult54 * ( tex2D( _Emissivecities, i.uv_texcoord ) * _LightsColor ) ) ) * 2.0 ) ).rgb;
			float2 uv_PlanetSpecular = i.uv_texcoord * _PlanetSpecular_ST.xy + _PlanetSpecular_ST.zw;
			o.Smoothness = CalculateContrast(_SpecularIntensity,tex2D( _PlanetSpecular, uv_PlanetSpecular )).r;
			o.Alpha = 1;
		}

		ENDCG
		CGPROGRAM
		#pragma surface surf StandardSpecular keepalpha fullforwardshadows 

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
			};
			v2f vert( appdata_full v )
			{
				v2f o;
				UNITY_SETUP_INSTANCE_ID( v );
				UNITY_INITIALIZE_OUTPUT( v2f, o );
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
				SurfaceOutputStandardSpecular o;
				UNITY_INITIALIZE_OUTPUT( SurfaceOutputStandardSpecular, o )
				surf( surfIN, o );
				#if defined( CAN_SKIP_VPOS )
				float2 vpos = IN.pos;
				#endif
				SHADOW_CASTER_FRAGMENT( IN )
			}
			ENDCG
		}
	}
	Fallback "Standard (Specular setup)"
}
/*ASEBEGIN
Version=16700
58;108.6667;2498;1359;4431.148;2835.686;4.066842;True;True
Node;AmplifyShaderEditor.CommentaryNode;44;-769.2943,311.1436;Float;False;2612.78;842.7703;Dark side of the planet;11;67;66;57;52;51;49;45;70;71;65;59;;1,1,1,1;0;0
Node;AmplifyShaderEditor.CommentaryNode;45;-696.1856,353.8404;Float;False;1000.746;336.4277;Mask Controls;5;54;53;50;47;46;;1,1,1,1;0;0
Node;AmplifyShaderEditor.WorldNormalVector;41;-967.308,-113.4883;Float;False;False;1;0;FLOAT3;0,0,1;False;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.WorldSpaceLightDirHlpNode;42;-967.308,101.5119;Float;False;False;1;0;FLOAT;0;False;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.RangedFloatNode;46;-664.995,461.7971;Float;False;Constant;_Float0;Float 0;18;0;Create;True;0;0;False;0;0.65;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.DotProductOpNode;43;-574.308,4.511598;Float;False;2;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleSubtractOpNode;47;-480.3493,438.3639;Float;False;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.NegateNode;50;-291.6456,437.2459;Float;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.TextureCoordinatesNode;49;-519.8952,714.2709;Float;False;0;-1;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.ColorNode;52;-65.4694,924.9982;Float;False;Property;_LightsColor;Lights Color;10;0;Create;True;0;0;False;0;0.990566,0.8709589,0.7055447,0;0,0,0,0;False;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.WorldNormalVector;83;-157.4812,-188.534;Float;False;False;1;0;FLOAT3;0,0,1;False;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.RangedFloatNode;33;-157.4812,51.46607;Float;False;Property;_RimScale;Rim Scale;8;0;Create;True;0;0;False;0;0;0;0;10;0;1;FLOAT;0
Node;AmplifyShaderEditor.ClampOpNode;53;-123.1962,435.7329;Float;False;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;28;-157.4812,147.4661;Float;False;Property;_RimPower;RimPower;7;0;Create;True;0;0;False;0;0;0;0;10;0;1;FLOAT;0
Node;AmplifyShaderEditor.SamplerNode;51;-115.2247,713.7478;Float;True;Property;_Emissivecities;Emissive (cities);11;1;[NoScaleOffset];Create;True;0;0;False;0;504f5f15135ed0540a3ddc55a7622cf9;504f5f15135ed0540a3ddc55a7622cf9;True;0;False;black;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;1;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SmoothstepOpNode;54;67.94646,445.7668;Float;False;3;0;FLOAT;0;False;1;FLOAT;0.3;False;2;FLOAT;0.7;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;57;327.1294,741.6993;Float;True;2;2;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.WorldSpaceLightDirHlpNode;97;-250.2027,-449.9534;Float;False;False;1;0;FLOAT;0;False;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.WorldNormalVector;96;-190.6016,-627.674;Float;False;False;1;0;FLOAT3;0,0,0;False;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.FresnelNode;32;271.3228,-191.7833;Float;False;Standard;WorldNormal;ViewDir;False;5;0;FLOAT3;0,0,1;False;4;FLOAT3;0,0,0;False;1;FLOAT;0;False;2;FLOAT;1;False;3;FLOAT;5;False;1;FLOAT;0
Node;AmplifyShaderEditor.ColorNode;70;773.2548,378.1182;Float;False;Property;_LightControl;Light Control;12;0;Create;True;0;0;False;0;0,0,0,0;0,0,0,0;False;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.OneMinusNode;71;1050.345,493.8075;Float;False;1;0;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.DotProductOpNode;98;89.79884,-564.2709;Float;True;2;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;59;637.0336,663.0695;Float;False;2;2;0;FLOAT;0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SmoothstepOpNode;99;698.6063,-371.8308;Float;False;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;10;False;1;FLOAT;0
Node;AmplifyShaderEditor.SamplerNode;35;341.8336,-1867.494;Float;True;Property;_Clouds_Shadow;Clouds_Shadow;4;0;Create;True;0;0;False;0;25851468c7c833c4380ae222d7b87084;25851468c7c833c4380ae222d7b87084;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.ColorNode;31;699.7942,-221.9239;Float;False;Property;_RimColor;RimColor;6;0;Create;True;0;0;False;0;0,0.1686275,0.3215686,0;0,0,0,0;False;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RangedFloatNode;85;721.4506,-1669.528;Float;False;Constant;_FractionConstant;Fraction Constant;12;0;Create;True;0;0;False;0;-0.5;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;90;53.08137,-873.0987;Float;False;Property;_NormalIntensity;Normal Intensity;3;0;Create;True;0;0;False;0;0;0;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;30;1036.355,-430.5936;Float;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;95;676.0119,-1395.706;Float;False;Property;_ColorVariationControl;Color Variation Control;1;0;Create;True;0;0;False;0;1;0;-3;3;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;66;1248.781,913.5684;Float;False;Constant;_Lightintensity;Light intensity;11;0;Create;True;0;0;False;0;2;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;65;1291.392,491.9121;Float;False;2;2;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SamplerNode;8;356.1494,-1572.786;Float;True;Property;_PlanetAlbedo;Planet Albedo;0;0;Create;True;0;0;False;0;d63afafb9d8d0994b93c2c9084eeeae7;a1e55f67d0e35784a8be898db0e1ef52;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleContrastOpNode;86;938.9427,-1794.243;Float;False;2;1;COLOR;0,0,0,0;False;0;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.SamplerNode;78;347.0754,-1349.066;Float;True;Property;_PlanetSpecular;Planet Specular;5;0;Create;True;0;0;False;0;a32947a7636ad3f42ae7c7b4755912b1;25851468c7c833c4380ae222d7b87084;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;67;1591.296,698.85;Float;True;2;2;0;COLOR;0,0,0,0;False;1;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.RangedFloatNode;81;400.0928,-1088.236;Float;False;Property;_SpecularIntensity;Specular Intensity;13;0;Create;True;0;0;False;0;0;0;0;5;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;100;1067.866,-239.0089;Float;False;2;2;0;FLOAT;0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SamplerNode;10;393.0088,-987.7887;Float;True;Property;_PlanetNormal;Planet Normal;2;1;[Normal];Create;True;0;0;False;0;None;fe77426814a07ce449c22acd27925b96;True;0;True;bump;Auto;True;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.Vector3Node;93;515.1541,-797.9637;Float;False;Constant;_Vector0;Vector 0;13;0;Create;True;0;0;False;0;0,0,1;0,0,0;0;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.DesaturateOpNode;94;990.2078,-1560.501;Float;False;2;0;FLOAT3;0,0,0;False;1;FLOAT;0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.RangedFloatNode;34;-157.4812,-28.53397;Float;False;Property;_RimBias;Rim Bias;9;0;Create;True;0;0;False;0;0;0;0;10;0;1;FLOAT;0
Node;AmplifyShaderEditor.BlendOpsNode;89;1447.563,-1580.926;Float;False;Overlay;True;2;0;COLOR;0,0,0,0;False;1;FLOAT3;0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleAddOpNode;75;1589.522,-434.1682;Float;False;2;2;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleContrastOpNode;80;721.6113,-1129.501;Float;False;2;1;COLOR;0,0,0,0;False;0;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.LerpOp;91;1266.891,-820.972;Float;False;3;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.StandardSurfaceOutputNode;0;2336.941,-687.6295;Float;False;True;2;Float;;0;0;StandardSpecular;Methexis Planet Shader;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;Back;0;False;-1;0;False;-1;False;1;False;-1;1;False;-1;False;0;Opaque;0.5;True;True;0;False;Opaque;;Geometry;All;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;0;False;-1;False;0;False;-1;255;False;-1;255;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;False;0;32;10;25;True;0.5;True;0;6;False;-1;2;False;-1;0;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;0;0,0,0,0;VertexScale;True;False;Cylindrical;False;Relative;0;Standard (Specular setup);-1;-1;-1;-1;0;False;0;0;False;-1;-1;0;False;-1;0;0;0;False;0.1;False;-1;0;False;-1;16;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT3;0,0,0;False;3;FLOAT3;0,0,0;False;4;FLOAT;0;False;5;FLOAT;0;False;6;FLOAT3;0,0,0;False;7;FLOAT3;0,0,0;False;8;FLOAT;0;False;9;FLOAT;0;False;10;FLOAT;0;False;13;FLOAT3;0,0,0;False;11;FLOAT3;0,0,0;False;12;FLOAT3;0,0,0;False;14;FLOAT4;0,0,0,0;False;15;FLOAT3;0,0,0;False;0
WireConnection;43;0;41;0
WireConnection;43;1;42;0
WireConnection;47;0;43;0
WireConnection;47;1;46;0
WireConnection;50;0;47;0
WireConnection;53;0;50;0
WireConnection;51;1;49;0
WireConnection;54;0;53;0
WireConnection;57;0;51;0
WireConnection;57;1;52;0
WireConnection;32;0;83;0
WireConnection;32;2;33;0
WireConnection;32;3;28;0
WireConnection;71;0;70;0
WireConnection;98;0;96;0
WireConnection;98;1;97;0
WireConnection;59;0;54;0
WireConnection;59;1;57;0
WireConnection;99;0;32;0
WireConnection;30;0;98;0
WireConnection;30;1;99;0
WireConnection;65;0;71;0
WireConnection;65;1;59;0
WireConnection;86;1;35;0
WireConnection;86;0;85;0
WireConnection;67;0;65;0
WireConnection;67;1;66;0
WireConnection;100;0;30;0
WireConnection;100;1;31;0
WireConnection;10;5;90;0
WireConnection;94;0;8;0
WireConnection;94;1;95;0
WireConnection;89;0;86;0
WireConnection;89;1;94;0
WireConnection;75;0;100;0
WireConnection;75;1;67;0
WireConnection;80;1;78;0
WireConnection;80;0;81;0
WireConnection;91;0;10;0
WireConnection;91;1;93;0
WireConnection;0;0;89;0
WireConnection;0;1;91;0
WireConnection;0;2;75;0
WireConnection;0;4;80;0
ASEEND*/
//CHKSM=862D447C02BAE75A0B2175FBDA09D1F806A143FF