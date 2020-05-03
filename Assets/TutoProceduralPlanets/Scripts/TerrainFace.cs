using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TerrainFace
{
    private ShapeGenerator m_shapeGenerator;
    private Mesh m_mesh;
    private int m_res;
    private Vector3 m_localUp;
    private Vector3 m_axisA;
    private Vector3 m_axisB;

    public TerrainFace(ShapeGenerator shapeGenerator, Mesh mesh, int res, Vector3 localUp)
    {
        m_shapeGenerator = shapeGenerator;

        m_mesh = mesh;
        m_res = res;
        m_localUp = localUp;

        m_axisA = new Vector3(m_localUp.y, m_localUp.z, m_localUp.x);
        m_axisB = Vector3.Cross(m_localUp, m_axisA);
    }

    public float[,] CellHeighMap;

    public void ConstructMesh()
    {
        Vector3[] vertices = new Vector3[m_res * m_res];
        int[] triangles = new int[(m_res - 1) * (m_res - 1) * 2 * 3];
        int triangleIndex = 0;
        Vector2[] uv = m_mesh.uv;

        CellHeighMap = new float[m_res, m_res];

        for (int y = 0; y < m_res; y++)
        {
            for (int x = 0; x < m_res; x++)
            {
                int i = x + y * m_res;
                Vector2 percent = new Vector2(x, y) / (m_res - 1);
                Vector3 pointOnUnitCube = m_localUp + (percent.x - 0.5f) * 2.0f * m_axisA + (percent.y - 0.5f) * 2.0f * m_axisB;
                Vector3 pointOnUnitSphere = pointOnUnitCube.normalized;
                vertices[i] = m_shapeGenerator.CalculatePointOnPlanet(pointOnUnitSphere);

                float elevation = vertices[i].x / pointOnUnitSphere.x;
                CellHeighMap[x, y] = elevation;

                if (y != m_res - 1 && x != m_res - 1)
                {
                    triangles[triangleIndex + 0] = i;
                    triangles[triangleIndex + 1] = i + m_res + 1;
                    triangles[triangleIndex + 2] = i + m_res;

                    triangles[triangleIndex + 3] = i;
                    triangles[triangleIndex + 4] = i + 1;
                    triangles[triangleIndex + 5] = i + m_res + 1;

                    triangleIndex += 6;
                }
            }
        }

        m_mesh.Clear();
        m_mesh.vertices = vertices;
        m_mesh.triangles = triangles;
        m_mesh.RecalculateNormals();
        m_mesh.uv = uv;
    }

    public void UpdateUVs(ColorGenerator colorGenerator)
    {
        Vector2[] uv = new Vector2[m_res * m_res];

        for (int y = 0; y < m_res; y++)
        {
            for (int x = 0; x < m_res; x++)
            {
                int i = x + y * m_res;
                Vector2 percent = new Vector2(x, y) / (m_res - 1);
                Vector3 pointOnUnitCube = m_localUp + (percent.x - 0.5f) * 2.0f * m_axisA + (percent.y - 0.5f) * 2.0f * m_axisB;
                Vector3 pointOnUnitSphere = pointOnUnitCube.normalized;

                uv[i] = new Vector2(colorGenerator.BiomePercentFromPoint(pointOnUnitSphere), 0);
            }
        }
        m_mesh.uv = uv;
    }
}