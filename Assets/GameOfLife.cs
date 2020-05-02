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

    private CellInfo.Content m_CurrentContentMode = CellInfo.Content.dead;

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

    public void SetMode(int mode)
    {
        m_CurrentContentMode = (CellInfo.Content)mode;
    }

    public void SelectCell(CellInfo cell)
    {
        if (cell.GetContent != m_CurrentContentMode)
        {
            cell.SetContent(m_CurrentContentMode);
        }
        else
        {
            cell.SetContent(CellInfo.Content.dead);
        }
    }

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

    private IEnumerator LifeLogic()
    {
        while (m_ShouldPlay)
        {
            foreach (CellInfo cell in Cells)
            {
                //if (cell.GetContent == CellInfo.Content.vegetal)
                //{
                //    VegetalLogic(cell);
                //}
                VegetalLogic(cell);

                VegetarianLogic(cell);

                CarnivorusLogic(cell);
            }

            foreach (CellInfo cell in Cells)
            {
                cell.UpdateContent();
            }
            yield return new WaitForSeconds(PlaySpeed);
        }
    }

    private void CarnivorusLogic(CellInfo cell)
    {
        int n = GetNeighboursAmount(cell, CellInfo.Content.carnivorus);

        //                   != CellInfo.Content.dead
        if ((cell.GetContent == CellInfo.Content.carnivorus && n == 2) || (n == 3 && GetNeighboursAmount(cell, CellInfo.Content.vegetarian) >= 1))
        {
            cell.SetNextContent(CellInfo.Content.carnivorus);
        }
        else
        {
            if (cell.GetContent == CellInfo.Content.carnivorus)
                cell.SetNextContent(CellInfo.Content.dead);
        }
    }

    private void VegetarianLogic(CellInfo cell)
    {
        int n = GetNeighboursAmount(cell, CellInfo.Content.vegetarian);

        //if (n == 2)
        //{
        //    float chance = Random.Range(0f, 1f);
        //    chance -= GetNeighboursAmount(cell, CellInfo.Content.vegetal) / 6f;
        //    if (chance < 0f)
        //        cell.SetNextContent(CellInfo.Content.vegetarian);
        //}
        //else
        //if (n >= 4)
        //    cell.SetNextContent(CellInfo.Content.dead);
        //else
        //{
        //    if (cell.GetContent == CellInfo.Content.vegetarian)
        //        cell.SetNextContent(CellInfo.Content.dead);
        //}

        if ((cell.GetContent == CellInfo.Content.vegetarian && n == 2) || (n == 3 && GetNeighboursAmount(cell, CellInfo.Content.vegetal) >= 1))
        {
            cell.SetNextContent(CellInfo.Content.vegetarian);
        }
        else
        {
            if (cell.GetContent == CellInfo.Content.vegetarian)
                cell.SetNextContent(CellInfo.Content.dead);
        }
    }

    private void VegetalLogic(CellInfo cell)
    {
        int n = GetNeighboursAmount(cell, CellInfo.Content.vegetal);

        if (/*(cell.GetContent != CellInfo.Content.dead && n == 2) ||*/ n >= 2)
        {
            cell.SetNextContent(CellInfo.Content.vegetal);
        }
        //else
        //{
        //    if (cell.GetContent == CellInfo.Content.vegetarian)
        //        cell.SetNextContent(CellInfo.Content.dead);
        //}
        //for (var i = -1; i <= 1; i += 1)
        //{
        //    for (var j = -1; j <= 1; j += 1)
        //    {
        //        var neighborX = (cell.X + i + GridWidth) % GridWidth;
        //        var neighborY = (cell.Y + j + GridHeight) % GridHeight;

        //        if (cell.X != neighborX - 1 && cell.X != neighborX && cell.X != neighborX + 1) continue;
        //        if (cell.Y != neighborY - 1 && cell.Y != neighborY && cell.Y != neighborY + 1) continue;

        //        if (neighborX == cell.X || neighborY == cell.Y)
        //            CellsArray[neighborX, neighborY].SetNextContent(CellInfo.Content.vegetal);
        //    }
        //}
    }

    private int GetNeighboursAmount(CellInfo cell, CellInfo.Content type)
    {
        int aliveNeighbors = 0;

        for (var i = -1; i <= 1; i += 1)
        {
            for (var j = -1; j <= 1; j += 1)
            {
                var neighborX = (cell.X + i + GridWidth) % GridWidth;
                var neighborY = (cell.Y + j + GridHeight) % GridHeight;

                if (cell.X != neighborX - 1 && cell.X != neighborX && cell.X != neighborX + 1) continue;
                if (cell.Y != neighborY - 1 && cell.Y != neighborY && cell.Y != neighborY + 1) continue;

                if (neighborX != cell.X || neighborY != cell.Y)
                {
                    if (CellsArray[neighborX, neighborY].GetContent == type)
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