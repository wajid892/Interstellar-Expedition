// Made with Amplify Shader Editor
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "Methexis Star Shader"
{
	Properties
	{
		_FireSpeed("Fire Speed", Range( 0 , 25)) = 0.3
		_PulsatingTime("Pulsating Time", Range( 0 , 5)) = 1.5
		_FireIntensity("Fire Intensity", Range( 0 , 2)) = 0.7
		_Smoothness("Smoothness", Float) = 0
		_Albedo("Albedo", 2D) = "white" {}
		_Normal("Normal", 2D) = "bump" {}
		_Flames("Flames", 2D) = "white" {}
		_Emission("Emission", 2D) = "white" {}
		_Specular("Specular", 2D) = "white" {}
		[HideInInspector] _texcoord( "", 2D ) = "white" {}
		[HideInInspector] __dirty( "", Int ) = 1
	}

	SubShader
	{
		Tags{ "RenderType" = "Opaque"  "Queue" = "Geometry+0" "IsEmissive" = "true"  }
		Cull Back
		CGPROGRAM
		#include "UnityShaderVariables.cginc"
		#pragma target 3.0
		#pragma surface surf StandardSpecular keepalpha addshadow fullforwardshadows 
		struct Input
		{
			float2 uv_texcoord;
		};

		uniform sampler2D _Normal;
		uniform float4 _Normal_ST;
		uniform sampler2D _Albedo;
		uniform float4 _Albedo_ST;
		uniform sampler2D _Emission;
		uniform float4 _Emission_ST;
		uniform sampler2D _Flames;
		uniform float _FireSpeed;
		uniform float _FireIntensity;
		uniform float _PulsatingTime;
		uniform sampler2D _Specular;
		uniform float4 _Specular_ST;
		uniform float _Smoothness;

		void surf( Input i , inout SurfaceOutputStandardSpecular o )
		{
			float2 uv_Normal = i.uv_texcoord * _Normal_ST.xy + _Normal_ST.zw;
			o.Normal = UnpackNormal( tex2D( _Normal, uv_Normal ) );
			float2 uv_Albedo = i.uv_texcoord * _Albedo_ST.xy + _Albedo_ST.zw;
			o.Albedo = tex2D( _Albedo, uv_Albedo ).rgb;
			float2 uv_Emission = i.uv_texcoord * _Emission_ST.xy + _Emission_ST.zw;
			float2 temp_cast_1 = (_FireSpeed).xx;
			float2 panner72 = ( _Time.x * temp_cast_1 + i.uv_texcoord);
			o.Emission = ( ( tex2D( _Emission, uv_Emission ) * tex2D( _Flames, panner72 ) ) * ( _FireIntensity * ( _SinTime.w + _PulsatingTime ) ) ).rgb;
			float2 uv_Specular = i.uv_texcoord * _Specular_ST.xy + _Specular_ST.zw;
			o.Specular = tex2D( _Specular, uv_Specular ).rgb;
			o.Smoothness = _Smoothness;
			o.Alpha = 1;
		}

		ENDCG
	}
	Fallback "Standard (Specular setup)"
}
/*ASEBEGIN
Version=16700
2586;88.66667;1959;1269;59.62622;1233.574;1.275344;True;True
Node;AmplifyShaderEditor.TextureCoordinatesNode;73;-292.7544,-397.084;Float;False;0;-1;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RangedFloatNode;82;-265.4188,-213.2351;Float;False;Property;_FireSpeed;Fire Speed;0;0;Create;True;0;0;False;0;0.3;0.32;0;25;0;1;FLOAT;0
Node;AmplifyShaderEditor.TimeNode;85;-279.642,-103.862;Float;False;0;5;FLOAT4;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.PannerNode;72;242.759,-388.1201;Float;False;3;0;FLOAT2;0,0;False;2;FLOAT2;0,0;False;1;FLOAT;1;False;1;FLOAT2;0
Node;AmplifyShaderEditor.RangedFloatNode;79;379.9241,238.1124;Float;False;Property;_PulsatingTime;Pulsating Time;1;0;Create;True;0;0;False;0;1.5;1.757;0;5;0;1;FLOAT;0
Node;AmplifyShaderEditor.SinTimeNode;78;539.9241,68.11243;Float;False;0;5;FLOAT4;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SamplerNode;69;508.858,-655.9658;Float;True;Property;_Emission;Emission;7;0;Create;True;0;0;False;0;7562c51ff571f464bb474ac892f2e11d;7562c51ff571f464bb474ac892f2e11d;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RangedFloatNode;76;399.5376,-66.3815;Float;False;Property;_FireIntensity;Fire Intensity;2;0;Create;True;0;0;False;0;0.7;0.719;0;2;0;1;FLOAT;0
Node;AmplifyShaderEditor.SamplerNode;70;511.477,-375.762;Float;True;Property;_Flames;Flames;6;0;Create;True;0;0;False;0;34690f860847e254988cc887da35a98a;34690f860847e254988cc887da35a98a;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleAddOpNode;77;745.9241,108.1124;Float;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;71;1021.697,-474.6631;Float;False;2;2;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;75;978.9241,-64.88757;Float;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SamplerNode;68;1200.761,-894.2682;Float;True;Property;_Albedo;Albedo;4;0;Create;True;0;0;False;0;a1e55f67d0e35784a8be898db0e1ef52;a1e55f67d0e35784a8be898db0e1ef52;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;80;1207.924,-310.8876;Float;False;2;2;0;COLOR;0,0,0,0;False;1;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.SamplerNode;67;1198.666,-680.8721;Float;True;Property;_Normal;Normal;5;0;Create;True;0;0;False;0;fe77426814a07ce449c22acd27925b96;fe77426814a07ce449c22acd27925b96;True;0;True;bump;Auto;True;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SamplerNode;84;1233.703,-183.2403;Float;True;Property;_Specular;Specular;8;0;Create;True;0;0;False;0;7a170cdb7cc88024cb628cfcdbb6705c;7a170cdb7cc88024cb628cfcdbb6705c;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RangedFloatNode;83;1306.292,-436.3438;Float;False;Property;_Smoothness;Smoothness;3;0;Create;True;0;0;False;0;0;0.31;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.StandardSurfaceOutputNode;0;1639.294,-530.9864;Float;False;True;2;Float;;0;0;StandardSpecular;Methexis Star Shader;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;Back;0;False;-1;0;False;-1;False;1;False;-1;1;False;-1;False;0;Opaque;0.5;True;True;0;False;Opaque;;Geometry;All;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;0;False;-1;False;0;False;-1;255;False;-1;255;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;False;0;32;10;25;True;0.5;True;0;6;False;-1;2;False;-1;0;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;0;0,0,0,0;VertexScale;True;False;Cylindrical;False;Relative;0;Standard (Specular setup);-1;-1;-1;-1;0;False;0;0;False;-1;-1;0;False;-1;0;0;0;False;0.1;False;-1;0;False;-1;16;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT3;0,0,0;False;3;FLOAT3;0,0,0;False;4;FLOAT;0;False;5;FLOAT;0;False;6;FLOAT3;0,0,0;False;7;FLOAT3;0,0,0;False;8;FLOAT;0;False;9;FLOAT;0;False;10;FLOAT;0;False;13;FLOAT3;0,0,0;False;11;FLOAT3;0,0,0;False;12;FLOAT3;0,0,0;False;14;FLOAT4;0,0,0,0;False;15;FLOAT3;0,0,0;False;0
WireConnection;72;0;73;0
WireConnection;72;2;82;0
WireConnection;72;1;85;1
WireConnection;70;1;72;0
WireConnection;77;0;78;4
WireConnection;77;1;79;0
WireConnection;71;0;69;0
WireConnection;71;1;70;0
WireConnection;75;0;76;0
WireConnection;75;1;77;0
WireConnection;80;0;71;0
WireConnection;80;1;75;0
WireConnection;0;0;68;0
WireConnection;0;1;67;0
WireConnection;0;2;80;0
WireConnection;0;3;84;0
WireConnection;0;4;83;0
ASEEND*/
//CHKSM=F6D1C7A3D66829B55D0269F9109A45CD43763B08