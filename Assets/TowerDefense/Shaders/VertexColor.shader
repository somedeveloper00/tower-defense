Shader "Mobile/Vertex Color" {
    Properties {
        _Add ("Add Color", Color) = (0, 0, 0, 0)
        _Emission ("Emission", Range(0, 5)) = 0.5 
    }

    SubShader { 
        Tags { "RenderType"="Opaque" }
        LOD 200
        CGPROGRAM
        #pragma surface surf Lambert noambient noshadow

        struct Input {
            float4 color : COLOR;
        };

        UNITY_INSTANCING_BUFFER_START(Props)
           UNITY_DEFINE_INSTANCED_PROP(fixed4, _Add)
           UNITY_DEFINE_INSTANCED_PROP(float, _Emission)
           // UNITY_DEFINE_INSTANCED_PROP(fixed4, _Color)
        UNITY_INSTANCING_BUFFER_END(Props)

        void surf (Input IN, inout SurfaceOutput o) {
            fixed4 c = IN.color;
            o.Albedo = c.rgb;
            // add color
            o.Albedo += UNITY_ACCESS_INSTANCED_PROP(Props, _Add).rgb;
            o.Emission = c.rgb * UNITY_ACCESS_INSTANCED_PROP(Props, _Emission) ;
        }
        ENDCG
    }
    Fallback "Mobile/VertexLit"
}