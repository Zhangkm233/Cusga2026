Shader "Custom/ToonCutoutFlat"
{
	Properties
	{
		_MainTex ("Base Texture", 2D) = "white" {}
		_Color ("Tint Color", Color) = (1, 1, 1, 1)
		_Alpha ("Alpha", Range(0,1)) = 1
		_Cutoff ("Alpha Cutoff", Range(0,1)) = 0.35
		_Saturation ("Color Saturation", Range(0, 2)) = 1.2
		_ShadowSteps ("Toon Shadow Steps", Range(1, 8)) = 3
		_AmbientColor ("Ambient Color", Color) = (0.65, 0.72, 0.78, 1)
		_LightDir ("Fake Sun Direction (world)", Vector) = (0.2, 1, 0.1, 0)
		_LightColor ("Fake Sun Color", Color) = (1, 0.96, 0.88, 1)
		_EdgeColor ("Edge Tint Color", Color) = (0.85, 0.9, 1, 1)
		_EdgeIntensity ("Edge Intensity", Range(0, 1)) = 0.22
		_FresnelPower ("Edge Fresnel Power", Range(1, 5)) = 2
	}

	SubShader
	{
		Tags { "Queue" = "AlphaTest" "RenderType" = "TransparentCutout" "IgnoreProjector" = "True" }
		Cull Back
		ZWrite On
		AlphaToMask On
		Blend SrcAlpha OneMinusSrcAlpha

		Pass
		{
			Tags { "LightMode" = "SRPDefaultUnlit" }
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#pragma target 3.0
			#include "UnityCG.cginc"

			sampler2D _MainTex;
			float4 _MainTex_ST;
			float4 _Color;
			float _Alpha;
			float _Cutoff;
			float _Saturation;
			float _ShadowSteps;
			float4 _AmbientColor;
			float4 _LightDir;
			float4 _LightColor;
			float4 _EdgeColor;
			float _EdgeIntensity;
			float _FresnelPower;

			struct appdata
			{
				float4 vertex : POSITION;
				float2 uv : TEXCOORD0;
			};

			struct v2f
			{
				float4 pos : SV_POSITION;
				float2 uv : TEXCOORD0;
				float3 worldPos : TEXCOORD1;
			};

			v2f vert(appdata v)
			{
				v2f o;
				o.pos = UnityObjectToClipPos(v.vertex);
				o.uv = TRANSFORM_TEX(v.uv, _MainTex);
				o.worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;
				return o;
			}

			float3 adjustSaturation(float3 color, float saturation)
			{
				float luma = dot(color, float3(0.299, 0.587, 0.114));
				return lerp(float3(luma, luma, luma), color, saturation);
			}

			half4 frag(v2f i) : SV_Target
			{
				half4 baseCol = tex2D(_MainTex, i.uv) * _Color;
				clip(baseCol.a * _Alpha - _Cutoff);
				baseCol.rgb = adjustSaturation(baseCol.rgb, _Saturation);

				// 平面法线（低模块面感）
				float3 dpdx = ddx(i.worldPos);
				float3 dpdy = ddy(i.worldPos);
				float3 flatNormal = normalize(cross(dpdy, dpdx));

				// Toon 分层主光（自定义方向，避免管线差异）
				float3 lightDir = normalize(_LightDir.xyz);
				float ndl = saturate(dot(flatNormal, lightDir));
				float steps = max(2.0, _ShadowSteps);
				ndl = (_ShadowSteps <= 1.0) ? ndl : floor(ndl * steps) / (steps - 1.0);
				float3 lit = _AmbientColor.rgb + ndl * _LightColor.rgb;

				// 柔和边缘晕染（凯尔特插画氛围）
				float3 viewDir = normalize(_WorldSpaceCameraPos.xyz - i.worldPos);
				float fres = pow(1.0 - saturate(dot(flatNormal, viewDir)), _FresnelPower);
				float3 edge = _EdgeColor.rgb * fres * _EdgeIntensity;

				return half4(baseCol.rgb * lit + edge, 1);
			}
			ENDCG
		}
	}
	FallBack "TransparentCutout"
}


