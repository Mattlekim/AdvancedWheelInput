#include "Macros2D.fxh" 

DECLARE_TEXTURE(Texture, 0);

DECLARE_TEXTURE(Mask, 1);


float Enabled = 1;

float4 MainPS(float4 pos : SV_POSITION, float4 color1 : COLOR0, float2 texCoord : TEXCOORD0) : SV_TARGET0
{
	float4 pixelColor = SAMPLE_TEXTURE(Texture, texCoord);
	float4 maskColor = SAMPLE_TEXTURE(Mask, texCoord);

	
	if (Enabled == 1)
	{
		//return maskColor * min(pixelColor.a, maskColor.a);
		pixelColor *= maskColor.a;
		pixelColor.a *= maskColor.a;
	
	}

	return pixelColor;
}

TECHNIQUE(main, MainPS);