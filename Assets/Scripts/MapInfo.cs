using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapInfo : MonoBehaviour
{
    public float[,] CellHeighMap;

    public int ID = -1;

    private void OnMouseUp()
    {
        GameManager.SelectMesh(ID, CellHeighMap);
    }

    private void OnMouseEnter()
    {
        transform.localScale += Vector3.one * 0.05f;
    }

    private void OnMouseExit()
    {
        transform.localScale -= Vector3.one * 0.05f;
    }
}