Shader "Custom/TextureWithTransparency"
{
    Properties
    {
        _MainTex ("Base Texture", 2D) = "white" { }
        _Alpha ("Alpha Transparency", Range(0, 1)) = 1
        _Color ("Tint Color", Color) = (1, 1, 1, 1)
        _EnvStrength ("Environment Strength", Range(0, 2)) = 0.1
    }

    SubShader
    {
        Tags { "Queue" = "Overlay" }
        
        Pass
        {
            // 混合模式设置，使用源Alpha和1减去源Alpha
            Blend SrcAlpha OneMinusSrcAlpha

            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"
            
            // Properties
            sampler2D _MainTex;
            float _Alpha;
            float4 _Color;
            float _EnvStrength;

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
                float3 normal : NORMAL;
            };
            
            struct v2f
            {
                float4 pos : SV_POSITION;
                float2 uv : TEXCOORD0;
                float3 worldPos : TEXCOORD1;
                float3 worldNormal : TEXCOORD2;
            };
            
            v2f vert(appdata v)
            {
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                o.worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;
                o.worldNormal = UnityObjectToWorldNormal(v.normal);
                return o;
            }
            
            half4 frag(v2f i) : SV_Target
            {
                // 获取纹理颜色
                half4 texColor = tex2D(_MainTex, i.uv);
                texColor *= _Color; // 调整颜色
                
                // 使用透明度控制透明度值
                texColor.a *= _Alpha;
                
                // 从场景反射探针 / HDRI 天空盒取样
                // 计算视线方向与反射向量（世界空间）
                float3 viewDirWS = normalize(_WorldSpaceCameraPos.xyz - i.worldPos);
                float3 nWS = normalize(i.worldNormal);
                float3 reflWS = reflect(-viewDirWS, nWS);

                // 采样全局反射贴图（由反射探针/天空盒提供）
                // unity_SpecCube0 与 unity_SpecCube0_HDR 由 Unity 提供
                #if defined(UNITY_USE_NATIVE_HDR)
                    half3 envColor = UNITY_SAMPLE_TEXCUBE(unity_SpecCube0, reflWS).rgb;
                #else
                    half4 encoded = UNITY_SAMPLE_TEXCUBE(unity_SpecCube0, reflWS);
                    half3 envColor = DecodeHDR(encoded, unity_SpecCube0_HDR);
                #endif

                // 按强度叠加环境颜色
                texColor.rgb += envColor * _EnvStrength;

                // 返回最终颜色
                return texColor;
            }
            ENDCG
        }
    }
    FallBack "Diffuse"
}
