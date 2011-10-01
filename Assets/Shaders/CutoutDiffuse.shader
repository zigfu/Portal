Shader "Portal/Transparent Diffuse With Cutoff" {
Properties {
	_Color ("Main Color", Color) = (1,1,1,1)
	_MainTex ("Base (RGB) Trans (A)", 2D) = "white" {}
	_Cutoff ("DistanceCovered", Range(0,1)) = 0.5
	_Direction ("FillDirection", Vector) = (1,0,0,0)
	_StartPoint ("StartPoint", Vector) = (0,0,0,0)
}

SubShader {
	Tags {"Queue"="Transparent" "IgnoreProjector"="True" "RenderType"="Transparent"}
	LOD 200

CGPROGRAM
#pragma surface surf Lambert alpha

sampler2D _MainTex;
fixed4 _Color;
float4 _Direction;
float4 _StartPoint;
float _Cutoff;

struct Input {
	float2 uv_MainTex;
};

void surf (Input IN, inout SurfaceOutput o) {
	float2 cutoffPoint = _StartPoint.xy + _Direction.xy * _Cutoff;
	float2 delta = IN.uv_MainTex - cutoffPoint;
	clip(-dot(delta, _Direction.xy));
	// in case we aren't clipped, do the texture sampling, etc.
	fixed4 c = tex2D(_MainTex, IN.uv_MainTex) * _Color;
	o.Albedo = c.rgb;
	o.Alpha = c.a;	
}
ENDCG
}

Fallback "Transparent/VertexLit"
}
