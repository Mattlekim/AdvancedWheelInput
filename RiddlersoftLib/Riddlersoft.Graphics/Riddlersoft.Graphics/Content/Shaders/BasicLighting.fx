#include "Macros2D.fxh"

///basicic lighting texture



float Darken = .2f;
float Brightness = 0;
float Contrast = 1;
float AmbientBrigthness = 1;

DECLARE_TEXTURE(Texture, 0);
DECLARE_TEXTURE(TextureLight, 1);


float3 LightDirection;

float3 AmbientColor = 0.75f;



float EnableLightMap = 1;


float4 MainPS(float4 pos : SV_POSITION, float4 color1 : COLOR0, float2 texCoord : TEXCOORD0) : SV_TARGET0
{
//	EnableLightMap = EnableLightMap;

	float4 pixelColor = SAMPLE_TEXTURE(Texture, texCoord);
	float4 lightColor = SAMPLE_TEXTURE(TextureLight, texCoord);

	//  pixelColor.rgb /= pixelColor.a;

	  // Apply brightness.
	
	float4 col = Brightness - 1;
	if (EnableLightMap == 1)
	{
		  // Apply contrast.
		//pixelColor.rgb = ((pixelColor.rgb - 0.5f) * max(Contrast, 0)) + 0.5f;
		col = lightColor.a * Brightness;
        
        pixelColor.rgb *= (AmbientColor * AmbientBrigthness) + (lightColor.rgb * col.a);
			//pixelColor.rbg *= AmbientBrigthness;

      //  pixelColor.rgb *= max(AmbientBrigthness, col.a);
        return pixelColor;
    }
	else
	{
		  pixelColor.rgb = ((pixelColor.rgb - 0.5f) * max(Contrast, 0)) + 0.5f;
	}
		
	  // Return final pixel color.
	  //  pixelColor.rgb *= pixelColor.a;

	

	return pixelColor;
}

TECHNIQUE(main, MainPS);
