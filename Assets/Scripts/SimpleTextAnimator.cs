﻿using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Animate a text UI.
/// </summary>
public class SimpleTextAnimator : MonoBehaviour
{
    /// <summary>
    /// Speed at which a character is typed.
    /// </summary>
    [SerializeField]
    private float m_CharacterSpeed = 0.25f;

    /// <summary>
    /// Should use typing SFX?
    /// </summary>
    [SerializeField]
    private bool m_TypingSFXEnabled = false;

    /// <summary>
    /// Audio clip for typing SFX.
    /// </summary>
    [SerializeField]
    private AudioClip m_TypingSFX = null;

    /// <summary>
    /// Audio clip for spacebar typing SFX.
    /// </summary>
    [SerializeField]
    private AudioClip m_TypingSpacebarSFX = null;

    /// <summary>
    /// Audio clip for carriage return typing SFX.
    /// </summary>
    [SerializeField]
    private AudioClip m_CarriageReturnSFX = null;

    /// <summary>
    /// Audio source to use for the typing SFXs.
    /// </summary>
    [SerializeField]
    private AudioSource m_TypingAudioSource = null;

    /// <summary>
    /// Should use a narrator when typing text?
    /// </summary>
    [SerializeField]
    private bool m_AudioNarratoEnabled = false;

    /// <summary>
    /// Narrator voice clip.
    /// </summary>
    [SerializeField]
    private AudioClip m_NarratorClip = null;

    /// <summary>
    /// Audio source to use for the narrator voice.
    /// </summary>
    [SerializeField]
    private AudioSource m_NarratorAudioSource = null;

    /// <summary>
    /// Next text
    /// </summary>
    [SerializeField]
    private GameObject m_nextText = null;

    /// <summary>
    /// Text UI element.
    /// </summary>
    private Text m_Text = null;

    /// <summary>
    /// Text content to display.
    /// </summary>
    private string m_Content = "";

    /// <summary>
    /// Time before the text disappears for the next text.
    /// </summary>
    [SerializeField]
    private float m_TimeBeforeDisable = 0.0f;

    public Action OnAnimationEnded;

    /// <summary>
    /// Start is called by Unity after initialization.
    /// </summary>
    private void Start()
    {
        m_Text = GetComponent<Text>();

        m_TypingAudioSource = GetComponent<AudioSource>();

        m_Content = m_Text.text;
        m_Text.text = "";

        if(m_nextText != null)
           m_nextText.SetActive(false);

        StartCoroutine(Animate());
    }

    /// <summary>
    /// Animate the text.
    /// </summary>
    private IEnumerator Animate()
    {
        int index = -1;

        float narratorSpeed = 0f;

        if (m_AudioNarratoEnabled)
        {
            narratorSpeed = m_NarratorClip.length / m_Content.Length;

            m_NarratorAudioSource.PlayOneShot(m_NarratorClip);
        }
        else
        {
            narratorSpeed = m_CharacterSpeed;
        }

        while (m_Text.text.Length < m_Content.Length)
        {
            m_Text.text += m_Content[++index];

            if (m_Content[index] == ' ')
            {
                if (m_TypingSFXEnabled)
                    m_TypingAudioSource.PlayOneShot(m_TypingSpacebarSFX);
            }
            else
            {
                if (m_TypingSFXEnabled)
                    m_TypingAudioSource.PlayOneShot(m_TypingSFX);
            }

            yield return new WaitForSeconds(narratorSpeed);
        }

        m_TypingAudioSource.PlayOneShot(m_CarriageReturnSFX);

        while(m_Text.color.a >0.0f)
        {
            Color color = m_Text.color;
            m_Text.color = new Color(color.r, color.g, color.b, color.a - 0.01f);

            yield return new WaitForSeconds(m_TimeBeforeDisable / 100.0f);
        }

        OnAnimationEnded?.Invoke();
        if (m_nextText != null)
            m_nextText.SetActive(true);
        gameObject.SetActive(false);
    }
}