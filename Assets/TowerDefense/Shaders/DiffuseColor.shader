Shader "Mobile/Toon Diffuse Color" {
    Properties {
        _Color ("Color", Color) = (1, 1, 1, 1)
        _Emission ("Emission", Float) = 0.5
    }

    SubShader { 
        Tags { "RenderType"="Opaque" }
        LOD 200
        CGPROGRAM
        #include "tooninc.cginc"
        #pragma surface surf Stepped noambient noshadow
        
        UNITY_INSTANCING_BUFFER_START(Props)
           UNITY_DEFINE_INSTANCED_PROP(fixed4, _Color)
           UNITY_DEFINE_INSTANCED_PROP(float, _Emission)
        UNITY_INSTANCING_BUFFER_END(Props)
        

        struct Input {
            float4 color;
        };

        void surf (Input IN, inout SurfaceOutput o) {
            o.Albedo = UNITY_ACCESS_INSTANCED_PROP(Props, _Color);
            o.Emission = o.Albedo.rgb * UNITY_ACCESS_INSTANCED_PROP(Props, _Emission);
        }
        ENDCG
    }
    Fallback "Mobile/VertexLit"
}