using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Renderer))]
public class BigBangPlanet : MonoBehaviour
{
    private Vector3     m_Dir;
    private bool        m_go = false;
    
    [SerializeField]
    Material[]          m_PlanetsMaterials;
    
    [SerializeField]
    Material[]          m_SunsMaterials;


    // Start is called before the first frame update
    void Start()
    {
        float size = Random.Range(0.5f, 2.5f);
        transform.localScale = new Vector3(size, size, size);

        /*
        if (Random.Range(0.0f, 1.0f) < 0.95f)
            GetComponent<Renderer>().material = m_PlanetsMaterials[Random.Range(0, m_PlanetsMaterials.Length)];
        else
            GetComponent<Renderer>().material = m_SunsMaterials[Random.Range(0, m_SunsMaterials.Length)];//*/

//        m_Dir = new Vector3(Random.Range(-1.0f, 1.0f), Random.Range(-1.0f, 1.0f), Random.Range(-1.0f, 1.0f));
//        m_Dir.Normalize();
        m_Dir = Random.onUnitSphere;

        if (Vector3.Dot(m_Dir, Vector3.back) > 0.0f)
            m_Dir *= -1.0f;

        m_Dir *= Random.Range(0.1f, 0.5f);
    }

    // Update is called once per frame
    void Update()
    {
        if(m_go)
        {
            transform.Translate(m_Dir);
        }
    }

    public void Go()
    {
        m_go = true;

        Invoke("NoGo", 4.0f);
    }

    private void NoGo()
    {
        m_go = false;
    }

}
