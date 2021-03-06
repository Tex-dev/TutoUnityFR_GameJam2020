﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class LifeManager : MonoBehaviour
{
    [Header("Game parameters")]
    private List<CellInfo> Cells = new List<CellInfo>();

    private CellInfo[,] CellsArray;

    private int m_GridWidth = 10;

    private int m_GridHeight = 10;

    private bool m_ShouldPlay = false;

    public int PlantSeeds = 10;

    public int HerbivorusSeeds = 10;

    public int CarnivorusSeeds = 10;

    [Header("Prey predator model")]
    [SerializeField]
    private float m_PlantGrowth = 2f;

    [SerializeField]
    private float m_PlantHerbivorusMeetingRate = 1f;

    [SerializeField]
    private float m_PlantEmigrationRate = 0.1f;

    [SerializeField]
    private float m_HerbivorusGrowth = 2f;

    [SerializeField]
    private float m_HerbivorusLossRate = 0.5f;

    [SerializeField]
    private float m_HerbivorusEmigrationRate = 0.02f;

    [SerializeField]
    private float m_HerbivorusCarnivorusMeetingRate = 1f;

    [SerializeField]
    private float m_CarnivorusGrowth = 1f;

    [SerializeField]
    private float m_CarnivorusLossRate = 0.5f;

    [SerializeField]
    private float m_CarnivorusEmigrationRate = 0.01f;

    public const int MAX_POPULATION_PER_CELL = 10000;

    private bool m_Configured = false;

    // Start is called before the first frame update
    private void Start()
    {
        m_GridWidth = GetComponent<GridCreator>().GridWidth;
        m_GridHeight = GetComponent<GridCreator>().GridHeight;

        Cells = GetComponentsInChildren<CellInfo>().ToList();

        CellsArray = new CellInfo[m_GridWidth, m_GridHeight];

        foreach (CellInfo cell in Cells)
        {
            CellsArray[cell.X, cell.Y] = cell;
            cell.InitCell(SelectCell);
        }
    }

    public void Reset()
    {
        foreach (CellInfo cell in Cells)
        {
            if (cell.GetContent != CellInfo.Content.water)
            {
                cell.SetContentManually(CellInfo.Content.dead);
            }
        }

        PlantSeeds = 10;

        HerbivorusSeeds = 10;

        CarnivorusSeeds = 10;

        UpdateSeedDisplay();
    }

    public void ConfigureGrid(float[,] heighMap, float waterLevel, int planetResolution, int meshID)
    {
        if (m_Configured)
            return;

        bool[,] drowned = new bool[planetResolution, planetResolution];

        for (int i = 0; i < planetResolution; i++)
        {
            for (int j = 0; j < planetResolution; j++)
            {
                if (heighMap[i, j] < waterLevel)
                    drowned[i, j] = true;
                else
                    drowned[i, j] = false;
            }
        }

        int caseWidth = planetResolution / m_GridWidth;
        int caseHeight = planetResolution / m_GridHeight;

        for (int i = 0; i < m_GridWidth; i++)
        {
            for (int j = 0; j < m_GridHeight; j++)
            {
                int nbDrowned = 0;

                for (int k = 0; k < caseWidth; k++)
                {
                    for (int l = 0; l < caseHeight; l++)
                    {
                        if (drowned[i * caseWidth + k, j * caseHeight + l])
                            nbDrowned++;
                    }
                }

                if (nbDrowned > caseWidth * caseHeight * 0.8f)
                {
                    CellsArray[i, j].SetContentManually(CellInfo.Content.water);
                }
                else
                {
                }
            }
        }

        m_Configured = true;
    }

    public void UpdateSeedDisplay()
    {
        GameManager.Instance.UpdateSeed(PlantSeeds, CellInfo.Content.plant);
        GameManager.Instance.UpdateSeed(HerbivorusSeeds, CellInfo.Content.herbivorus);
        GameManager.Instance.UpdateSeed(CarnivorusSeeds, CellInfo.Content.carnivorus);
    }

    #region Player interactions

    public void SelectCell(CellInfo cell)
    {
        int currentSeedValue = 0;

        switch (GameManager.Instance.CurrentContentMode)
        {
            case CellInfo.Content.plant:

                if (PlantSeeds < 1)
                    return;
                PlantSeeds--;
                currentSeedValue = PlantSeeds;
                break;

            case CellInfo.Content.herbivorus:
                if (HerbivorusSeeds < 1)
                    return;
                HerbivorusSeeds--;
                currentSeedValue = HerbivorusSeeds;
                break;

            case CellInfo.Content.carnivorus:
                if (CarnivorusSeeds < 1)
                    return;
                CarnivorusSeeds--;
                currentSeedValue = CarnivorusSeeds;
                break;

            default:
                break;
        }

        cell.SetContentManually(GameManager.Instance.CurrentContentMode, 200f);

        GameManager.Instance.UpdateSeed(currentSeedValue);
    }

    public void SelectMap(MapInfo map)
    {
        Debug.Log(map.name);
    }

    #region Time management

    public void Play()
    {
        if (!m_ShouldPlay)
        {
            m_ShouldPlay = true;
            StartCoroutine(LifeLogic());
        }
    }

    public void Pause()
    {
        m_ShouldPlay = false;
    }

    #endregion Time management

    #endregion Player interactions

    #region Game of life Logic

    public int PlantPopulation()
    {
        int pop = 0;

        foreach (CellInfo cell in Cells)
        {
            pop += (int)cell.PlantPopulation;
        }

        return pop;
    }

    public int HerbivorusPopulation()
    {
        int pop = 0;

        foreach (CellInfo cell in Cells)
        {
            pop += (int)cell.HerbivorusPopulation;
        }

        return pop;
    }

    public int CarnivorusPopulation()
    {
        int pop = 0;

        foreach (CellInfo cell in Cells)
        {
            pop += (int)cell.CarnivorusPopulation;
        }

        return pop;
    }

    private IEnumerator LifeLogic()
    {
        while (m_ShouldPlay)
        {
            foreach (CellInfo cell in Cells)
            {
                if (cell.GetContent != CellInfo.Content.water)
                {
                    PreyPredatorModel(cell);

                    VegetalLogic(cell);

                    HerbivorusLogic(cell);

                    CarnivorusLogic(cell);
                }
            }

            foreach (CellInfo cell in Cells)
            {
                cell.UpdateContent();
            }
            yield return new WaitForEndOfFrame();
        }
    }

    private void PreyPredatorModel(CellInfo cell)
    {
        float deltaPlant = m_PlantGrowth * cell.PlantPopulation - m_PlantHerbivorusMeetingRate * cell.PlantPopulation * cell.HerbivorusPopulation;

        float deltaHerbivorus = m_HerbivorusGrowth * cell.PlantPopulation * cell.HerbivorusPopulation - m_HerbivorusLossRate * cell.HerbivorusPopulation;

        deltaHerbivorus = deltaHerbivorus - m_HerbivorusCarnivorusMeetingRate * cell.HerbivorusPopulation * cell.CarnivorusPopulation;

        float deltaCarnivorus = m_CarnivorusGrowth * cell.HerbivorusPopulation * cell.CarnivorusPopulation - m_CarnivorusLossRate * cell.CarnivorusPopulation;

        deltaPlant *= Time.deltaTime * 1f / GameManager.PlaySpeed;
        deltaHerbivorus *= Time.deltaTime * 1f / GameManager.PlaySpeed;
        deltaCarnivorus *= Time.deltaTime * 1f / GameManager.PlaySpeed;

        cell.PlantPopulation += deltaPlant;
        cell.PlantPopulation = Mathf.Floor(Mathf.Clamp(cell.PlantPopulation, 0f, MAX_POPULATION_PER_CELL));

        cell.HerbivorusPopulation += deltaHerbivorus;
        cell.HerbivorusPopulation = Mathf.Floor(Mathf.Clamp(cell.HerbivorusPopulation, 0f, MAX_POPULATION_PER_CELL));

        cell.CarnivorusPopulation += deltaCarnivorus;
        cell.CarnivorusPopulation = Mathf.Floor(Mathf.Clamp(cell.CarnivorusPopulation, 0f, MAX_POPULATION_PER_CELL));

        cell.GrowthDeltas = new Vector3(deltaPlant, deltaHerbivorus, deltaCarnivorus);
    }

    private void CarnivorusLogic(CellInfo cell)
    {
        Tuple<int, float> result = GetNeighboursAmount(cell, CellInfo.Content.carnivorus);

        if (result.Item1 != 0f && result.Item2 > 0f)
            cell.CarnivorusPopulation += Mathf.Floor((float)result.Item2 / (float)result.Item1) * m_CarnivorusEmigrationRate;
    }

    private void HerbivorusLogic(CellInfo cell)
    {
        Tuple<int, float> result = GetNeighboursAmount(cell, CellInfo.Content.herbivorus);

        if (result.Item1 != 0f && result.Item2 > 0f)
            cell.HerbivorusPopulation += Mathf.Floor((float)result.Item2 / (float)result.Item1) * m_HerbivorusEmigrationRate;
    }

    private void VegetalLogic(CellInfo cell)
    {
        Tuple<int, float> result = GetNeighboursAmount(cell, CellInfo.Content.plant);

        if (result.Item1 != 0f && result.Item2 > 0f)
            cell.PlantPopulation += (Mathf.Floor((float)result.Item2 / (float)result.Item1)) * m_PlantEmigrationRate;
    }

    private Tuple<int, float> GetNeighboursAmount(CellInfo cell, CellInfo.Content type)
    {
        Tuple<int, float> result = new Tuple<int, float>(0, 0f);

        for (var i = -1; i <= 1; i += 1)
        {
            for (var j = -1; j <= 1; j += 1)
            {
                var neighborX = (cell.X + i + m_GridWidth) % m_GridWidth;
                var neighborY = (cell.Y + j + m_GridHeight) % m_GridHeight;

                if (cell.X != neighborX - 1 && cell.X != neighborX && cell.X != neighborX + 1) continue;
                if (cell.Y != neighborY - 1 && cell.Y != neighborY && cell.Y != neighborY + 1) continue;

                if (neighborX != cell.X || neighborY != cell.Y)
                {
                    switch (type)
                    {
                        case CellInfo.Content.plant:
                            if (CellsArray[neighborX, neighborY].PlantPopulation > 0f)
                            {
                                result = new Tuple<int, float>(result.Item1 + 1, result.Item2 + CellsArray[neighborX, neighborY].GrowthDeltas.x);
                            }
                            break;

                        case CellInfo.Content.herbivorus:
                            if (CellsArray[neighborX, neighborY].HerbivorusPopulation > 0f)
                            {
                                result = new Tuple<int, float>(result.Item1 + 1, result.Item2 + CellsArray[neighborX, neighborY].GrowthDeltas.y);
                            }
                            break;

                        case CellInfo.Content.carnivorus:
                            if (CellsArray[neighborX, neighborY].CarnivorusPopulation > 0f)
                            {
                                result = new Tuple<int, float>(result.Item1 + 1, result.Item2 + CellsArray[neighborX, neighborY].GrowthDeltas.z);
                            }
                            break;

                        default:
                            break;
                    }
                }
            }
        }

        return result;
    }

    #endregion Game of life Logic
}