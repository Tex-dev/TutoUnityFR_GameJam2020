using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Planet : MonoBehaviour
{
    [Range(2, 256)]
    public int m_res = 150;

    public bool autoUpdate = true;

    public enum FaceRenderMask { All, Top, Bottom, Left, Right, Front, Back };

    public FaceRenderMask m_faceRenderMask;

    public ShapeSettings m_shapeSettings;
    public ColorSettings m_colorSettings;

    [HideInInspector]
    public bool m_shapeSettingsFoldout;

    [HideInInspector]
    public bool m_colorSettingsFoldout;

    private ShapeGenerator m_shapeGenerator = new ShapeGenerator();
    private ColorGenerator m_colorGenerator = new ColorGenerator();

    [SerializeField, HideInInspector]
    private MeshFilter[] m_meshFilters;

    private TerrainFace[] m_terrainFaces;

    [SerializeField]
    private Canvas TutorialCanvas = null;

    [SerializeField]
    private Toggle TutorialToggle = null;

    private void Start()
    {
        GeneratePlanet();
    }

    private void Initialize()
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

    public float GetLiveableAreaPercent()
    {
        float liveableAreaPercent = 0.0f;

        for (int i = 0; i < 6; i++)
        {
            if (m_meshFilters[i].gameObject.activeSelf)
            {
                MapInfo mapInfo = m_meshFilters[i].gameObject.GetComponent<MapInfo>();
                if (mapInfo == null)
                    return 0.0f;

                liveableAreaPercent += mapInfo.GetLiveableAreaPercent(m_shapeGenerator.m_elevationMinMax.Min + (m_shapeGenerator.m_elevationMinMax.Max - m_shapeGenerator.m_elevationMinMax.Min) * 5f / 100f);
            }
        }

        liveableAreaPercent /= 6.0f;

        return liveableAreaPercent;
    }

    public void ActivateGameOfLife()
    {
        for (int i = 0; i < 6; i++)
        {
            if (m_meshFilters[i].gameObject.activeSelf)
            {
                MapInfo mapInfo = m_meshFilters[i].gameObject.GetComponent<MapInfo>();
                if (mapInfo == null)
                    return;

                mapInfo.ActivateGameOfLife();
            }
        }
        TutorialCanvas.gameObject.SetActive(true);
        TutorialToggle.gameObject.SetActive(true);
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

    private void GenerateMesh()
    {
        for (int i = 0; i < 6; i++)
        {
            if (m_meshFilters[i].gameObject.activeSelf)
            {
                m_terrainFaces[i].ConstructMesh();

                MapInfo mapInfo = m_meshFilters[i].gameObject.GetComponent<MapInfo>();
                if (mapInfo == null)
                    mapInfo = m_meshFilters[i].gameObject.AddComponent<MapInfo>();

                MeshCollider collider = m_meshFilters[i].gameObject.GetComponent<MeshCollider>();
                if (collider == null)
                    m_meshFilters[i].gameObject.AddComponent<MeshCollider>();

                mapInfo.CellHeighMap = m_terrainFaces[i].CellHeighMap;
                mapInfo.ID = i;
            }
        }

        m_colorGenerator.UpdateElevation(m_shapeGenerator.m_elevationMinMax);

        GameManager.SetPlanetParameters(m_shapeGenerator.m_elevationMinMax.Min + (m_shapeGenerator.m_elevationMinMax.Max - m_shapeGenerator.m_elevationMinMax.Min) * 5f / 100f, m_res);
    }

    private void GenerateColors()
    {
        for (int i = 0; i < 6; i++)
        {
            if (m_meshFilters[i].gameObject.activeSelf)
                m_terrainFaces[i].UpdateUVs(m_colorGenerator);
        }

        m_colorGenerator.UpdateColor();
    }
}