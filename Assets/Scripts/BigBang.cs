using System;
using UnityEngine;

public class BigBang : MonoBehaviour
{
    [SerializeField]
    private GameObject              m_PlanetPrefab = null;
    [SerializeField]
    private GameObject[]            m_SunPrefab = null;
    [SerializeField]
    private GameObject              m_exoplanete = null;
    [SerializeField]
    private ParticleSystem          m_particles = null;

    [SerializeField]
    private int                     m_NbPlanet = 0;

    private bool                    m_ok = false;

    [SerializeField]
    private BlinkTextAnimator       m_textAnim = null;

    [SerializeField]
    private float                   m_expansionTime = 0.0f;

    private bool                    m_isBigBangHappened = false;

    public Action                   OnExpansionEnded;

    [SerializeField]
    private Material[]              m_materials = null;



    // Start is called before the first frame update
    void Start()
    {
        m_exoplanete.SetActive(false);
        m_textAnim.OnAnimationEnded += () => m_ok = true;
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.A) && m_ok && !m_isBigBangHappened)
        {
            m_textAnim.gameObject.SetActive(false);

            BeginExpansion();
            Invoke("EndExpansion", m_expansionTime);

            m_isBigBangHappened = true;
        }
    }

    private void BeginExpansion()
    {
        m_exoplanete.SetActive(true);

        /*
        go = new GameObject();
        go.transform.parent = transform;
        go.AddComponent<Planet>();
        go.AddComponent<BigBangPlanet>();

        Planet planet = go.GetComponent<Planet>();

        planet.m_shapeSettings = m_shapeSettings;
        planet.m_colorSettings = m_colorSettings;

        planet.GeneratePlanet();//*/

        for (int i = 0; i < m_NbPlanet; i++)
        {
            GameObject go = Instantiate(m_PlanetPrefab, transform);
            go.AddComponent<BigBangPlanet>();
            go.GetComponent<Renderer>().sharedMaterial = m_materials[UnityEngine.Random.Range(0, m_materials.Length)];

            if(i%9 == 0)
            {
                go = Instantiate(m_SunPrefab[UnityEngine.Random.Range(0, m_SunPrefab.Length)], transform);
                go.AddComponent<BigBangPlanet>();
            }
        }

        m_particles.Play();

        BigBangPlanet[] bbps = GetComponentsInChildren<BigBangPlanet>();
        foreach (BigBangPlanet bbp in bbps)
            bbp.Go(m_expansionTime);
    }

    private void EndExpansion()
    {
        BigBangPlanet[] bbps = GetComponentsInChildren<BigBangPlanet>();
        foreach (BigBangPlanet bbp in bbps)
            bbp.Stop();

        OnExpansionEnded?.Invoke();
    }
}
