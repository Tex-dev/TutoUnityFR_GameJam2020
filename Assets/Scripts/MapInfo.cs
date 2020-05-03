using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class MapInfo : MonoBehaviour
{
    public float[,] CellHeighMap;

    public int ID = -1;

    public bool isGameOfLife = false;
    
    public void ActivateGameOfLife()
    {
        isGameOfLife = true;
    }

    private void OnMouseUp()
    {
        if (!isGameOfLife)
            return;

        if (EventSystem.current.currentSelectedGameObject == null)
            GameManager.SelectMesh(ID, CellHeighMap);
    }

    private void OnMouseEnter()
    {
        if (!isGameOfLife)
            return;

        transform.localScale += Vector3.one * 0.05f;
    }

    private void OnMouseExit()
    {
        if (!isGameOfLife)
            return;

        transform.localScale -= Vector3.one * 0.05f;
    }

    public float GetLiveableAreaPercent(float seaHeight)
    {
        float percent = 0.0f;

        int size = (int)Mathf.Sqrt(CellHeighMap.Length);
        for (int i = 0; i < size; i++)
        {
            for (int j = 0; j < size; j++)
            {
                if (CellHeighMap[i, j] >= seaHeight)
                    percent += 1.0f;
            }
        }

        percent /= CellHeighMap.Length;

        return percent;
    }
}