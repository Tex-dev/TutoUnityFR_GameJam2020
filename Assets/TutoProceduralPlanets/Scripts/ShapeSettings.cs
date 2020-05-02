using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu()]
public class ShapeSettings : ScriptableObject
{
    public float m_planetRadius;
    public NoiseLayer[] m_noiseLayers;


    [System.Serializable]
    public class NoiseLayer
    {
        public bool             m_enabled = true;
        public bool             m_useFirstLayerAsMask;
        public NoiseSettings    m_noiseSettings;
    }

    public ShapeSettings(ShapeSettings shapeSettings)
    {
        m_planetRadius  = shapeSettings.m_planetRadius;
        m_noiseLayers   = shapeSettings.m_noiseLayers;
    }
}
