
#include "Lighting.cginc"

//our lighting function. Will be called once per light
// source: https://www.ronja-tutorials.com/post/031-single-step-toon/
float4 LightingStepped(SurfaceOutput s, float3 lightDir, half3 viewDir, float shadowAttenuation) {
    //how much does the normal point towards the light?
    float towardsLight = dot(s.Normal, lightDir);
    // make the lighting a hard cut
    float towardsLightChange = fwidth(towardsLight);
    float lightIntensity = smoothstep(0, towardsLightChange, towardsLight);

#ifdef USING_DIRECTIONAL_LIGHT
    //for directional lights, get a hard vut in the middle of the shadow attenuation
    float attenuationChange = fwidth(shadowAttenuation) * 0.5;
    float shadow = smoothstep(0.5 - attenuationChange, 0.5 + attenuationChange, shadowAttenuation);
#else
    //for other light types (point, spot), put the cutoff near black, so the falloff doesn't affect the range
    float attenuationChange = fwidth(shadowAttenuation);
    float shadow = smoothstep(0, attenuationChange, shadowAttenuation);
#endif
    lightIntensity = lightIntensity * shadow;

    //calculate shadow color and mix light and shadow based on the light. Then taint it based on the light color
    
    float3 shadowColor = s.Albedo * 0.5;
    float4 color;
    color.rgb = lerp(shadowColor, s.Albedo, lightIntensity) * _LightColor0.rgb;
    color.a = s.Alpha;
    return color;
}