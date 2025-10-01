Shader "Custom/WheatMovement"
{
    Properties
    {
        _MainTex ("Base Texture", 2D) = "white" { }
        _Alpha ("Alpha Transparency", Range(0, 1)) = 1
        _Color ("Tint Color", Color) = (1, 1, 1, 1)
        _Speed ("Wave Speed", Float) = 1.0
        _MaxAmplitude ("Max Wave Amplitude", Float) = 0.1
        _MinHeight ("Minimum Height", Float) = 0.0
        _MaxHeight ("Maximum Height", Float) = 10.0
        _GroundHeight ("Ground Height", Float) = 0.0 // 设置底部高度
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
            float _Speed;
            float _MaxAmplitude;
            float _MinHeight;
            float _MaxHeight;
            float _GroundHeight; // 底部高度

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };
            
            struct v2f
            {
                float4 pos : SV_POSITION;
                float2 uv : TEXCOORD0;
            };

            // 顶点着色器：根据世界空间高度来修改波动
            v2f vert(appdata v)
            {
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);

                // 获取当前顶点的世界空间位置
                float3 worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;

                // 使用世界空间的Y坐标来控制波动
                float heightFactor = (worldPos.y - _MinHeight) / (_MaxHeight - _MinHeight);
                heightFactor = clamp(heightFactor, 0.0, 1.0); // 保证值在0和1之间
                float adjustedAmplitude = _MaxAmplitude * (1.0 - heightFactor);  // 离地越近，幅度越小

                // 确保最底部的物体不动
                if (worldPos.y <= _GroundHeight) {
                    adjustedAmplitude = 0.0;  // 最底部的物体波动幅度为0
                }

                // 使用时间控制波动，确保同一高度的物体一起移动
                float time = _Time.y * _Speed;
                float wave = sin(worldPos.y * 0.1 + time) * adjustedAmplitude;  // 波动控制

                // 修改UV坐标，模拟纹理随麦穗摇动
                o.uv = v.uv + float2(wave, 0);

                return o;
            }

            // 片段着色器：将修改后的UV坐标应用到纹理采样
            half4 frag(v2f i) : SV_Target
            {
                // 获取纹理颜色
                half4 texColor = tex2D(_MainTex, i.uv);
                texColor *= _Color; // 调整颜色

                // 使用透明度控制透明度值
                texColor.a *= _Alpha;

                // 返回最终颜色
                return texColor;
            }
            ENDCG
        }
    }

    Fallback "Diffuse"
}
