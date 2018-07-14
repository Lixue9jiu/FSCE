Shader "Custom/MobileVertexLit" {
    Properties {
        _MainTex ("Base (RGB) Transparency (A)", 2D) = "" {}
    }

SubShader {
    Tags {"RenderType"="Opaque"}
    LOD 80

CGPROGRAM
#pragma surface surf Lambert noforwardadd

sampler2D _MainTex;

struct Input {
    float2 uv_MainTex;
    float3 color : COLOR;
};

void surf (Input IN, inout SurfaceOutput o) {
    fixed4 c = tex2D(_MainTex, IN.uv_MainTex);
    o.Albedo = c.rgb * IN.color;
    //o.Alpha = c.a;
}
ENDCG
}
}
