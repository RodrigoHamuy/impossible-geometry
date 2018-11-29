Shader "Lit/Diffuse With Ambient" {

	Properties {
			_NormalColor ("Normal color", Color) = (1,1,1,1)
			_ObjectColor("Object color", Color) = (.5,.5,.5,1)
	}

  SubShader {

    Pass {

      Tags {"LightMode"="ForwardBase"}

      CGPROGRAM
      #pragma vertex vert
      #pragma fragment frag
      #include "UnityCG.cginc"
      #include "UnityLightingCommon.cginc"

      struct v2f {
        float2 uv : TEXCOORD0;
        fixed4 diff : COLOR0;
        float4 vertex : SV_POSITION;
      };

      v2f vert (appdata_base v) {

        v2f o;
        o.vertex = UnityObjectToClipPos(v.vertex);
        o.uv = v.texcoord;
        half3 worldNormal = UnityObjectToWorldNormal(v.normal);
        half nl = max(0, dot(worldNormal, _WorldSpaceLightPos0.xyz));
        o.diff = nl * _LightColor0;

        // the only difference from previous shader:
        // in addition to the diffuse lighting from the main light,
        // add illumination from ambient or light probes
        // ShadeSH9 function from UnityCG.cginc evaluates it,
        // using world space normal
        o.diff.rgb += ShadeSH9(half4(worldNormal,1));
        return o;

      }

      fixed4 _ObjectColor;

      fixed4 frag (v2f i) : SV_Target {

        fixed4 col = _ObjectColor;
        col *= i.diff;
        return col;

      }

      ENDCG
    }
  }
}
