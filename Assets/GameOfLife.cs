using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GameOfLife : MonoBehaviour
{
    [Header("Game parameters")]
    private List<CellInfo> Cells = new List<CellInfo>();

    private CellInfo[,] CellsArray;

    private int m_GridWidth = 10;

    private int m_GridHeight = 10;

    public float PlaySpeed = 0.5f;

    private bool m_ShouldPlay = false;

    private CellInfo.Content m_CurrentContentMode = CellInfo.Content.dead;

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

    public void SetMode(int mode)
    {
        m_CurrentContentMode = (CellInfo.Content)mode;
    }

    public void SelectCell(CellInfo cell)
    {
        cell.SetContentManually(m_CurrentContentMode, 200f);
    }

    private void Play()
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

    public void SpeedChange(float newSpeed)
    {
        PlaySpeed = newSpeed;
        Play();
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
            yield return new WaitForEndOfFrame();//new WaitForSeconds(PlaySpeed);
        }
    }

    private void PreyPredatorModel(CellInfo cell)
    {
        float deltaPlant = m_PlantGrowth * cell.PlantPopulation - m_PlantHerbivorusMeetingRate * cell.PlantPopulation * cell.HerbivorusPopulation;

        float deltaHerbivorus = m_HerbivorusGrowth * cell.PlantPopulation * cell.HerbivorusPopulation - m_HerbivorusLossRate * cell.HerbivorusPopulation;

        deltaHerbivorus = deltaHerbivorus - m_HerbivorusCarnivorusMeetingRate * cell.HerbivorusPopulation * cell.CarnivorusPopulation;

        float deltaCarnivorus = m_CarnivorusGrowth * cell.HerbivorusPopulation * cell.CarnivorusPopulation - m_CarnivorusLossRate * cell.CarnivorusPopulation;

        deltaPlant *= Time.deltaTime * 1f / PlaySpeed;
        deltaHerbivorus *= Time.deltaTime * 1f / PlaySpeed;
        deltaCarnivorus *= Time.deltaTime * 1f / PlaySpeed;

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
}