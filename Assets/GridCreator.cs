using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class GridCreator : MonoBehaviour
{
    public int GridWidth = 2;

    public int GridHeight = 2;

    public Vector2 XPadding = Vector2.zero;
    public Vector2 YPadding = Vector2.zero;

    public GameObject CellPrefab = null;

    public List<GameObject> Cells = new List<GameObject>();

    public void CreateGrid()
    {
        ClearGrid();

        RectTransform parentRect = GetComponent<RectTransform>();

        float xStep = 1f / (float)GridWidth;
        float yStep = 1f / (float)GridHeight;

        for (int x = 0; x < GridWidth; x++)
        {
            for (int y = 0; y < GridHeight; y++)
            {
                GameObject cell = Instantiate(CellPrefab, transform);

                RectTransform rect = cell.GetComponent<RectTransform>();

                rect.anchorMin = new Vector2(XPadding.x * (0.5f) + x * xStep, YPadding.x * (0.5f) + y * yStep);
                rect.anchorMax = new Vector2(-XPadding.y * (0.5f) + (x + 1f) * xStep, -YPadding.y * (0.5f) + (y + 1f) * yStep);

                cell.GetComponent<CellInfo>().X = x;
                cell.GetComponent<CellInfo>().Y = y;

                cell.name = $"Cell {x} {y}";

                Cells.Add(cell);
            }
        }
    }

    public void ClearGrid()
    {
        if (Cells != null)
        {
            for (int i = 0; i < Cells.Count; i++)
            {
                DestroyImmediate(Cells[i]);
            }
        }
    }
}

[CustomEditor(typeof(GridCreator))]
public class GridCreatorInspector : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        if (GUILayout.Button("Create grid"))
        {
            ((GridCreator)target).CreateGrid();
        }

        if (GUILayout.Button("Clear grid"))
        {
            ((GridCreator)target).ClearGrid();
        }
    }
}