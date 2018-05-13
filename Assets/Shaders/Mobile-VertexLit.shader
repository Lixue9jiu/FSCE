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
fixed4 _Color;

struct Input {
    float2 uv_MainTex;
    float3 worldNormal;
    float3 worldPos;
    float3 color : COLOR;
};

float4 fourTapSample(float2 tileOffset, //Tile offset in the atlas
                  float2 tileUV, //Tile coordinate (as above)
                  float tileSize, //Size of a tile in atlas
                  sampler2D atlas) {
  //Initialize accumulators
  float4 color = float4(0.0, 0.0, 0.0, 0.0);
  float totalWeight = 0.0;

  for(int dx=0; dx<2; ++dx)
  for(int dy=0; dy<2; ++dy) {
    //Compute coordinate in 2x2 tile patch
    float2 tileCoord = 2.0 * frac(0.5 * (tileUV + float2(dx,dy)));

    //Weight sample based on distance to center
    float w = pow(1.0 - max(abs(tileCoord.x-1.0), abs(tileCoord.y-1.0)), 16.0);

    //Compute atlas coord
    float2 atlasUV = tileOffset + tileSize * tileCoord;

    //Sample and accumulate
    color += w * tex2D(atlas, atlasUV);
    totalWeight += w;
  }

  //Return weighted color
  return color / totalWeight;
}

void surf (Input IN, inout SurfaceOutput o) {
    float2 tileUV = abs(float2(
        dot(IN.worldNormal.zxy,
            IN.worldPos),
        dot(IN.worldNormal.yzx,
            IN.worldPos)
    ));
    if (IN.worldNormal.x != 0) {
        tileUV = tileUV.yx;
    }
    fixed4 c = fourTapSample(IN.uv_MainTex, tileUV, 0.03125f, _MainTex);
    o.Albedo = c.rgb * IN.color;
}
ENDCG
}
}
