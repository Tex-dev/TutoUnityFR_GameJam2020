using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GameOfLife : MonoBehaviour
{
    private List<CellInfo> Cells = new List<CellInfo>();

    private CellInfo[,] CellsArray;

    // Start is called before the first frame update
    private void Start()
    {
        Cells = GetComponentsInChildren<CellInfo>().ToList();

        CellsArray = new CellInfo[10, 10];

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

    private IEnumerator LifeLogic()
    {
        while (Application.isPlaying)
        {
            yield return new WaitForSeconds(5f);
        }
    }

    private int GetNeighboursAmount(CellInfo cell)
    {
        int neighbours = 0;

        return neighbours;
    }

    // Update is called once per frame
    private void Update()
    {
    }
}