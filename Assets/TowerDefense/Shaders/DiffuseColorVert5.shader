Shader "Mobile/Diffuse Color Vert 5" {
    Properties {
        _ColorFrom1("Color From 1", Color) = (1, 1, 1, 1)
        _ColorTo1("Color To 1", Color) = (1, 1, 1, 1)
        _ColorFrom2("Color From 2", Color) = (1, 1, 1, 1)
        _ColorTo2("Color To 2", Color) = (1, 1, 1, 1)
        _ColorFrom3("Color From 3", Color) = (1, 1, 1, 1)
        _ColorTo3("Color To 3", Color) = (1, 1, 1, 1)
        _ColorFrom4("Color From 4", Color) = (1, 1, 1, 1)
        _ColorTo4("Color To 4", Color) = (1, 1, 1, 1)
        _ColorFrom4("Color From 5", Color) = (1, 1, 1, 1)
        _ColorTo5("Color To 4", Color) = (1, 1, 1, 1)
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
           UNITY_DEFINE_INSTANCED_PROP(fixed4, _ColorFrom1)
           UNITY_DEFINE_INSTANCED_PROP(fixed4, _ColorTo1)
           UNITY_DEFINE_INSTANCED_PROP(fixed4, _ColorFrom2)
           UNITY_DEFINE_INSTANCED_PROP(fixed4, _ColorTo2)
           UNITY_DEFINE_INSTANCED_PROP(fixed4, _ColorFrom3)
           UNITY_DEFINE_INSTANCED_PROP(fixed4, _ColorTo3)
           UNITY_DEFINE_INSTANCED_PROP(fixed4, _ColorFrom4)
           UNITY_DEFINE_INSTANCED_PROP(fixed4, _ColorTo4)
           UNITY_DEFINE_INSTANCED_PROP(fixed4, _ColorFrom5)
           UNITY_DEFINE_INSTANCED_PROP(fixed4, _ColorTo5)
           UNITY_DEFINE_INSTANCED_PROP(float, _Emission)
           // UNITY_DEFINE_INSTANCED_PROP(fixed4, _Color)
        UNITY_INSTANCING_BUFFER_END(Props)

        half fdist(fixed4 c1, fixed4 c2)
        {
            return abs(c2.r-c1.r) + abs(c2.g-c1.g) + abs(c2.b-c1.b);
        }

        void surf (Input IN, inout SurfaceOutput o) {
            fixed4 c = IN.color;
            // find nearest color and assign that as albedo
            float d1 = fdist(c, UNITY_ACCESS_INSTANCED_PROP(Props, _ColorFrom1));
            float d2 = fdist(c, UNITY_ACCESS_INSTANCED_PROP(Props, _ColorFrom2));
            float d3 = fdist(c, UNITY_ACCESS_INSTANCED_PROP(Props, _ColorFrom3));
            float d4 = fdist(c, UNITY_ACCESS_INSTANCED_PROP(Props, _ColorFrom4));
            float d5 = fdist(c, UNITY_ACCESS_INSTANCED_PROP(Props, _ColorFrom5));
            float _min = min(d1, min(d2, min(d3, min(d4, d5))));
            o.Albedo = _min == d1
                ? UNITY_ACCESS_INSTANCED_PROP(Props, _ColorTo1)
                : _min == d2
                    ? UNITY_ACCESS_INSTANCED_PROP(Props, _ColorTo2)
                    : _min == d3
                        ? UNITY_ACCESS_INSTANCED_PROP(Props, _ColorTo3)
                        : _min == d4
                            ? UNITY_ACCESS_INSTANCED_PROP(Props, _ColorTo4)
                            : UNITY_ACCESS_INSTANCED_PROP(Props, _ColorTo5);
            // set emission
            o.Emission = o.Albedo * UNITY_ACCESS_INSTANCED_PROP(Props, _Emission);
        }
        ENDCG
    }
    Fallback "Mobile/VertexLit"
}