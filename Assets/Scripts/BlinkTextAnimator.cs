using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Text))]
public class BlinkTextAnimator : MonoBehaviour
{
    [SerializeField]
    private float       m_Time = 1.5f;

    private bool        m_Increase = true;
    private Text        m_Text = null;

    private bool        m_isInvoked = false;
    public Action       OnAnimationEnded;

    // Start is called before the first frame update
    void Start()
    {
        m_Text = GetComponent<Text>();

        if (m_Time <= 0.0f)
            m_Time = 1.5f;

        StartCoroutine(Animate());
    }

    private IEnumerator Animate()
    {
        while (true)
        {
            Color color = m_Text.color;

            if(m_Increase)
            {
                m_Text.color = new Color(color.r, color.g, color.b, Math.Min(1.0f, color.a + 0.01f));

                if (m_Text.color.a >= 1.0f)
                    m_Increase = false;
            }
            else
            {
                m_Text.color = new Color(color.r, color.g, color.b, Math.Max(0.2f, color.a - 0.01f));

                if (m_Text.color.a <= 0.2f)
                    m_Increase = true;
            }

            if (!m_isInvoked && color.a > 0.2f)
            {
                OnAnimationEnded?.Invoke();
                m_isInvoked = true;
            }


            yield return new WaitForSeconds(m_Time/100.0f);
        }
    }
}
