using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

[System.Serializable]
public class NoiseSettings
{
    public enum FilterType { Simple, Rigid };

    public FilterType           m_filterType;
    
    [ConditionalHide("m_filterType", 0)]
    public SimpleNoiseSettings  m_simpleNoiseSettings;
    [ConditionalHide("m_filterType", 1)]
    public RigidNoiseSettings   m_rigidNoiseSettings;

    [System.Serializable]
    public class SimpleNoiseSettings
    {
        public float m_strength = 1;

        [Range(1, 8)]
        public int      m_numLayers = 1;
        public float    m_baseRoughness = 1;
        public float    m_roughness = 2;
        public float    m_persistence = 0.5f;
        public Vector3  m_centre;
        public float    m_minValue;
    }

    [System.Serializable]
    public class RigidNoiseSettings : SimpleNoiseSettings
    {
        public float    m_weightMultiplier = 0.8f;
    }
}
