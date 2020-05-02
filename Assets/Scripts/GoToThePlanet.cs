using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GoToThePlanet : MonoBehaviour
{
    [SerializeField]
    private BigBang     m_bigbang = null;

    private bool        m_moveToPlanet = false;
    private Transform   m_planetToMove = null;

    private Vector3     m_targetPosition;
    private Quaternion  m_targetRotation;
    private float       m_targetDistance;

    [SerializeField]
    private float       m_speed = 0.0f;

    // Start is called before the first frame update
    void Start()
    {
        m_bigbang.OnExpansionEnded += () => BeginMovement();
    }

    // Update is called once per frame
    void Update()
    {
        if (m_moveToPlanet && m_planetToMove != null)
        {
            float distance = Vector3.Distance(transform.position, m_planetToMove.position);
            if ( distance > m_targetDistance)
            {
                /*
                transform.position = Vector3.Lerp(transform.position, m_planetToMove.position, m_speed * Time.deltaTime);
                transform.rotation = Quaternion.Slerp(transform.rotation, m_planetToMove.rotation, m_speed * Time.deltaTime);//*/

                transform.position = Vector3.Lerp(transform.position, m_targetPosition, m_speed * Time.deltaTime);
                transform.rotation = Quaternion.Slerp(transform.rotation, m_targetRotation, m_speed * Time.deltaTime);
            }
        }
    }

    private void BeginMovement()
    {
//        BigBangPlanet[] planets = m_bigbang.GetComponentsInChildren<BigBangPlanet>();
//        int planetID = Random.Range(0, planets.Length - 1);
//        m_planetToMove = planets[planetID].transform;

        m_planetToMove = m_bigbang.GetComponentInChildren<Planet>().transform;

        m_targetPosition = m_planetToMove.position;
        m_targetRotation = Quaternion.LookRotation(m_planetToMove.position, transform.position);
        m_targetDistance = m_planetToMove.localScale.x * 1.9f;

 //       m_planetToMove.gameObject.name = "My exoplanet";

 //       m_planetToMove.GetComponent<Planet>().m_res = 256;
 //       m_planetToMove.GetComponent<Planet>().GeneratePlanet();

        GameObject go = Instantiate(new GameObject(), m_targetPosition, m_targetRotation, null);

        m_targetPosition += go.transform.right * -0.8f;

        Vector3 planetAxis = Quaternion.Euler(0.0f, 0.0f, 25.0f) * go.transform.up;
//        planets[planetID].Rotate(planetAxis);
        m_planetToMove.GetComponent<BigBangPlanet>().Rotate(planetAxis);

        m_moveToPlanet = true;
    }
}
