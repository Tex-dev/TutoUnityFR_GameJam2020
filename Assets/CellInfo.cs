using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CellInfo : MonoBehaviour
{
    public enum Content
    {
        dead = 0,
        plant = 1,
        herbivorus = 2,
        carnivorus = 4,
        water = 8,
    }

    public int X = 0;

    public int Y = 0;

    public Content GetContent => m_Content;

    public Content GetNextContent => m_NextContent;

    public Vector3 GrowthDeltas = Vector3.zero;

    public float PlantPopulation = 0f;

    public float HerbivorusPopulation = 0f;

    public float CarnivorusPopulation = 0f;

    public float TotalPopulation => PlantPopulation + HerbivorusPopulation + CarnivorusPopulation;

    [SerializeField]
    private Image m_PlantImage = null;

    [SerializeField]
    private Image m_HerbivorusImage = null;

    [SerializeField]
    private Image m_CarnivorusImage = null;

    private Content m_Content = Content.dead;

    private Content m_NextContent = Content.dead;

    private Button m_Button = null;

    private void Awake()
    {
        m_Button = GetComponent<Button>();
    }

    public void InitCell(Action<CellInfo> OnClick)
    {
        m_Button.onClick.AddListener(() => OnClick(this));

        m_Content = Content.dead;
        m_NextContent = Content.dead;

        SetContentManually(Content.dead);

        if (UnityEngine.Random.value > 0.8f)
        {
            m_Content = Content.water;
            m_PlantImage.fillAmount = 1f;
            m_PlantImage.color = Color.blue;
        }
    }

    public void SetNextContent(Content content)
    {
        m_NextContent = content;
    }

    public void SetContentManually(Content content, float populationToAdd = 10f)
    {
        if (m_Content == Content.water)
            return;

        m_Content = content;
        m_NextContent = content;

        switch (content)
        {
            case Content.plant:
                PlantPopulation += populationToAdd;

                break;

            case Content.herbivorus:
                HerbivorusPopulation += populationToAdd;

                break;

            case Content.carnivorus:
                CarnivorusPopulation += populationToAdd;

                break;

            case Content.dead:
                PlantPopulation = 0f;
                HerbivorusPopulation = 0f;
                CarnivorusPopulation = 0f;

                m_PlantImage.fillAmount = 0f;
                m_HerbivorusImage.fillAmount = 0f;
                m_CarnivorusImage.fillAmount = 0f;

                break;

            default:
                break;
        }

        UpdateContent();
    }

    public void UpdateContent()
    {
        if (m_Content == Content.water)
            return;

        if (TotalPopulation != PlantPopulation)
            m_PlantImage.fillAmount = TotalPopulation != 0f ? PlantPopulation / TotalPopulation : 0f;
        else
            m_PlantImage.fillAmount = PlantPopulation / GameOfLife.MAX_POPULATION_PER_CELL;

        if (TotalPopulation != HerbivorusPopulation)
            m_HerbivorusImage.fillAmount = TotalPopulation != 0f ? HerbivorusPopulation / TotalPopulation : 0f;
        else
            m_HerbivorusImage.fillAmount = HerbivorusPopulation / GameOfLife.MAX_POPULATION_PER_CELL;

        m_HerbivorusImage.transform.eulerAngles = new Vector3(0f, 0f, -m_PlantImage.fillAmount * 360f);

        if (TotalPopulation != CarnivorusPopulation)
            m_CarnivorusImage.fillAmount = TotalPopulation != 0f ? CarnivorusPopulation / TotalPopulation : 0f;
        else
            m_CarnivorusImage.fillAmount = CarnivorusPopulation / GameOfLife.MAX_POPULATION_PER_CELL;

        m_CarnivorusImage.transform.eulerAngles = new Vector3(0f, 0f, -(m_PlantImage.fillAmount + m_HerbivorusImage.fillAmount) * 360f);
    }
}