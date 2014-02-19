//Input variables
float4x4 viewProjection  : ViewProjection;
//------------------------------------
texture diffuseTexture : Diffuse <string ResourceName = "default_color.dds";>;

sampler TextureSampler = sampler_state
{
    texture = <diffuseTexture>;
    AddressU  = CLAMP;
    AddressV  = CLAMP;
    AddressW  = CLAMP;
    MIPFILTER = POINT;
    MINFILTER = POINT;
    MAGFILTER = POINT;
};

struct Vertex
{
    float4 Position: POSITION;
    float4 Color: COLOR;
    float2 TexCoord : TEXCOORD0;
};

Vertex VS_Identity(Vertex v)
{
    Vertex result;
    //  2D vertices are already pre-multiplied times the world matrix.
    result.Position = mul(v.Position, viewProjection);
    result.Color = v.Color;
    result.TexCoord = v.TexCoord;
    return result;
}

//-----------------------------------
float4 PS_Textured(Vertex v): COLOR
{
    float4 diffuseTexture = tex2D( TextureSampler, v.TexCoord);
    return v.Color * diffuseTexture;
}

technique Identity
{

   pass p0
   {
        VertexShader = compile vs_1_1 VS_Identity();
        PixelShader  = compile ps_1_1 PS_Textured();
   }

}