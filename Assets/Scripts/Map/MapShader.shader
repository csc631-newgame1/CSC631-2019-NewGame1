Shader "Unlit/MapShader" {

	Properties {
		_MainTex ("Texture", 2D) = "white" {}
	}

    SubShader {
	
		Tags {"RenderType"="Opaque"}
		
        Pass {
		
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
			#pragma multi_compile_fog
			
            #include "UnityCG.cginc"
			#include "UnityLightingCommon.cginc"
			
			sampler2D _MainTex;
			
			struct i2v {
				float4 pos : POSITION;
				float2 uv : TEXCOORD0;
				half3 normal : NORMAL;
			};
         
            struct v2f {
                float4 pos : POSITION;
                float2 uv : TEXCOORD0;
				float4 color : COLOR;
				UNITY_FOG_COORDS(1)
            };
			
			float4 _MainTex_ST;
            
            v2f vert (i2v v)
            {
                v2f o;
                o.pos = UnityObjectToClipPos(v.pos);
				o.uv = TRANSFORM_TEX(v.uv, _MainTex);
				
				half3 world_normal = UnityObjectToWorldNormal(v.normal);
				half intensity = max(0.4, dot(world_normal, _WorldSpaceLightPos0.xyz));
                o.color = float4(1, 1, 1, 1) * intensity;
				
				UNITY_TRANSFER_FOG(o, o.pos);
                return o;
            }

            float4 frag (v2f i) : SV_Target 
			{
				i.uv.x = clamp(i.uv.x + (_SinTime[2] / 3.0f), 0.01f, 0.99f);
				float4 col = tex2D(_MainTex, i.uv) * i.color;
				UNITY_APPLY_FOG(i.fogCoord, col);
				col.a = 1;
				return col;
			}
            ENDCG
        }
    } 
	FallBack "Diffuse"
}
