using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StatesViewer : MonoBehaviour
{
    [SerializeField]
    private Image m_PlantImage = null;

    [SerializeField]
    private Image m_HerbivorusImage = null;

    [SerializeField]
    private Image m_CarnivorusImage = null;

    private float TotalPopulation => GameManager.Instance.TotalPopulation();

    private float PlantPopulation => GameManager.Instance.TotalPlantPopulation();

    private float HerbivorusPopulation => GameManager.Instance.TotalHerbivorusPopulation();

    private float CarnivorusPopulation => GameManager.Instance.TotalCarnivorusPopulation();

    private void Update()
    {
        if (TotalPopulation == 0)
            return;

        m_PlantImage.fillAmount = PlantPopulation / TotalPopulation;

        m_HerbivorusImage.fillAmount = HerbivorusPopulation / TotalPopulation;

        m_HerbivorusImage.transform.eulerAngles = new Vector3(0f, 0f, -m_PlantImage.fillAmount * 360f);

        m_CarnivorusImage.fillAmount = CarnivorusPopulation / TotalPopulation;

        m_CarnivorusImage.transform.eulerAngles = new Vector3(0f, 0f, -(m_PlantImage.fillAmount + m_HerbivorusImage.fillAmount) * 360f);
    }
}