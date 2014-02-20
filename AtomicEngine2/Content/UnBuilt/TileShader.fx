float4x4 World;
float4x4 View;
float4x4 Projection;

float4 AmbientColor;
int XTexs;
int YTexs;

Texture2D Atlas;            // Color texture for mesh

SamplerState AtlasSampler
{
    Filter = MIN_MAG_MIP_LINEAR;
    AddressU = Wrap;
    AddressV = Wrap;
	
    Magfilter = POINT;
    Minfilter = POINT;
    Mipfilter = POINT;
};

struct VertexShaderInput
{
    float4 Position : POSITION0;
	float2 TexCoords : TEXCOORD0;
	float4 Color : COLOR0;
	int TexID : TEXCOORD1;
};

struct VertexShaderOutput
{
    float4 Position : POSITION0;	
	float2 TexCoords : TEXCOORD0;
	float4 Color : COLOR0;
	int TexID : TEXCOORD1;
};

float Wrap(float val, float min, float max)
{
	float range = max - min;

	if (max - val > 0)
		val -= range * (max - val % range);

	if (val - min < 0)
		val += range * (max - val % range);

	return val;
}

float2 WrapTex(float2 texCoords, int texID)
{
	float y = (texID / YTexs);
	float x = texID - (y * XTexs);

	float xStart = x * (1 / XTexs);
	float yStart = y * (1 / YTexs);

	float xEnd = xStart + (1 / XTexs);
	float yEnd = yStart + (1 / YTexs);

	float xI = texCoords.x / XTexs;
	float yI = texCoords.y / YTexs;

	return float2(Wrap(xI, xStart, xEnd), Wrap(yI, yStart, yEnd));
}

VertexShaderOutput VertexShaderFunction(VertexShaderInput input)
{
    VertexShaderOutput output;

    float4 worldPosition = mul(input.Position, World);
    float4 viewPosition = mul(worldPosition, View);
    output.Position = mul(viewPosition, Projection);

    output.TexCoords = input.TexCoords;
	output.TexID = input.TexID;
	output.Color = saturate(input.Color + AmbientColor);

    return output;
}

float4 PixelShaderFunction(VertexShaderOutput input) : COLOR0
{
    float4 texColor = Atlas.Sample(AtlasSampler, WrapTex(input.TexCoords, input.TexID));
	texColor.a = 1;

	return saturate(texColor + AmbientColor + input.Color);
}

technique Technique1
{
    pass Pass1
    {
        // TODO: set renderstates here.

        VertexShader = compile vs_3_0 VertexShaderFunction();
        PixelShader = compile ps_3_0 PixelShaderFunction();
    }
}
