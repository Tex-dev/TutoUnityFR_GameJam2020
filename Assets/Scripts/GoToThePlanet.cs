using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GoToThePlanet : MonoBehaviour
{
    [SerializeField]
    private BigBang     m_bigbang = null;

    private bool        m_moveToPlanet = false;
    private Transform   m_planetToMove = null;

    private GameObject  m_targetPlanet = null;

    private Vector3     m_targetPosition;
    private Quaternion  m_targetRotation;
    private float       m_targetDistance;

    [SerializeField]
    private GameObject  m_text;

    [SerializeField]
    private float       m_speed = 0.0f;

    private bool        m_testNearAstres = false;

    // Start is called before the first frame update
    void Start()
    {
        m_bigbang.OnExpansionEnded += () => BeginMovement();
    }

    // Update is called once per frame
    void Update()
    {
        if (m_moveToPlanet && m_targetPlanet != null)
        {
            if(!m_testNearAstres)
            {
                float distance = Vector3.Distance(m_targetPlanet.transform.position, m_planetToMove.position);

                Transform parent = m_planetToMove.parent;
                int nbChildren = parent.childCount;

                for (int i = 0; i < nbChildren; i++)
                {
                    Transform child = parent.GetChild(i);

                    if (child == m_planetToMove)
                        continue;

                    if (Vector3.Distance(m_planetToMove.position, child.position) < distance)
                        child.gameObject.SetActive(false);
                }

                m_testNearAstres = true;
            }

            if ( Vector3.Distance(transform.position, m_targetPlanet.transform.position) > 0.1f)
            {
                transform.position = Vector3.Lerp(transform.position, m_targetPlanet.transform.position, m_speed * Time.deltaTime);
                transform.rotation = Quaternion.Slerp(transform.rotation, m_targetPlanet.transform.rotation, m_speed * Time.deltaTime);
            }
            else
            {
                if (!m_text.activeSelf)
                    m_text.SetActive(true);
            }
        }
    }

    private void BeginMovement()
    {
//        BigBangPlanet[] planets = m_bigbang.GetComponentsInChildren<BigBangPlanet>();
//        int planetID = Random.Range(0, planets.Length - 1);
//        m_planetToMove = planets[planetID].transform;

        m_planetToMove = m_bigbang.GetComponentInChildren<Planet>().transform;

        m_targetPlanet = new GameObject();

        m_targetPlanet.transform.position = m_planetToMove.position;
        m_targetPlanet.transform.rotation = Quaternion.LookRotation(m_planetToMove.position, transform.position);

        m_targetPlanet.transform.Translate(0.0f, 0.0f, -m_planetToMove.localScale.x * 1.9f, Space.Self);
        m_targetPlanet.transform.Translate(-m_planetToMove.localScale.x * 1.9f / 2.0f, 0.0f, 0.0f, Space.Self);

        Vector3 planetAxis = Quaternion.Euler(0.0f, 0.0f, 25.0f) * m_targetPlanet.transform.up;
        m_planetToMove.GetComponent<BigBangPlanet>().Rotate(planetAxis);


        /*
        m_targetPosition = m_planetToMove.position;
        m_targetRotation = Quaternion.LookRotation(m_planetToMove.position, transform.position);
        m_targetDistance = m_planetToMove.localScale.x * 1.9f;//*/

//       m_planetToMove.gameObject.name = "My exoplanet";

//       m_planetToMove.GetComponent<Planet>().m_res = 256;
//       m_planetToMove.GetComponent<Planet>().GeneratePlanet();

        /*
        GameObject go = Instantiate(new GameObject(), m_targetPosition, m_targetRotation, null);

        m_targetPosition += go.transform.right * -0.8f;

        Vector3 planetAxis = Quaternion.Euler(0.0f, 0.0f, 25.0f) * go.transform.up;
//        planets[planetID].Rotate(planetAxis);
        m_planetToMove.GetComponent<BigBangPlanet>().Rotate(planetAxis);//*/

        m_moveToPlanet = true;
    }
}
