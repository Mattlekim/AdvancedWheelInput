#include "Macros2D.fxh"

DECLARE_TEXTURE(Texture, 0);
DECLARE_TEXTURE(NormalMap, 1);
DECLARE_TEXTURE(LightDirectionMap, 2);
DECLARE_TEXTURE(LightMap, 3);
DECLARE_TEXTURE(EmmisionsMap, 4);

//float3 LightDirection =  float3(1,0,0);
//float4 LightColor = float4(1,1,1, 1);
float4 AmbientColor = float4(.3f,.3f,.3f,1);

float AmbientLightMultiplyer = 1.0f;

float MinimumLightPower = 0.1f;
float LightingMultiplyer = 5.0f;

int RenderNormalLightingOnly = 0;

float2 TextureScale = float2(1.0f, 1.0f);

float4 MainPS(float4 pos : SV_POSITION, float4 color1 : COLOR0, float2 texCoord : TEXCOORD0) : SV_TARGET0
{
	
	float4 lightData = SAMPLE_TEXTURE(LightMap, texCoord); //sample the light map

	float4 emissionsLight = SAMPLE_TEXTURE(EmmisionsMap, texCoord / TextureScale); //sample the light map
	//lightData.a = 1;
	lightData *= AmbientLightMultiplyer;
	float4 lightDirectionData = SAMPLE_TEXTURE(LightDirectionMap, texCoord); //sampel the direction light data
	//green is x direction 
	//red is y direction

	//convert it to proper directonaly vector
	float3 LightDirection = float3((lightDirectionData.r - .5f) * -2, (lightDirectionData.g - .5f) * -2, 0);

	//normalize the vector
	//this will make the light a constant strenght regardless of distance
	LightDirection.z = .1f;
	LightDirection = normalize(LightDirection);
	//we will use the blue chanel to depit light strenght

	//now we want to take the max light value from ether the light mask or the minimul light within ambientColor
	//float4 ambientColor = max(lightData, AmbientColor);

	//move to lbc for calculations of normal
	float4 lbc = lightData;



	//if the light level is bellow a level the normals will not light up
	//so we use the minimulLightPower to check this
	if (lbc.r < MinimumLightPower && lbc.g < MinimumLightPower && lbc.b < MinimumLightPower)
	{
		//if below set them to mimum levels
		lbc.r = MinimumLightPower;
		lbc.g = MinimumLightPower;
		lbc.b = MinimumLightPower;
	}


	//get the game world we want to draw
	float4 tex = SAMPLE_TEXTURE(Texture, texCoord);

	// Look up the texture and normalmap values.
	float4 NormalMapTex = SAMPLE_TEXTURE(NormalMap, texCoord);

	//get the blue chanel form the normal for the new edge lighing
	float NormalBlueChannel = NormalMapTex.b;

	//for noraml maps to work we need the blue texture to be 255 
	//as blue controles normal map strenght
//	NormalMapTex.b = 255;

	//calculate the normal (I STOLE THESE LINESS OF CODE)
	float3 normal = 2 * NormalMapTex - 1.0;
	//float3 normal = normalize(normal);
	float lightAmount = max(dot(normal.xyz, LightDirection), 0);
	//Calculate the lighting value to return and multiplyer by the game texture
	//partial my code partial stolen
	float norm = ((lightAmount * lbc * LightingMultiplyer));
	
	if (RenderNormalLightingOnly == 1)
	{
		return tex * norm * lightData;
	}
	
	//return tex *(ambientColor * norm);
	//if (norm < 0)
		//return 1;
	//return tex * (/*NormalBlueChannel * lightDirectionData.b*/ /*+ AmbientColor * AmbientLightMultiplyer + norm **/ lightData);

	return tex * max(emissionsLight, (AmbientColor * AmbientLightMultiplyer + norm * lightData));
	
//	return tex * (/*NormalBlueChannel * lightDirectionData.b*/ +ambientColor * AmbientLightMultiplyer * (1 - NormalBlueChannel * .3f) + norm);
	//return tex * (NormalBlueChannel * lightDirectionData.b + ambientColor * AmbientLightMultiplyer * (lightAmount * lbc * LightingMultiplyer * lightDirectionData.b));
}

TECHNIQUE(main, MainPS);
