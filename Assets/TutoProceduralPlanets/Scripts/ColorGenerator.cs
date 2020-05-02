using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ColorGenerator
{
    ColorSettings   m_settings;
    Texture2D       m_texture;
    const int       m_textureRes = 50;
    INoiseFilter    m_biomeNoiseFilter;

    public void UpdateSettings(ColorSettings settings)
    {
        m_settings  = settings;

        if(m_texture == null || m_texture.height != m_settings.m_biomeColorSettings.m_biomes.Length)
            m_texture = new Texture2D(m_textureRes, m_settings.m_biomeColorSettings.m_biomes.Length);

        m_biomeNoiseFilter = NoiseFilterFactory.CreateNoiseFilter(m_settings.m_biomeColorSettings.m_noise);
    }

    public void UpdateElevation(MinMax elevationMinMax)
    {
        m_settings.m_planetMat.SetVector("_elevationMinMax", new Vector4(elevationMinMax.Min, elevationMinMax.Max));
    }

    public float BiomePercentFromPoint(Vector3 pointonUnitSphere)
    {
        float heightPercent = (pointonUnitSphere.y + 1) / 2.0f;
        heightPercent += (m_biomeNoiseFilter.Evaluate(pointonUnitSphere) - m_settings.m_biomeColorSettings.m_noiseOffset) * m_settings.m_biomeColorSettings.m_noiseStrength;
        float biomeIndex = 0;
        int numBiomes = m_settings.m_biomeColorSettings.m_biomes.Length;

        float blendRange = m_settings.m_biomeColorSettings.m_blendAmount / 2.0f + 0.001f;

        for (int i = 0; i < numBiomes; i++)
        {
            float dst = heightPercent - m_settings.m_biomeColorSettings.m_biomes[i].m_startHeight;
            float weight = Mathf.InverseLerp(-blendRange, blendRange, dst);

            biomeIndex *= (1 - weight);
            biomeIndex += i * weight;
        }

        return biomeIndex / Mathf.Max(1, numBiomes - 1);
    }

    public void UpdateColor()
    {
        Color[] colors = new Color[m_texture.width * m_texture.height];
        int colorIndex = 0;

        foreach (var biome in m_settings.m_biomeColorSettings.m_biomes)
        {
            for (int i = 0; i < m_textureRes; i++)
            {
                Color gradientCol = biome.m_gradient.Evaluate(i / (m_textureRes - 1.0f));
                Color tintColor = biome.m_tint;
                colors[colorIndex] = gradientCol * (1 - biome.m_tintPercent) + tintColor * biome.m_tintPercent;

                colorIndex++;
            }
        }

        m_texture.SetPixels(colors);
        m_texture.Apply();
        m_settings.m_planetMat.SetTexture("_texture", m_texture);
    }
}
