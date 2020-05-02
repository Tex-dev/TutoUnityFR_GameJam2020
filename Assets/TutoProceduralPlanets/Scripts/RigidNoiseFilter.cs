using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RigidNoiseFilter : INoiseFilter
{
    NoiseSettings.RigidNoiseSettings m_settings;
    Noise m_noise = new Noise();

    public RigidNoiseFilter(NoiseSettings.RigidNoiseSettings settings)
    {
        m_settings = settings;
    }

    public float Evaluate(Vector3 point)
    {
        float noiseValue = 0;// (m_noise.Evaluate(point * m_settings.m_roughness + m_settings.m_centre) + 1) * 0.5f;
        float frequency = m_settings.m_baseRoughness;
        float amplitude = 1;
        float weight = 1;

        for (int i = 0; i < m_settings.m_numLayers; i++)
        {
            float v = 1 - Mathf.Abs(m_noise.Evaluate(point * frequency + m_settings.m_centre));
            v *= v;
            v *= weight;
            weight = Mathf.Clamp01(v * m_settings.m_weightMultiplier);

            noiseValue += v * amplitude;
            frequency *= m_settings.m_roughness;
            amplitude *= m_settings.m_persistence;
        }

        noiseValue = Mathf.Max(0, noiseValue - m_settings.m_minValue);

        return noiseValue * m_settings.m_strength;
    }
}
