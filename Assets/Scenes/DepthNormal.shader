Shader "Custom/DepthNormal"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
    }
    SubShader
    {
        Tags
        {
            "RenderType"="Opaque"
        }
        LOD 100

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

            sampler2D _MainTex;
            sampler2D _CameraDepthNormalsTexture;

            v2f vert(appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }

            fixed4 frag(v2f i) : SV_Target
            {
                float4 depthnormal = tex2D(_CameraDepthNormalsTexture, i.uv);

                float3 normal;
                float depth;
                DecodeDepthNormal(depthnormal, depth, normal);
                float alpha = depth > 0 ? 0 : 1;
                // depth를 0-1 사이의 grayscale로 변환
                return float4(depthnormal.xxx, alpha);
            }
            ENDCG
        }
    }
}