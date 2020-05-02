using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GameOfLife : MonoBehaviour
{
    private List<CellInfo> Cells = new List<CellInfo>();

    private CellInfo[,] CellsArray;

    public int GridWidth = 10;

    public int GridHeight = 10;

    public float PlaySpeed = 0.5f;

    private bool m_ShouldPlay = false;

    // Start is called before the first frame update
    private void Start()
    {
        Cells = GetComponentsInChildren<CellInfo>().ToList();

        CellsArray = new CellInfo[GridWidth, GridHeight];

        foreach (CellInfo cell in Cells)
        {
            CellsArray[cell.X, cell.Y] = cell;
            cell.InitCell(SelectCell);
        }
    }

    public void SelectCell(CellInfo cell)
    {
        switch (cell.m_Status)
        {
            case CellInfo.Status.alive:
                cell.UpdateStatus(CellInfo.Status.dead);
                break;

            case CellInfo.Status.dead:
                cell.UpdateStatus(CellInfo.Status.alive);
                break;

            default:
                break;
        }
    }

    public void Play()
    {
        m_ShouldPlay = true;
        StartCoroutine(LifeLogic());
    }

    public void Pause()
    {
        m_ShouldPlay = false;
    }

    private IEnumerator LifeLogic()
    {
        while (m_ShouldPlay)
        {
            foreach (CellInfo cell in Cells)
            {
                int n = GetNeighboursAmount(cell);

                if ((cell.m_Status == CellInfo.Status.alive && n == 2) || n == 3)
                {
                    cell.m_NextStatus = CellInfo.Status.alive;
                }
                else
                {
                    cell.m_NextStatus = CellInfo.Status.dead;
                }
            }

            foreach (CellInfo cell in Cells)
            {
                cell.UpdateStatus();
            }
            yield return new WaitForSeconds(PlaySpeed);
        }
    }

    private int GetNeighboursAmount(CellInfo cell)
    {
        int aliveNeighbors = 0;

        for (var i = -1; i <= 1; i += 1)
        {
            for (var j = -1; j <= 1; j += 1)
            {
                var neighborX = (cell.X + i + GridWidth) % GridWidth;
                var neighborY = (cell.Y + j + GridHeight) % GridHeight;

                if (neighborX != cell.X || neighborY != cell.Y)
                {
                    if (CellsArray[neighborX, neighborY].m_Status == CellInfo.Status.alive)
                    {
                        aliveNeighbors += 1;
                    }
                }
            }
        }

        return aliveNeighbors;
    }

    // Update is called once per frame
    private void Update()
    {
    }
}