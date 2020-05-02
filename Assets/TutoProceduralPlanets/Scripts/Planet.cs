using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Planet : MonoBehaviour
{
    [Range(2, 256)]
    public int m_res = 150;
    public bool autoUpdate = true;

    public enum FaceRenderMask { All, Top, Bottom, Left, Right, Front, Back};
    public FaceRenderMask   m_faceRenderMask;

    public ShapeSettings    m_shapeSettings;
    public ColorSettings    m_colorSettings;

    [HideInInspector]
    public bool             m_shapeSettingsFoldout;
    [HideInInspector]
    public bool             m_colorSettingsFoldout;

    ShapeGenerator          m_shapeGenerator = new ShapeGenerator();
    ColorGenerator          m_colorGenerator = new ColorGenerator();

    [SerializeField, HideInInspector]
    MeshFilter[]    m_meshFilters;
    TerrainFace[]   m_terrainFaces;

    void Start()
    {
        GeneratePlanet();
    }

    void Initialize()
    {
        m_shapeGenerator.UpdateSettings(m_shapeSettings);
        m_colorGenerator.UpdateSettings(m_colorSettings);

        if (m_meshFilters == null || m_meshFilters.Length == 0)
            m_meshFilters = new MeshFilter[6];
    
        m_terrainFaces = new TerrainFace[6];

        Vector3[] directions =
        {
            Vector3.up,
            Vector3.down,
            Vector3.left,
            Vector3.right,
            Vector3.forward,
            Vector3.back
        };

        for (int i = 0; i < 6; i++)
        {

            if (m_meshFilters[i] == null || m_meshFilters[i].sharedMesh == null)
            {
                GameObject meshObj = new GameObject("Mesh");
                meshObj.transform.parent = transform;

                meshObj.AddComponent<MeshRenderer>();
                m_meshFilters[i] = meshObj.AddComponent<MeshFilter>();
                m_meshFilters[i].sharedMesh = new Mesh();
            }

            m_meshFilters[i].GetComponent<MeshRenderer>().sharedMaterial = m_colorSettings.m_planetMat;

            m_terrainFaces[i] = new TerrainFace(m_shapeGenerator, m_meshFilters[i].sharedMesh, m_res, directions[i]);
            bool renderFace = m_faceRenderMask == FaceRenderMask.All || (int)m_faceRenderMask - 1 == i;
            m_meshFilters[i].gameObject.SetActive(renderFace);
        }
    }

    public void GeneratePlanet()
    {
        Initialize();
        GenerateMesh();
        GenerateColors();
    }

    public void OnShapeSettingsUpdated()
    {
        if (autoUpdate)
        {
            Initialize();
            GenerateMesh();
        }
    }

    public void OnColorSettingsUpdated()
    {
        if (autoUpdate)
        {
            Initialize();
            GenerateColors();
        }
    }

    void GenerateMesh()
    {
        for (int i = 0; i < 6; i++)
        {
            if (m_meshFilters[i].gameObject.activeSelf)
                m_terrainFaces[i].ConstructMesh();
        }

        m_colorGenerator.UpdateElevation(m_shapeGenerator.m_elevationMinMax);
    }

    void GenerateColors()
    {
        for (int i = 0; i < 6; i++)
        {
            if (m_meshFilters[i].gameObject.activeSelf)
                m_terrainFaces[i].UpdateUVs(m_colorGenerator);
        }

        m_colorGenerator.UpdateColor();
    }
}
