#include "Macros.fxh"

DECLARE_TEXTURE(Texture, 0);

float4x4 Projection    _vs(c0) _cb(c0);

#define IDENTITY_MATRIX float4x4(1, 0, 0, 0, 0, 1, 0, 0, 0, 0, 1, 0, 0, 0, 0, 1)

struct VSInput
{
    float4 Position : SV_Position;
    float2 TexCoord : TEXCOORD0;
};

struct VSOutput
{
    float4 Diffuse    : COLOR0;
    float2 TexCoord   : TEXCOORD0;
    float4 PositionPS : SV_Position;
};

float4x4 CreateScale(float xScale, float yScale, float zScale)
{
    return float4x4(xScale, 0, 0, 0, 0, yScale, 0, 0, 0, 0, zScale, 0, 0, 0, 0, 1);
}

float4x4 CreateTranslation(float x, float y, float z)
{
    return float4x4(1, 0, 0, 0, 0, 1, 0, 0, 0, 0, 1, 0, x, y, z, 1);
}

// Vertex shader: texture + vertex color
VSOutput MainVS(VSInput input, float4 trans : POSITION1, float4 diffuse : COLOR1)
{
    VSOutput vout;

    float4x4 worldMat = mul(CreateScale(trans.x, trans.y, 0), CreateTranslation(trans.z, trans.w, 0));
    worldMat = mul(worldMat, Projection);

    vout.PositionPS = mul(input.Position, worldMat);
    
    vout.TexCoord = input.TexCoord;
    vout.Diffuse = diffuse;
    
    return vout;
}

float4 MainPS(VSOutput input) : SV_Target0
{
    float4 color = SAMPLE_TEXTURE(Texture, input.TexCoord) * input.Diffuse;
 
    return color;
}

technique GPUInstancing
{
    pass
    {
        VertexShader = compile vs_2_0 MainVS();
        PixelShader = compile ps_2_0 MainPS();
    }
}