using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CellInfo : MonoBehaviour
{
    public enum Content
    {
        empty,
        vegetal,
        vegetarian,
        carnivorus
    }

    public enum Status
    {
        alive,
        dead
    }

    public int X = 0;

    public int Y = 0;

    public Status m_Status = Status.dead;

    public Status m_NextStatus = Status.dead;

    private Button m_Button = null;

    private Image m_Image = null;

    private void Awake()
    {
        m_Button = GetComponent<Button>();

        m_Image = GetComponent<Image>();
    }

    public void InitCell(Action<CellInfo> OnClick)
    {
        m_Button.onClick.AddListener(() => OnClick(this));
    }

    public void UpdateStatus()
    {
        m_Status = m_NextStatus;
        switch (m_Status)
        {
            case Status.alive:
                m_Image.color = Color.white;
                break;

            case Status.dead:
                m_Image.color = Color.black;
                break;

            default:
                break;
        }
    }

    public void UpdateStatus(Status status)
    {
        m_Status = status;
        switch (status)
        {
            case Status.alive:
                m_Image.color = Color.white;
                break;

            case Status.dead:
                m_Image.color = Color.black;
                break;

            default:
                break;
        }
    }
}