Shader "Custom/laser" {
Properties {
	_Color ("Main Color", Color) = (1,1,1,1)
	_MainTex ("Base (RGB) Trans (A)", 2D) = "white" {}
	_Alpha ("Transparency", range(0, 1)) = 1
}

SubShader {
	Tags {"Queue"="Transparent+2" "IgnoreProjector"="True" "RenderType"="Transparent"}
	LOD 200
	

  Pass {
		Blend SrcAlpha OneMinusSrcAlpha
		CGPROGRAM
		#pragma vertex vert
		#pragma fragment frag
		#include "UnityCG.cginc"
		
		sampler2D _MainTex;
		fixed4 _Color;
		float4 _MainTex_ST;
		float _Alpha;

		
		struct v2f {
		    float4 pos : SV_POSITION;
		    float2  uv : TEXCOORD0;
		};
		
		v2f vert (appdata_base v)
		{
		    v2f o;
		    o.pos = mul (UNITY_MATRIX_MVP, v.vertex);
		    o.uv = TRANSFORM_TEX (v.texcoord, _MainTex);
		    return o;
		}
		
		half4 frag (v2f i) : COLOR
		{
		    half4 texcol = tex2D (_MainTex, i.uv) * _Color;
		    texcol.a *= _Alpha;
		    return texcol;
		}
		
		ENDCG
		}
	}

Fallback "Transparent/VertexLit"
}
