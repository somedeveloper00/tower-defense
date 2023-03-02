Shader "Mobile/Diffuse Color"
{
    Properties
    {
        _Color("Color",COLOR)=(0.5,0.5,0.5,1.0)
    }

    SubShader
    { 
        Tags { "RenderType"="Opaque" }
        LOD 200
        CGPROGRAM
        #pragma surface surf Lambert noambient noshadow

        struct Input
        {
            float2 uv_MainTex;
        };

        UNITY_INSTANCING_BUFFER_START(Props)
           UNITY_DEFINE_INSTANCED_PROP(fixed4, _Color)
        UNITY_INSTANCING_BUFFER_END(Props)

        void surf (Input IN, inout SurfaceOutput o)
        {
            fixed4 c = UNITY_ACCESS_INSTANCED_PROP(Props, _Color);
            o.Albedo = c.rgb;
            o.Emission = c.rgb * 0.5;
        }
        ENDCG
    }
    Fallback "Mobile/VertexLit"
}