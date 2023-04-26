Shader ".valtterim/Instanced/i_m_TextureArray"{
    Properties{
        _Color("Color", Color) = (1,1,1,1)
    }
 
    SubShader{
        Tags { "Queue"="Geometry" "RenderType"="Opaque" }
        Lighting Off
        Cull Off
 
        Pass{
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"
 
            struct vertexInput{
                float4 vertex : POSITION;
            };
 
            struct vertexOutput{
                float4 vertex : SV_POSITION;
            };

            fixed4 _Color;
            
            vertexOutput vert(vertexInput input){
                vertexOutput output;
                output.vertex = UnityObjectToClipPos(input.vertex);
                return output;
            }
 
            fixed4 frag(vertexOutput output, fixed facing : VFACE) : SV_Target{
                float normal = -1;
                clip(facing * normal);
                return _Color;
            }
 
            ENDCG
        }
    }
}