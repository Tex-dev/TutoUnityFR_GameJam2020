using System;
using UnityEngine;

public class BigBangPlanet : MonoBehaviour
{
    private Vector3     m_Dir;
    private bool        m_go = false;

    private bool        m_rotate = false;
    private Vector3     m_axis;
    
    private float       m_size;
    private float       m_stepSizeBySecond;


    // Start is called before the first frame update
    void Start()
    {
        m_size = UnityEngine.Random.Range(0.5f, 2.5f);

        if(GetComponent<Planet>() != null)
            transform.localScale = new Vector3(0.0f, 0.0f, 0.0f);
        else
            transform.localScale = new Vector3(m_size, m_size, m_size);

            m_Dir = UnityEngine.Random.onUnitSphere;

        if (Vector3.Dot(m_Dir, Vector3.back) > 0.0f)
            m_Dir *= -1.0f;

        m_Dir *= UnityEngine.Random.Range(0.1f, 0.5f);
    }

    // Update is called once per frame
    void Update()
    {
        if(m_go)
        {
            transform.Translate(m_Dir);

            if (GetComponent<Planet>() != null)
            {
                Vector3 scale = transform.localScale;
                transform.localScale = scale + new Vector3(m_stepSizeBySecond * Time.deltaTime, m_stepSizeBySecond * Time.deltaTime, m_stepSizeBySecond * Time.deltaTime);
            }
        }

        if(m_rotate)
        {
            transform.Rotate(m_axis, 7.0f * Time.deltaTime, Space.World);
        }
    }

    public void Go(float expansionTime)
    {
        m_go = true;

        m_stepSizeBySecond = m_size / expansionTime;
    }

    public void Stop()
    {
        m_go = false;
    }

    public void Rotate(Vector3 axis)
    {
        m_rotate = true;
        m_axis = axis;
    }

}
