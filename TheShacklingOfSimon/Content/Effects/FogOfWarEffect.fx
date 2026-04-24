#if OPENGL
#define SV_POSITION POSITION
#define VS_SHADERMODEL vs_3_0
#define PS_SHADERMODEL ps_3_0
#else
#define VS_SHADERMODEL vs_4_0_level_9_1
#define PS_SHADERMODEL ps_4_0_level_9_1
#endif

float4 FogBounds;
float MaxFogOpacity;
int LightCount;

float4 LightData[16];
// x = centerX
// y = centerY
// z = innerRadius
// w = outerRadius

float4 LightStrengths[16];
// x = strength

float SmoothLightFalloff(float distance, float innerRadius, float outerRadius)
{
    if (distance <= innerRadius)
    {
        return 1.0f;
    }

    if (distance >= outerRadius)
    {
        return 0.0f;
    }

    float t = saturate((distance - innerRadius) / max(outerRadius - innerRadius, 0.0001f));

    // smoothstep.
    float smooth = t * t * (3.0f - 2.0f * t);

    // Lower exponent makes the light edge softer and prevents dark seams
    // between nearby light sources.
    return pow(1.0f - smooth, 0.62f);
}

float GetLightContribution(float2 worldPosition, float4 lightData, float strength)
{
    float2 difference = worldPosition - lightData.xy;
    float distance = length(difference);

    float falloff = SmoothLightFalloff(distance, lightData.z, lightData.w);

    return saturate(falloff * strength);
}

float4 MainPS(float4 color : COLOR0, float2 texCoord : TEXCOORD0) : COLOR0
{
    float2 worldPosition = FogBounds.xy + texCoord * FogBounds.zw;

    float remainingDarkness = 1.0f;
    float strongestLight = 0.0f;

    for (int i = 0; i < 16; i++)
    {
        if (i >= LightCount)
        {
            break;
        }

        float contribution = GetLightContribution(
            worldPosition,
            LightData[i],
            LightStrengths[i].x
        );

        strongestLight = max(strongestLight, contribution);

        // Screen-style light combination.
        // This makes overlapping lights combine instead of creating dark seams.
        remainingDarkness *= 1.0f - contribution;
    }

    float combinedLight = 1.0f - remainingDarkness;

    // Keep the strongest light readable, but allow overlaps to fill in naturally.
    float finalLight = max(strongestLight, combinedLight);

    // Prevent huge blowout while still fixing dark overlap seams.
    finalLight = saturate(finalLight * 0.92f);

    float fogAlpha = MaxFogOpacity * (1.0f - finalLight);

    return float4(0.0f, 0.0f, 0.0f, fogAlpha);
}

technique FogOfWar
{
    pass Pass1
    {
        PixelShader = compile PS_SHADERMODEL MainPS();
    }
}