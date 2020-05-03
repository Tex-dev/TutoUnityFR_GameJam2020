using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Planet))]
public class GeneratePlanet : MonoBehaviour
{
    public enum PlanetType { water, sulfur, methane, iodine, nbType};
    
    PlanetType      m_type;
    
    [SerializeField]
    ColorSettings[] m_colorsSettings;

    Planet          m_planetScript;

    private float   m_liveableAreaValue;

    [SerializeField]
    Dropdown        m_dropdownType;


    // Start is called before the first frame update
    void Start()
    {
        m_planetScript = GetComponent<Planet>();

        m_type = (PlanetType)Random.Range(0, (int)PlanetType.nbType);
        m_planetScript.m_colorSettings = m_colorsSettings[(int)m_type];

        m_dropdownType.value = (int)m_type;

        m_dropdownType.onValueChanged.AddListener(OnDropdownValueChanged);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnDropdownValueChanged(int value)
    {
        Debug.LogWarning("changed");

        m_type = (PlanetType)value;
        m_planetScript.m_colorSettings = m_colorsSettings[(int)m_type];
        m_planetScript.GeneratePlanet();
    }
}
