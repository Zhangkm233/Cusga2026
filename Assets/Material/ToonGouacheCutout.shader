Shader "Custom/ToonGouacheCutout"
{
	Properties
	{
		_MainTex ("Base Texture", 2D) = "white" {}
		_Color ("Tint Color", Color) = (1, 1, 1, 1)
		_Alpha ("Alpha", Range(0,1)) = 1
		_Cutoff ("Alpha Cutoff", Range(0,1)) = 0.4
		// 纸绘/水彩风格控制
		_RampTex ("Light Ramp (1xN)", 2D) = "white" {}
		_RampIntensity ("Ramp Intensity", Range(0, 2)) = 1
		_PaperTex ("Paper Grain", 2D) = "gray" {}
		_PaperScale ("Paper Scale", Range(0.1, 10)) = 2
		_PaperStrength ("Paper Strength", Range(0, 1)) = 0.25
		_Saturation ("Saturation", Range(0, 2)) = 0.95
		// 光照（自定义方向，避免依赖管线内置）
		_LightDir ("Fake Sun Direction (world)", Vector) = (0.25, 0.9, 0.15, 0)
		_LightColor ("Fake Sun Color", Color) = (1, 0.96, 0.88, 1)
		_AmbientColor ("Ambient Color", Color) = (0.72, 0.78, 0.82, 1)
		// 边线（轻度墨线）
		_OutlineColor ("Outline Color", Color) = (0.1, 0.12, 0.13, 1)
		_OutlineStrength ("Outline Strength", Range(0, 1)) = 0.25
		_OutlinePower ("Outline Power", Range(1, 6)) = 3
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

			sampler2D _MainTex; float4 _MainTex_ST;
			sampler2D _RampTex;
			sampler2D _PaperTex;
			float4 _Color; float _Alpha; float _Cutoff;
			float _RampIntensity; float _PaperScale; float _PaperStrength; float _Saturation;
			float4 _LightDir; float4 _LightColor; float4 _AmbientColor;
			float4 _OutlineColor; float _OutlineStrength; float _OutlinePower;

			struct appdata { float4 vertex:POSITION; float2 uv:TEXCOORD0; };
			struct v2f { float4 pos:SV_POSITION; float2 uv:TEXCOORD0; float3 worldPos:TEXCOORD1; };

			v2f vert(appdata v)
			{
				v2f o; o.pos = UnityObjectToClipPos(v.vertex); o.uv = TRANSFORM_TEX(v.uv, _MainTex); o.worldPos = mul(unity_ObjectToWorld, v.vertex).xyz; return o;
			}

			float3 adjustSaturation(float3 c, float s)
			{ float l = dot(c, float3(0.299, 0.587, 0.114)); return lerp(float3(l,l,l), c, s); }

			half4 frag(v2f i):SV_Target
			{
				half4 albedo = tex2D(_MainTex, i.uv) * _Color;
				clip(albedo.a * _Alpha - _Cutoff);
				albedo.rgb = adjustSaturation(albedo.rgb, _Saturation);

				// 低模平面法线
				float3 dpdx = ddx(i.worldPos); float3 dpdy = ddy(i.worldPos);
				float3 n = normalize(cross(dpdy, dpdx));

				// Ramp 映射的光照
				float ndl = saturate(dot(n, normalize(_LightDir.xyz)));
				float rampU = saturate(ndl);
				float3 ramp = tex2D(_RampTex, float2(rampU, 0.5)).rgb * _RampIntensity;
				float3 lit = _AmbientColor.rgb + ramp * _LightColor.rgb;

				// 纸纹叠加（乘/覆盖混合）
				float2 paperUV = i.uv * _PaperScale;
				float3 paper = tex2D(_PaperTex, paperUV).rrr; // 单通道
				float3 paperMod = lerp(1.0.xxx, paper, _PaperStrength);

				// 轻墨线（基于菲涅耳）
				float3 viewDir = normalize(_WorldSpaceCameraPos.xyz - i.worldPos);
				float fres = pow(1.0 - saturate(dot(n, viewDir)), _OutlinePower);
				float3 outline = _OutlineColor.rgb * fres * _OutlineStrength;

				float3 finalRGB = albedo.rgb * lit * paperMod + outline;
				return half4(finalRGB, 1);
			}
			ENDCG
		}
	}
	FallBack "TransparentCutout"
}



