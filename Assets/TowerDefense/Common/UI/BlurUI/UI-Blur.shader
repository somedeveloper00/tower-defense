// Unity built-in shader source. Copyright (c) 2016 Unity Technologies. MIT license (see license.txt)

Shader "UI/Blurred Background" {
    Properties {
        _Blur ("Blur", Range(0, 1)) = 0.5
        
        [PerRendererData] _MainTex ("Sprite Texture", 2D) = "white" {}
        _Color ("Tint", Color) = (1,1,1,1)

        _StencilComp ("Stencil Comparison", Float) = 8
        _Stencil ("Stencil ID", Float) = 0
        _StencilOp ("Stencil Operation", Float) = 0
        _StencilWriteMask ("Stencil Write Mask", Float) = 255
        _StencilReadMask ("Stencil Read Mask", Float) = 255

        _ColorMask ("Color Mask", Float) = 15

        [Toggle(UNITY_UI_ALPHACLIP)] _UseUIAlphaClip ("Use Alpha Clip", Float) = 0
    }

    SubShader {
        Tags {
            "Queue"="Transparent"
            "IgnoreProjector"="True"
            "RenderType"="Transparent"
            "PreviewType"="Plane"
            "CanUseSpriteAtlas"="True"
        }

        Stencil {
            Ref [_Stencil]
            Comp [_StencilComp]
            Pass [_StencilOp]
            ReadMask [_StencilReadMask]
            WriteMask [_StencilWriteMask]
        }

        Cull Off
        Lighting Off
        ZWrite Off
        ZTest [unity_GUIZTestMode]
        Blend SrcAlpha OneMinusSrcAlpha
        ColorMask [_ColorMask]
        
        GrabPass { "_UI_Blur_GrabPass" }

        Pass {
            Name "Default"
        CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma target 2.0

            #include "UnityCG.cginc"

            #pragma multi_compile_local _ UNITY_UI_CLIP_RECT
            #pragma multi_compile_local _ UNITY_UI_ALPHACLIP

            #define QUALITY 1

            struct appdata_t {
                float4 vertex   : POSITION;
                float4 color    : COLOR;
                float2 texcoord : TEXCOORD0;
                UNITY_VERTEX_INPUT_INSTANCE_ID
            };

            struct v2f {
                float4 vertex   : SV_POSITION;
                fixed4 color    : COLOR;
                float2 texcoord  : TEXCOORD0;
                float4 worldPosition : TEXCOORD1;
                float4 screenPos : TEXCOORD2;
                UNITY_VERTEX_OUTPUT_STEREO
            };

            sampler2D _MainTex;
            sampler2D _UI_Blur_GrabPass;
            fixed4 _Color;
            fixed4 _TextureSampleAdd;
            float4 _ClipRect;
            float4 _MainTex_ST;
            float _Blur;

            float UnityGet2DClipping (in float2 position, in float4 clipRect)
            {
                float2 inside = step(clipRect.xy, position.xy) * step(position.xy, clipRect.zw);
                return inside.x * inside.y;
            }

            v2f vert(appdata_t v) {
                v2f OUT;
                UNITY_SETUP_INSTANCE_ID(v);
                UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(OUT);
                OUT.worldPosition = v.vertex;
                OUT.vertex = UnityObjectToClipPos(OUT.worldPosition);
                OUT.screenPos = ComputeScreenPos(OUT.vertex);
                OUT.texcoord = TRANSFORM_TEX(v.texcoord, _MainTex);
            #if UNITY_UV_STARTS_AT_TOP
                OUT.texcoord.y = 1 - OUT.texcoord.y;
                OUT.screenPos.y = 1 - OUT.screenPos.y;
            #endif
                OUT.color = v.color * _Color;
                return OUT;
            }

            fixed4 frag(v2f IN) : SV_Target {
                float4 color = 0;
                // (tex2D(_MainTex, IN.texcoord) + _TextureSampleAdd) * IN.color;

                // box blur on _UI_Blur_GrabPass texture, given the QUALITY (with for loop)
                half power = ((tex2D(_MainTex, IN.texcoord) + _TextureSampleAdd) * IN.color).a;
                float2 pivot_uv = float2(IN.screenPos.x, IN.screenPos.y);
                float2 offset = power * _Blur / 200 / QUALITY;
                float totalInfluence = 0;
                for (int x = -QUALITY; x <= QUALITY; ++x) {
                    for (int y = -QUALITY; y <= QUALITY; ++y) {
                        float2 uv = pivot_uv + float2(x, y) * offset;
                        float influence = QUALITY * 2 + 1 - abs(x) - abs(y);
                        totalInfluence += influence;
                        color = color + tex2D(_UI_Blur_GrabPass, uv) * influence;
                    }
                }
                color.rgb = color / totalInfluence;
                color = color + _TextureSampleAdd * IN.color;
                color.a = IN.color.a;
                
                

                // #ifdef UNITY_UI_CLIP_RECT
                // color.a *= UnityGet2DClipping(IN.worldPosition.xy, _ClipRect);
                // #endif

                #ifdef UNITY_UI_ALPHACLIP
                clip (color.a - 0.001);
                #endif

                return color;
            }
        ENDCG
        }
    }
}