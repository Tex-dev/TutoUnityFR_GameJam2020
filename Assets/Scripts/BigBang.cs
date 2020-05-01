using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BigBang : MonoBehaviour
{
    [SerializeField]
    private GameObject     m_PlanetPrefab;

    [SerializeField]
    private int            m_NbPlanet;

    // Start is called before the first frame update
    void Start()
    {
        for (int i = 0; i < m_NbPlanet; i++)
            Instantiate(m_PlanetPrefab, transform);
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.A))
        {
            Debug.Log("coucou");

            BigBangPlanet[] bbps = GetComponentsInChildren<BigBangPlanet>();

            foreach (BigBangPlanet bbp in bbps)
                bbp.Go();
        }
    }
}
