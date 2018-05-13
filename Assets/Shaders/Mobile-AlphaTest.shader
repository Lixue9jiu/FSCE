Shader "Custom/MobileAlphaTest" {
    Properties {
        _MainTex ("Base (RGB) Transparency (A)", 2D) = "" {}
    }

SubShader {
    Tags {"Queue"="AlphaTest" "IgnoreProjector"="True" "RenderType"="TransparentCutout"}
    LOD 120

CGPROGRAM
#pragma surface surf Lambert noforwardadd

sampler2D _MainTex;
fixed4 _Color;

struct Input {
    float2 uv_MainTex;
    float3 color : COLOR;
};

void surf (Input IN, inout SurfaceOutput o) {
    fixed4 c = tex2D(_MainTex, IN.uv_MainTex);
    o.Albedo = c.rgb * IN.color;
    clip(c.a - 0.5);
    o.Alpha = c.a;
}
ENDCG
}
}