using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

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

    [Header("Player setting")]
    public float GamePlaySpeed = 1f;

    public float WaterLevel = 0f;
    public int PlanetResolution = 256;

    public long PlanetAge = 0;

    [Header("Picker")]
    public Image PickerBackground = null;

    public Image PickerIcon = null;

    public Sprite PlantIcon = null;

    public Sprite HerbivorusIcon = null;

    public Sprite CarnivorusIcon = null;

    public Sprite DeleteIcon = null;

    public Color PlantColor = Color.white;

    public Color HerbivorusColor = Color.white;

    public Color CarnivorusColor = Color.white;

    public Color DeleteColor = Color.white;

    private LifeManager[] m_LifeLogics = null;

    private LifeManager m_CurrentLifeManager = null;

    public CellInfo.Content CurrentContentMode = CellInfo.Content.dead;

    [Header("Objects")]
    [SerializeField]
    private LifeMachineHolder m_LifeManagerPrefab = null;

    [SerializeField]
    private GameObject m_Menu = null;

    [SerializeField]
    private Text m_PlantSeedText = null;

    [SerializeField]
    private Text m_HerbivorusSeedText = null;

    [SerializeField]
    private Text m_CarnivorusSeedText = null;

    [SerializeField]
    private Text m_StatsDisplay = null;

    [SerializeField]
    private Text m_PlanetAgeDisplay = null;

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
        m_Menu.SetActive(false);
    }

    private void Update()
    {
        if (GamePlaySpeed != 0f)
            PlanetAge += (long)((Time.deltaTime / GamePlaySpeed) * 100f);

        m_PlanetAgeDisplay.text = $"Planet age : {PlanetAge} years";

        m_StatsDisplay.text =
            $"Plants pop.  \t:\t<color=green>{TotalPlantPopulation()}</color>\n" +
            $"Rabbits pop. \t:\t<color=yellow>{TotalHerbivorusPopulation()}</color>\n" +
            $"Foxes pop.   \t:\t<color=red>{TotalCarnivorusPopulation()}</color>\n";
    }

    public int TotalPopulation()
    {
        return TotalPlantPopulation() + TotalHerbivorusPopulation() + TotalCarnivorusPopulation();
    }

    public int TotalPlantPopulation()
    {
        int pop = 0;

        for (int i = 0; i < 6; i++)
        {
            pop += Instance.m_LifeLogics[i].PlantPopulation();
        }

        return pop;
    }

    public int TotalHerbivorusPopulation()
    {
        int pop = 0;

        for (int i = 0; i < 6; i++)
        {
            pop += Instance.m_LifeLogics[i].HerbivorusPopulation();
        }

        return pop;
    }

    public int TotalCarnivorusPopulation()
    {
        int pop = 0;

        for (int i = 0; i < 6; i++)
        {
            pop += Instance.m_LifeLogics[i].CarnivorusPopulation();
        }

        return pop;
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
            Instance.gameObject.GetComponentInChildren<Canvas>().enabled = true; ;

            Instance.m_LifeLogics[ID].transform.parent.parent.gameObject.GetComponent<Canvas>().enabled = true;
            Instance.m_Menu.SetActive(true);

            Instance.m_CurrentLifeManager = Instance.m_LifeLogics[ID];

            Instance.m_CurrentLifeManager.ConfigureGrid(cellHeighMap, Instance.WaterLevel, Instance.PlanetResolution, ID);
            Instance.m_CurrentLifeManager.UpdateSeedDisplay();

            Instance.m_CurrentLifeManager.Play();
        }
    }

    public void Reset()
    {
        m_CurrentLifeManager.Reset();
    }

    public void SetMode(int mode)
    {
        CurrentContentMode = (CellInfo.Content)mode;

        CellInfo.Content currentMode = (CellInfo.Content)mode;

        switch (currentMode)
        {
            case CellInfo.Content.dead:
                PickerBackground.color = DeleteColor;
                PickerIcon.sprite = DeleteIcon;
                break;

            case CellInfo.Content.plant:
                PickerBackground.color = PlantColor;
                PickerIcon.sprite = PlantIcon;
                break;

            case CellInfo.Content.herbivorus:
                PickerBackground.color = HerbivorusColor;
                PickerIcon.sprite = HerbivorusIcon;
                break;

            case CellInfo.Content.carnivorus:
                PickerBackground.color = CarnivorusColor;
                PickerIcon.sprite = CarnivorusIcon;
                break;

            default:
                break;
        }
    }

    public void UpdateSeed(int value, CellInfo.Content content = CellInfo.Content.dead)
    {
        if (content == CellInfo.Content.dead)
            content = CurrentContentMode;

        switch (content)
        {
            case CellInfo.Content.plant:
                m_PlantSeedText.text = $"seed : {value}";
                break;

            case CellInfo.Content.herbivorus:
                m_HerbivorusSeedText.text = $"seed : {value}";
                break;

            case CellInfo.Content.carnivorus:
                m_CarnivorusSeedText.text = $"seed : {value}";
                break;

            default:
                break;
        }
    }

    public void Pause()
    {
        GamePlaySpeed = 0f;
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