using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.ShaderGraph.Internal;
using UnityEngine;

public class SimpleNoiseFilter : INoiseFilter
{
    NoiseSettings.SimpleNoiseSettings m_settings;
    Noise m_noise = new Noise();

    public SimpleNoiseFilter(NoiseSettings.SimpleNoiseSettings settings)
    {
        m_settings = settings;
    }

    public float Evaluate(Vector3 point)
    {
        float noiseValue = 0;// (m_noise.Evaluate(point * m_settings.m_roughness + m_settings.m_centre) + 1) * 0.5f;
        float frequency = m_settings.m_baseRoughness;
        float amplitude = 1;

        for (int i = 0; i < m_settings.m_numLayers; i++)
        {
            float v = m_noise.Evaluate(point * frequency + m_settings.m_centre);
            noiseValue += (v + 1) * 0.5f * amplitude;
            frequency *= m_settings.m_roughness;
            amplitude *= m_settings.m_persistence;
        }

        noiseValue = Mathf.Max(0, noiseValue - m_settings.m_minValue);

        return noiseValue * m_settings.m_strength;
    }
}
