Shader "Mobile/Diffuse Color Transparent" {
    Properties {
        _Color("Color",COLOR)=(0.5,0.5,0.5,1.0)
        _ColorTo("Color To",COLOR)=(0.5,0.5,0.5,1.0)
        _Speed("Color Change Speed", Float) = 1
        _Emission ("Emission", Range(0, 5)) = 0.5 
    }

    SubShader {
        Tags { "Queue"="Transparent" "RenderType"="Transparent" } 
        LOD 150
        CGPROGRAM
        #include "tooninc.cginc"
        #pragma surface surf Stepped alpha:fade noambient noshadow

        struct Input {
            float3 worldPos;
        };


        UNITY_INSTANCING_BUFFER_START(Props)
           UNITY_DEFINE_INSTANCED_PROP(fixed4, _Color)
           UNITY_DEFINE_INSTANCED_PROP(fixed4, _ColorTo)
           UNITY_DEFINE_INSTANCED_PROP(float, _Emission)
           UNITY_DEFINE_INSTANCED_PROP(float, _Speed)
        UNITY_INSTANCING_BUFFER_END(Props)
        
        void surf (Input IN, inout SurfaceOutput o) {
            const fixed4 c1 = UNITY_ACCESS_INSTANCED_PROP(Props, _Color);
            const fixed4 c2 = UNITY_ACCESS_INSTANCED_PROP(Props, _ColorTo);
            fixed4 c = lerp( c1, c2, 0.5 + 0.5 * sin(_Time.y * UNITY_ACCESS_INSTANCED_PROP(Props, _Speed)));
            o.Albedo = c.rgb;
            o.Alpha = c.a;
            o.Emission = c.rgb * UNITY_ACCESS_INSTANCED_PROP(Props, _Emission);
        }
        ENDCG
    }
    Fallback "Mobile/VertexLit"
}