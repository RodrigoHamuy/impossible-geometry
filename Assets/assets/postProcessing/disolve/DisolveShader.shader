Shader "MyShaders/PostProcessing/Disolve"
{
	Properties
	{
		_MainTex ("Texture", 2D) = "white" {}
		_D("Displacement", Range(0, 1)) = .3
	}
	SubShader
	{
		// No culling or depth
		Cull Off ZWrite Off ZTest Always

		Pass
		{
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			
			#include "UnityCG.cginc"

			struct appdata
			{
				float4 vertex : POSITION;
				float2 uv : TEXCOORD0;
			};

			struct v2f
			{
				float2 uv : TEXCOORD0;
				float4 vertex : SV_POSITION;
			};

			v2f vert (appdata v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.uv = v.uv;
				return o;
			}
			
			sampler2D _MainTex;
			float _D;

			fixed4 frag (v2f i) : SV_Target
			{

				float speed = 1.;


				float2 p = i.uv;

				for(float i = 1. ; i < 10; i++) {

				 	p.x += _D / i * sin( i * _D * 10. * p.y + _Time.y);
					p.y += _D / i * sin( i * _D * 10. * p.x + _Time.y);

				}

				fixed4 col = tex2D(_MainTex, p);

				return col;
			}
			ENDCG
		}
	}
}
