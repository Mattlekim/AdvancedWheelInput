#include "Macros2D.fxh" 

DECLARE_TEXTURE(Texture, 0);

float Pixels[13] =
{
	-6,
	-5,
	-4,
	-3,
	-2,
	-1,
	0,
	1,
	2,
	3,
	4,
	5,
	6,
};


float4 col;
float4 MainPS(float4 pos : SV_POSITION, float4 color1 : COLOR0, float2 texCoord : TEXCOORD0) : SV_TARGET0
{
	float Pixels[13] =
{
	-6,
	-5,
	-4,
	-3,
	-2,
	-1,
	0,
	1,
	2,
	3,
	4,
	5,
	6,
};

	float BlurWeights[13] =
{
	0.002216,
	0.008764,
	0.026995,
	0.064759,
	0.120985,
	0.176033,
	0.199471,
	0.176033,
	0.120985,
	0.064759,
	0.026995,
	0.008764,
	0.002216,
};
	//return  SAMPLE_TEXTURE(Texture, texCoord.xy);
	// Pixel width
	float pixelWidth = 1.0 / 1280.0;

	float4 color = { 0, 0, 0, 1 };

	float2 blur = texCoord;
	//blur.y = texCoord.y;
	col = float4(0, 0, 0, 0);
	for (int i = 0; i < 13; i++)
	{
		
		blur.x = texCoord.x + Pixels[i] * pixelWidth;
	//	col = SAMPLE_TEXTURE(Texture, blur.xy);
	//	col.r *= BlurWeights[i];
	//	col.b *= BlurWeights[i];
//		col.g *= BlurWeights[i];
		color += SAMPLE_TEXTURE(Texture, blur.xy) * BlurWeights[i];
		//color += col;
	}

	return color;
}

TECHNIQUE(main, MainPS);