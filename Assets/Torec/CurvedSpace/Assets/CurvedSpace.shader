// Curved Space Shader 
// Torec Studio 2017
// torec.studio@gmail.com
//
// Vertex transform pipeline:
//		additional scale (shader "_Scale" variable)
//		inverse projection into 4D unit sphere
//		4D rotation (shader "_Rotation" variable)
//		stereographic projection back to 3d space

Shader "Torec/CurvedSpace"
{
	Properties
	{
		_Albedo("Albedo", Color) = (1, 1, 1, 1)

		[MaterialToggle] _UseTexture("Use Texture", Int) = 0
		_MainTex("Texture", 2D) = "" {}
		
		[MaterialToggle] _UseBumpMap("Use Normal Map", Int) = 0
		_BumpMap("Normal Map", 2D) = "" {}

		_FogColor("Fog color", Color) = (.5, .5, .5, 1)
		_FogShining("Fog", Range(0, 1)) = .5

		[HideInInspector] _Scale("Scale", Float) = 1.0
	}

	SubShader
	{
		Tags { "RenderType" = "Opaque" }

		CGPROGRAM

	#pragma surface surf Lambert finalcolor:finalcolor vertex:vert

	// geometry
	inline float4 stereo(float4 v) { // stereographic projection: 4d sphere -> 3d space
		v.xyz /= 1 - v.w;
		v.w = 1;
		return v;
	}
	inline float4 stereo1(float4 v) { // stereographic projection inverse function: 3d space -> 4d sphere
		v.xyz /= v.w;
		float s = dot(v.xyz, v.xyz);
		v.xyz *= 2;
		v.w = s - 1;
		v /= s + 1;
		return v;
	}

	//==============================================

	uniform fixed4 _Albedo;

	uniform float _Scale;
	uniform float4x4 _Rotation;  // 4D rotation	

	uniform int _UseTexture; // 1 if _MainTex set
	uniform sampler2D _MainTex;

#define usebump
#ifdef usebump
	uniform int _UseBumpMap; // 1 if _BumpMap set
	uniform sampler2D _BumpMap;
#endif

	uniform fixed4 _FogColor;
	uniform half _FogShining; // parabolic 0..1..0

	struct Input {
		float2 uv_MainTex;
		float2 uv_BumpMap;
		half fog;
	};

	void rotate(inout appdata_full d) {
		float4 v = d.vertex;
		float4 vn = v + float4(d.normal * 0.001, 0.0);

		v = stereo1(v);
		vn = stereo1(vn);

		v = mul(_Rotation, v);
		vn = mul(_Rotation, vn);

		v = stereo(v);
		vn = stereo(vn);

		d.vertex = v;
		d.normal = normalize(vn - v).xyz;
	}

#ifdef usebump
	void rotate2(inout appdata_full d) { // also rotate tangents for normal map
		float4 v = d.vertex;
		float4 vn = v + float4(d.normal      * 0.001, 0.0);
		float4 vt = v + float4(d.tangent.xyz * 0.001, 0.0);

		v = stereo1(v);
		vn = stereo1(vn);
		vt = stereo1(vt);

		v = mul(_Rotation, v);
		vn = mul(_Rotation, vn);
		vt = mul(_Rotation, vt);

		v = stereo(v);
		vn = stereo(vn);
		vt = stereo(vt);

		d.vertex = v;
		d.normal = normalize(vn - v).xyz;
		d.tangent = float4(normalize(vt - v).xyz, d.tangent.w);
	}
#endif

	void vert(inout appdata_full d, out Input data)
	{
		d.vertex.xyz *= _Scale;

#ifdef usebump
		if (_UseBumpMap) {
			rotate2(d);
		}
		else
#endif
		{
			rotate(d);
		}

		// fog
		UNITY_INITIALIZE_OUTPUT(Input, data);
		half dist = 0.5 + _FogShining * _FogShining * 5;
		data.fog = clamp(d.vertex.z * dist + _FogShining * 2, 0, 1);
	}

	void surf(Input IN, inout SurfaceOutput o) {
		if (_UseTexture) {
			o.Albedo = tex2D(_MainTex, IN.uv_MainTex).rgb;
		} else {
			o.Albedo = _Albedo;
		}
#ifdef usebump
		if (_UseBumpMap) {
			o.Normal = UnpackNormal(tex2D(_BumpMap, IN.uv_BumpMap));
		} else {
			o.Normal = float3(0, 0, 1);
		}
#endif
	}

	void finalcolor(Input IN, SurfaceOutput o, inout fixed4 col) {
		col.rgb = lerp(col.rgb, _FogColor.rgb, IN.fog);
	}

		ENDCG
	}

	//Fallback Off
	Fallback "Diffuse"
}