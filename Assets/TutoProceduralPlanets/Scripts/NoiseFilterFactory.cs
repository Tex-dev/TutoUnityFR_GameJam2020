using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class NoiseFilterFactory
{
    public static INoiseFilter CreateNoiseFilter(NoiseSettings settings)
    {
        switch (settings.m_filterType)
        {
            case NoiseSettings.FilterType.Simple:
                return new SimpleNoiseFilter(settings.m_simpleNoiseSettings);

            case NoiseSettings.FilterType.Rigid:
                return new RigidNoiseFilter(settings.m_rigidNoiseSettings);
        }

        return null;
    }
}
