using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShapeGenerator
{
    ShapeSettings   m_settings;
    INoiseFilter[]  m_noiseFilters;
    public MinMax   m_elevationMinMax;

    public void UpdateSettings(ShapeSettings settings)
    {
        m_settings      = settings;
        m_noiseFilters  = new INoiseFilter[m_settings.m_noiseLayers.Length];

        for (int i = 0; i < m_settings.m_noiseLayers.Length; i++)
        {
            m_noiseFilters[i] = NoiseFilterFactory.CreateNoiseFilter(m_settings.m_noiseLayers[i].m_noiseSettings);
        }

        m_elevationMinMax = new MinMax();
    }

    public Vector3 CalculatePointOnPlanet(Vector3 pointOnUnitSphere)
    {
        float firstLayerValue = 0;
        float elevation = 0;

        if(m_noiseFilters.Length > 0)
        {
            firstLayerValue = m_noiseFilters[0].Evaluate(pointOnUnitSphere);
            if (m_settings.m_noiseLayers[0].m_enabled)
                elevation = firstLayerValue;
        }

        for (int i = 1; i < m_noiseFilters.Length; i++)
        {
            if (m_settings.m_noiseLayers[i].m_enabled)
            {
                float mask = (m_settings.m_noiseLayers[i].m_useFirstLayerAsMask) ? firstLayerValue : 1;

                elevation += m_noiseFilters[i].Evaluate(pointOnUnitSphere) * mask;
            }
        }

        elevation = m_settings.m_planetRadius * (1 + elevation);
        m_elevationMinMax.AddValue(elevation);

        return pointOnUnitSphere * elevation;
    }
}
