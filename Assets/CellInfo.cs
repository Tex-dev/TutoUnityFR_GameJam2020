using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CellInfo : MonoBehaviour
{
    public enum Content
    {
        dead = 0,
        vegetal = 1,
        vegetarian = 2,
        carnivorus = 3,
    }

    public enum Status
    {
        alive,
        dead
    }

    public int X = 0;

    public int Y = 0;

    public Content GetContent => m_Content;

    public Content GetNextContent => m_NextContent;

    private Content m_Content = Content.dead;

    private Content m_NextContent = Content.dead;

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

        m_Content = Content.dead;
        m_NextContent = Content.dead;
    }

    public void UpdateContent()
    {
        SetContent(m_NextContent);
    }

    public void SetNextContent(Content content)
    {
        m_NextContent = content;
    }

    public void SetContent(Content content)
    {
        m_Content = content;
        m_NextContent = content;

        switch (content)
        {
            case Content.vegetal:
                m_Image.color = Color.green;
                break;

            case Content.vegetarian:
                m_Image.color = Color.yellow;
                break;

            case Content.carnivorus:
                m_Image.color = Color.red;
                break;

            case Content.dead:
                m_Image.color = Color.black;
                break;

            default:
                break;
        }
    }
}