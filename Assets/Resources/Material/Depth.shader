Shader "Custom/Depth"
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
                //sampler에서 제공하는 depthnormal값을 받아온다.

                float3 normal;
                float depth;
                DecodeDepthNormal(depthnormal, depth, normal);
                //해당 depthnormal값을 depth와 normal으로 분리한다.
                depth = depth*10; //depth값을 원하는 배율에 맞게 조정하고
                float alpha = depth > 5 ? 0 : 1; //배경을 제외하기 위해 알파값을 설정한다.
                return float4(depth,depth,depth,alpha);//그레이스케일에 알파값을 적용하여 반환한다.
            }
            ENDCG
        }
    }
}