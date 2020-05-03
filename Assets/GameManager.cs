using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    #region Singleton

    private static GameManager instance = null;

    // Game Instance Singleton
    public static GameManager Instance
    {
        get
        {
            return instance;
        }
    }

    private void SingletonStarter()
    {
        // if the singleton hasn't been initialized yet
        if (instance != null && instance != this)
        {
            Destroy(this.gameObject);
        }

        instance = this;
        DontDestroyOnLoad(this.gameObject);
    }

    #endregion Singleton

    public static float PlaySpeed => Instance.GamePlaySpeed;

    public float GamePlaySpeed = 1f;

    private LifeManager[] m_LifeLogics = null;

    public float WaterLevel = 0f;

    public int PlanetResolution = 256;

    [SerializeField]
    private LifeMachineHolder m_LifeManagerPrefab = null;

    [SerializeField]
    private GameObject m_TimeMenu = null;

    private void Awake()
    {
        SingletonStarter();

        m_LifeLogics = new LifeManager[6];

        for (int i = 0; i < 6; i++)
        {
            LifeMachineHolder holder = Instantiate(m_LifeManagerPrefab);

            m_LifeLogics[i] = holder.LifeManager;

            holder.gameObject.GetComponentInChildren<Canvas>().enabled = false;
        }
        m_TimeMenu.SetActive(false);
    }

    public static void SetPlanetParameters(float waterLevel, int planetResolution)
    {
        Instance.WaterLevel = waterLevel;

        Instance.PlanetResolution = planetResolution;
    }

    public static void SelectMesh(int ID, float[,] cellHeighMap)
    {
        for (int i = 0; i < 6; i++)
        {
            Instance.m_LifeLogics[i].transform.parent.parent.gameObject.GetComponent<Canvas>().enabled = false;
        }

        if (ID >= 0 && ID < Instance.m_LifeLogics.Length)
        {
            Instance.m_LifeLogics[ID].transform.parent.parent.gameObject.GetComponent<Canvas>().enabled = true;
            Instance.m_TimeMenu.SetActive(true);

            Instance.m_LifeLogics[ID].ConfigureGrid(cellHeighMap, Instance.WaterLevel, Instance.PlanetResolution, ID);
        }
    }

    public void Pause()
    {
        for (int i = 0; i < 6; i++)
        {
            m_LifeLogics[i].Pause();
        }
    }

    public void PlayAtSpeed(float newSpeed)
    {
        GamePlaySpeed = newSpeed;

        for (int i = 0; i < 6; i++)
        {
            m_LifeLogics[i].Play();
        }
    }
}