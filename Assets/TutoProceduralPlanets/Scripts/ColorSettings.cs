using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu()]
public class ColorSettings : ScriptableObject
{
    public Material             m_planetMat;
    public BiomeColorSettings   m_biomeColorSettings;

    [System.Serializable]
    public class BiomeColorSettings
    {
        public Biome[]          m_biomes;   
        public NoiseSettings    m_noise;
        public float            m_noiseOffset;
        public float            m_noiseStrength;
        [Range(0,1)]
        public float            m_blendAmount;

        [System.Serializable]
        public class Biome
        {
            public Gradient m_gradient;
            public Color    m_tint;

            [Range(0,1)]
            public float    m_startHeight;

            [Range(0, 1)]
            public float    m_tintPercent;
        }
    }

    public ColorSettings(ColorSettings colorSettings)
    {
        m_planetMat             = colorSettings.m_planetMat;
        m_biomeColorSettings    = colorSettings.m_biomeColorSettings;
    }
}
