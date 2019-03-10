Shader "Unlit/MapShader" {

    SubShader {
	
		Tags {"RenderType"="Opaque"}
		
        Pass {
		
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
			#pragma multi_compile_fog
			
            #include "UnityCG.cginc"
			#include "UnityLightingCommon.cginc"
			
			struct i2v {
				float4 pos : POSITION;
				float4 color : COLOR;
				half3 normal : NORMAL;
			};
         
            struct v2f {
                float4 pos : POSITION;
                float4 color : COLOR;
				UNITY_FOG_COORDS(1)
            };
            
            v2f vert (i2v v)
            {
                v2f o;
                o.pos = UnityObjectToClipPos(v.pos);
				half3 world_normal = UnityObjectToWorldNormal(v.normal);
				half intensity = max(0.4, dot(world_normal, _WorldSpaceLightPos0.xyz));
                o.color = v.color * intensity * 1.2;
				UNITY_TRANSFER_FOG(o, o.pos);
                return o;
            }

            float4 frag (v2f i) : SV_Target 
			{
				float4 col = i.color;
				UNITY_APPLY_FOG(i.fogCoord, col);
				return col;
			}
            ENDCG
        }
    } 
	FallBack "Diffuse"
}
