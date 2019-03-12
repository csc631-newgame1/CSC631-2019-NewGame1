Shader "Unlit/MapShader" {

	Properties {
		_MainTex ("Texture", 2D) = "white" {}
		_FluidTex ("Fluid Detail", 2D) = "white" {}
	}

    SubShader {
	
		Tags {"RenderType"="Opaque" "LightMode"="ForwardBase"}
		
        Pass {
		
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
			#pragma multi_compile_fog
			
            #include "UnityCG.cginc"
			#include "UnityLightingCommon.cginc"
			
			sampler2D _MainTex;
			sampler2D _FluidTex;
			
			struct i2v {
				float4 pos : POSITION;
				float2 uv1 : TEXCOORD0;
				float2 uv2 : TEXCOORD1;
				half3 normal : NORMAL;
			};
         
            struct v2f {
                float4 pos : POSITION;
                float2 uv1 : TEXCOORD0;
				float2 uv2 : TEXCOORD1;
				float4 color : COLOR;
				UNITY_FOG_COORDS(2)
            };
			
			float4 _MainTex_ST;
			float4 _FluidTex_ST;
            
            v2f vert (i2v v)
            {
                v2f o;
                o.pos = UnityObjectToClipPos(v.pos);
				o.uv1 = TRANSFORM_TEX(v.uv1, _MainTex);
				o.uv2 = TRANSFORM_TEX(v.uv2, _FluidTex);
				
				half3 world_normal = UnityObjectToWorldNormal(v.normal);
				half intensity = 0.8 + (max(0.4, dot(world_normal, _WorldSpaceLightPos0.xyz))) / 5.0;
                o.color = _LightColor0 * intensity;
				
				UNITY_TRANSFER_FOG(o, o.pos);
                return o;
            }

            float4 frag (v2f i) : SV_Target 
			{
				float detail = tex2D(_FluidTex, (_Time[0] * float2(1, 1)) + i.uv2).r / 3.0f;
				
				i.uv1.x = clamp(i.uv1.x + (_SinTime[2] / 3.0f) - detail, 0.01f, 0.99f);
				
				float4 col = tex2D(_MainTex, i.uv1) * i.color;
				
				UNITY_APPLY_FOG(i.fogCoord, col);
				col.a = 1;
				return col;
			}
            ENDCG
        }
    } 
	FallBack "Diffuse"
}
