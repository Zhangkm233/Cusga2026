Shader "Custom/TextureWithTransparency"
{
    Properties
    {
        _MainTex ("Base Texture", 2D) = "white" { }
        _Alpha ("Alpha Transparency", Range(0, 1)) = 1
        _Color ("Tint Color", Color) = (1, 1, 1, 1)
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
            
            v2f vert(appdata v)
            {
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }
            
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
    FallBack "Diffuse"
}
