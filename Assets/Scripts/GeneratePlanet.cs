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
    [Range(0, 100)]
    private float m_liveableAreaThreshold;

    [SerializeField]
    Dropdown        m_dropdownType;

    [SerializeField]
    Text            m_liveableAreaField;

    [SerializeField]
    Text[]          m_liveableAreaOK;

    [SerializeField]
    Button          m_button;

    [SerializeField]
    Slider[]        m_sliders;

    [Range(-10.0f, 10.0f)]
    private float[] m_values = new float[3];


    // Start is called before the first frame update
    void Start()
    {
        m_planetScript = GetComponent<Planet>();

        m_type = (PlanetType)Random.Range(0, (int)PlanetType.nbType);
        m_planetScript.m_colorSettings = m_colorsSettings[(int)m_type];

        m_dropdownType.value = (int)m_type;

        m_dropdownType.onValueChanged.AddListener(OnDropdownValueChanged);

        for (int i = 0; i < m_sliders.Length; i++)
        {
            int id = i;
            m_values[i] = Random.Range(-10.0f, 10.0f);
            m_planetScript.m_shapeSettings.m_noiseLayers[0].m_noiseSettings.m_simpleNoiseSettings.m_centre[i] = m_values[i];
            m_sliders[i].value = m_values[i];

            m_sliders[i].onValueChanged.AddListener((float param) => OnSliderValueChanged(id, param));
        }

        m_planetScript.GeneratePlanet();
    }

    // Update is called once per frame
    void Update()
    {
        m_liveableAreaValue = m_planetScript.GetLiveableAreaPercent() * 100.0f;
        m_liveableAreaField.text = ((int)m_liveableAreaValue).ToString() + "%";

        if (m_liveableAreaValue < m_liveableAreaThreshold)
            m_button.interactable = false;
        else
            m_button.interactable = true;

        m_liveableAreaOK[0].gameObject.SetActive(m_button.interactable);
        m_liveableAreaOK[1].gameObject.SetActive(!m_button.interactable);
    }

    void OnDropdownValueChanged(int value)
    {
        for (int i = 0; i < m_sliders.Length; i++)
        {
            m_values[i] = Random.Range(-10.0f, 10.0f);
            m_planetScript.m_shapeSettings.m_noiseLayers[0].m_noiseSettings.m_simpleNoiseSettings.m_centre[i] = m_values[i];
            m_sliders[i].value = m_values[i];
        }

        m_type = (PlanetType)value;
        m_planetScript.m_colorSettings = m_colorsSettings[(int)m_type];
        m_planetScript.GeneratePlanet();
    }

    void OnSliderValueChanged(int index, float value)
    {
        m_values[index] = value;
        m_planetScript.m_shapeSettings.m_noiseLayers[0].m_noiseSettings.m_simpleNoiseSettings.m_centre[index] = value;
        m_planetScript.GeneratePlanet();
    }
}
