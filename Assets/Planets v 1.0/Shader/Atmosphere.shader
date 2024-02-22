// Made with Amplify Shader Editor
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "Methexis Atmosphere"
{
	Properties
	{
		_CloudsAlbedo("Clouds Albedo", 2D) = "white" {}
		[Normal]_CloudsNormal("Clouds Normal", 2D) = "bump" {}
		_CloudsNormalIntensity("Clouds Normal Intensity", Range( 0 , 1)) = 0
		_ColorVariationControl("Color Variation Control", Range( -3 , 3)) = 1
		_Atmospherecolor("Atmosphere color", Color) = (1,1,1,0)
		_Atmospherecircle("Atmosphere circle", Range( 0 , 1)) = 0
		_AtmosphereSharpness("Atmosphere Sharpness", Range( 0 , 1)) = 0
		_Atmospheresize("Atmosphere size", Range( 0 , 10)) = 0
		[HideInInspector] _texcoord( "", 2D ) = "white" {}
		[HideInInspector] __dirty( "", Int ) = 1
	}

	SubShader
	{
		Tags{ "RenderType" = "Overlay"  "Queue" = "Overlay+0" "IsEmissive" = "true"  }
		Cull Front
		ZWrite On
		Blend One One
		BlendOp Add
		CGPROGRAM
		#include "UnityStandardUtils.cginc"
		#include "UnityCG.cginc"
		#pragma target 3.0
		#pragma surface surf Lambert keepalpha noshadow noambient nofog vertex:vertexDataFunc 
		struct Input
		{
			float2 uv_texcoord;
			float3 worldPos;
			float3 worldNormal;
			INTERNAL_DATA
		};

		uniform float _Atmospheresize;
		uniform float _CloudsNormalIntensity;
		uniform sampler2D _CloudsNormal;
		uniform float4 _CloudsNormal_ST;
		uniform sampler2D _CloudsAlbedo;
		uniform float4 _CloudsAlbedo_ST;
		uniform float _ColorVariationControl;
		uniform float4 _Atmospherecolor;
		uniform float _AtmosphereSharpness;
		uniform float _Atmospherecircle;

		void vertexDataFunc( inout appdata_full v, out Input o )
		{
			UNITY_INITIALIZE_OUTPUT( Input, o );
			float3 ase_vertexNormal = v.normal.xyz;
			v.vertex.xyz += ( ase_vertexNormal * _Atmospheresize );
		}

		void surf( Input i , inout SurfaceOutput o )
		{
			float2 uv_CloudsNormal = i.uv_texcoord * _CloudsNormal_ST.xy + _CloudsNormal_ST.zw;
			float3 lerpResult59 = lerp( UnpackScaleNormal( tex2D( _CloudsNormal, uv_CloudsNormal ), _CloudsNormalIntensity ) , float3(0,0,1) , float3( 0,0,0 ));
			o.Normal = lerpResult59;
			float2 uv_CloudsAlbedo = i.uv_texcoord * _CloudsAlbedo_ST.xy + _CloudsAlbedo_ST.zw;
			float3 desaturateInitialColor64 = tex2D( _CloudsAlbedo, uv_CloudsAlbedo ).rgb;
			float desaturateDot64 = dot( desaturateInitialColor64, float3( 0.299, 0.587, 0.114 ));
			float3 desaturateVar64 = lerp( desaturateInitialColor64, desaturateDot64.xxx, _ColorVariationControl );
			o.Albedo = desaturateVar64;
			float3 ase_worldPos = i.worldPos;
			#if defined(LIGHTMAP_ON) && UNITY_VERSION < 560 //aseld
			float3 ase_worldlightDir = 0;
			#else //aseld
			float3 ase_worldlightDir = normalize( UnityWorldSpaceLightDir( ase_worldPos ) );
			#endif //aseld
			float3 ase_worldNormal = WorldNormalVector( i, float3( 0, 0, 1 ) );
			float dotResult16 = dot( ase_worldlightDir , ase_worldNormal );
			float3 ase_worldViewDir = normalize( UnityWorldSpaceViewDir( ase_worldPos ) );
			float dotResult26 = dot( ase_worldNormal , ase_worldViewDir );
			float temp_output_32_0 = pow( (0.0 + (-dotResult26 - 0.0) * (( 15.0 * _AtmosphereSharpness ) - 0.0) / (1.0 - 0.0)) , ( 15.0 * _AtmosphereSharpness ) );
			float lerpResult41 = lerp( ( dotResult16 * temp_output_32_0 ) , temp_output_32_0 , _Atmospherecircle);
			float clampResult43 = clamp( lerpResult41 , 0.0 , 1.0 );
			o.Emission = ( _Atmospherecolor * clampResult43 ).rgb;
			o.Alpha = 1;
		}

		ENDCG
	}
}
/*ASEBEGIN
Version=16700
6.666667;13.33333;2498;1359;311.1747;333.8914;1;True;True
Node;AmplifyShaderEditor.CommentaryNode;46;-967.4506,101.3996;Float;False;1475.601;411.5002;Fresnel ;10;25;24;29;28;26;27;32;33;30;51;;1,1,1,1;0;0
Node;AmplifyShaderEditor.WorldNormalVector;24;-917.4506,151.3996;Float;False;False;1;0;FLOAT3;0,0,0;False;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.ViewDirInputsCoordNode;25;-910.1507,301.0996;Float;False;World;False;0;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.RangedFloatNode;53;-553.6279,601.1993;Float;False;Property;_AtmosphereSharpness;Atmosphere Sharpness;6;0;Create;True;0;0;False;0;0;0.165;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.DotProductOpNode;26;-669.5507,153.0999;Float;True;2;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;30;-575.6974,404.5526;Float;False;Constant;_Atmosphereintensity;Atmosphere intensity;3;0;Create;True;0;0;False;0;15;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;29;-456.3501,316.8998;Float;False;Constant;_Float2;Float 2;1;0;Create;True;0;0;False;0;1;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;28;-459.3501,242.8998;Float;False;Constant;_Float1;Float 1;1;0;Create;True;0;0;False;0;0;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.NegateNode;51;-442.4108,140.8691;Float;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;33;18.95053,251.2991;Float;False;Constant;_Atmospherepower;Atmosphere power;4;0;Create;True;0;0;False;0;15;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;54;-296.8285,445.5231;Float;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.CommentaryNode;47;-49.14934,-401.8528;Float;False;541.457;360.2413;Light Mask WS;3;16;40;14;;1,1,1,1;0;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;52;282.4266,594.6926;Float;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.WorldNormalVector;14;2.462931,-196.7618;Float;False;False;1;0;FLOAT3;0,0,0;False;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.TFHCRemapNode;27;-244.6504,175.3999;Float;True;5;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;1;False;3;FLOAT;0;False;4;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.WorldSpaceLightDirHlpNode;40;0.8506591,-351.8528;Float;False;False;1;0;FLOAT;0;False;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.PowerNode;32;328.1508,184.2999;Float;False;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.DotProductOpNode;16;257.3077,-294.6115;Float;True;2;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;1;FLOAT;0
Node;AmplifyShaderEditor.CommentaryNode;48;622.9507,-180.2;Float;False;562.6014;474.4;Control over either fresnel or directional light;3;42;41;37;;1,1,1,1;0;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;37;699.3507,-130.2;Float;True;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;42;672.9507,179.1999;Float;False;Property;_Atmospherecircle;Atmosphere circle;5;0;Create;True;0;0;False;0;0;0.362;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.CommentaryNode;49;1253.751,-203.5001;Float;False;450.9996;382.0001;Color control;3;43;34;35;;1,1,1,1;0;0
Node;AmplifyShaderEditor.CommentaryNode;45;1028.8,393.4002;Float;False;627.8995;345.7003;Vertex offset;3;9;8;7;;1,1,1,1;0;0
Node;AmplifyShaderEditor.LerpOp;41;1001.552,5.299968;Float;False;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;56;271.5269,-563.9299;Float;False;Property;_CloudsNormalIntensity;Clouds Normal Intensity;2;0;Create;True;0;0;False;0;0;0;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.SamplerNode;58;611.4551,-678.6201;Float;True;Property;_CloudsNormal;Clouds Normal;1;1;[Normal];Create;True;0;0;False;0;None;fe77426814a07ce449c22acd27925b96;True;0;True;bump;Auto;True;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SamplerNode;62;453.5352,-1110.934;Float;True;Property;_CloudsAlbedo;Clouds Albedo;0;0;Create;True;0;0;False;0;None;a1e55f67d0e35784a8be898db0e1ef52;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.ColorNode;35;1303.751,-153.5;Float;False;Property;_Atmospherecolor;Atmosphere color;4;0;Create;True;0;0;False;0;1,1,1,0;0,0.2881768,1,0;False;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.NormalVertexDataNode;8;1156.401,443.4003;Float;False;0;5;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RangedFloatNode;63;475.3977,-878.8536;Float;False;Property;_ColorVariationControl;Color Variation Control;3;0;Create;True;0;0;False;0;1;0;-3;3;0;1;FLOAT;0
Node;AmplifyShaderEditor.Vector3Node;57;733.6004,-488.7948;Float;False;Constant;_Vector0;Vector 0;13;0;Create;True;0;0;False;0;0,0,1;0,0,0;0;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.RangedFloatNode;7;1078.8,602.9006;Float;False;Property;_Atmospheresize;Atmosphere size;7;0;Create;True;0;0;False;0;0;0.01;0;10;0;1;FLOAT;0
Node;AmplifyShaderEditor.ClampOpNode;43;1319.151,22.49998;Float;False;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.LerpOp;59;1390.399,-427.6821;Float;False;3;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.DesaturateOpNode;64;969.5936,-1098.649;Float;False;2;0;FLOAT3;0,0,0;False;1;FLOAT;0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;34;1535.75,-17.80019;Float;False;2;2;0;COLOR;0,0,0,0;False;1;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;9;1421.7,486.1007;Float;True;2;2;0;FLOAT3;0,0,0;False;1;FLOAT;0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.StandardSurfaceOutputNode;0;2059,87.49983;Float;False;True;2;Float;;0;0;Lambert;Methexis Atmosphere;False;False;False;False;True;False;False;False;False;True;False;False;False;False;False;False;False;False;False;False;False;Front;1;False;-1;0;False;-1;False;100000;False;-1;100000;False;-1;False;0;Custom;0;True;False;0;True;Overlay;;Overlay;All;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;0;False;-1;False;100;False;-1;255;False;-1;255;False;-1;2;False;-1;1;False;-1;1;False;-1;1;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;False;0;4;10;25;False;0.5;False;4;1;False;-1;1;False;-1;0;0;False;-1;0;False;-1;1;False;-1;1;False;-1;0;False;0;0,0,0,0;VertexOffset;True;False;Cylindrical;False;Relative;0;;8;-1;-1;-1;0;False;0;0;False;-1;-1;0;False;-1;0;0;0;False;0.1;False;-1;0;False;-1;15;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT3;0,0,0;False;3;FLOAT;0;False;4;FLOAT;0;False;6;FLOAT3;0,0,0;False;7;FLOAT3;0,0,0;False;8;FLOAT;0;False;9;FLOAT;0;False;10;FLOAT;0;False;13;FLOAT3;0,0,0;False;11;FLOAT3;0,0,0;False;12;FLOAT3;0,0,0;False;14;FLOAT4;0,0,0,0;False;15;FLOAT3;0,0,0;False;0
WireConnection;26;0;24;0
WireConnection;26;1;25;0
WireConnection;51;0;26;0
WireConnection;54;0;30;0
WireConnection;54;1;53;0
WireConnection;52;0;33;0
WireConnection;52;1;53;0
WireConnection;27;0;51;0
WireConnection;27;1;28;0
WireConnection;27;2;29;0
WireConnection;27;3;28;0
WireConnection;27;4;54;0
WireConnection;32;0;27;0
WireConnection;32;1;52;0
WireConnection;16;0;40;0
WireConnection;16;1;14;0
WireConnection;37;0;16;0
WireConnection;37;1;32;0
WireConnection;41;0;37;0
WireConnection;41;1;32;0
WireConnection;41;2;42;0
WireConnection;58;5;56;0
WireConnection;43;0;41;0
WireConnection;59;0;58;0
WireConnection;59;1;57;0
WireConnection;64;0;62;0
WireConnection;64;1;63;0
WireConnection;34;0;35;0
WireConnection;34;1;43;0
WireConnection;9;0;8;0
WireConnection;9;1;7;0
WireConnection;0;0;64;0
WireConnection;0;1;59;0
WireConnection;0;2;34;0
WireConnection;0;11;9;0
ASEEND*/
//CHKSM=A95BAFB212C0C98D2ABF0D476A84F3DF9D759FC4