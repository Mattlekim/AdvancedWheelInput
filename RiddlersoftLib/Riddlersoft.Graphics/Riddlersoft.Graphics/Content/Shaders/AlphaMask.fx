#include "Macros2D.fxh" 

DECLARE_TEXTURE(Texture, 0);

DECLARE_TEXTURE(AlphaTexture, 1);


float Enabled = 1;
float Rotation = 1;
float4 MainPS(float4 pos : SV_POSITION, float4 color1 : COLOR0, float2 texCoord : TEXCOORD0) : SV_TARGET0
{ 

	texCoord.x = texCoord.x - .5f;
	texCoord.y = texCoord.y - .5f;

	float2 tmp;
	tmp.x = cos(Rotation) * texCoord.x - sin(Rotation) * texCoord.y;
	tmp.y = sin(Rotation) * texCoord.x + cos(Rotation) * texCoord.y;

	tmp.x = tmp.x + .5f;
	tmp.y = tmp.y + .5f;


	texCoord.x = texCoord.x + .5f;
	texCoord.y = texCoord.y + .5f;

	//	texCoord = tmp;
	float4 pixelColor = SAMPLE_TEXTURE(AlphaTexture, texCoord);
	float4 maskColor = SAMPLE_TEXTURE(Texture, tmp);

	if (Enabled != 1)
	{
		maskColor = SAMPLE_TEXTURE(Texture, texCoord);
		return maskColor;
	}

	return float4(pixelColor.r *  maskColor.a, pixelColor.g *  maskColor.a, pixelColor.b *  maskColor.a, maskColor.a);


}

TECHNIQUE(main, MainPS);