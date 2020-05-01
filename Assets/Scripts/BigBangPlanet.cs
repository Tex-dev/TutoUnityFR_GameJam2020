using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BigBangPlanet : MonoBehaviour
{
    private Vector3     m_Dir;
    private bool        m_go = false;

    // Start is called before the first frame update
    void Start()
    {
        float size = Random.Range(0.5f, 2.5f);
        transform.localScale = new Vector3(size, size, size);

        m_Dir = new Vector3(Random.Range(-1.0f, 1.0f), Random.Range(-1.0f, 1.0f), Random.Range(-1.0f, 1.0f));
        m_Dir.Normalize();

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
    }
}
