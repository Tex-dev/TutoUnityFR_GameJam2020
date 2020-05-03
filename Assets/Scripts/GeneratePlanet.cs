using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Planet))]
public class GeneratePlanet : MonoBehaviour
{
    public enum PlanetType { H2O, S, Hg, I, nbType};
    
    PlanetType      m_type;
    
    [SerializeField]
    ColorSettings[] m_colorsSettings;

    Planet          m_planetScript;

    private float   m_percentField;

    // Start is called before the first frame update
    void Start()
    {
        m_planetScript = GetComponent<Planet>();

        m_type = (PlanetType)Random.Range(0, (int)PlanetType.nbType);
        m_planetScript.m_colorSettings = m_colorsSettings[(int)m_type];
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
