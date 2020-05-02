using UnityEngine;

public class BigBang : MonoBehaviour
{
    [SerializeField]
    private GameObject              m_PlanetPrefab = null;

    [SerializeField]
    private int                     m_NbPlanet = 0;

    private bool                    m_ok = false;

    [SerializeField]
    private BlinkTextAnimator       m_textAnim = null;

    // Start is called before the first frame update
    void Start()
    {
        for (int i = 0; i < m_NbPlanet; i++)
            Instantiate(m_PlanetPrefab, transform);

        m_textAnim.OnAnimationEnded += () => m_ok = true;
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.A) && m_ok)
        {
            m_textAnim.gameObject.SetActive(false);

            BigBangPlanet[] bbps = GetComponentsInChildren<BigBangPlanet>();
            foreach (BigBangPlanet bbp in bbps)
                bbp.Go();
        }
    }
}
