﻿using System.Collections;
using UnityEngine;
using TMPro;

public class FlashTextEffect : MonoBehaviour
{
    // Config params
    [SerializeField] private float timeToFlick = 0.01f;
    [SerializeField] private bool isLooping = true;

    // Cached
    private Color color;
    private TextMeshPro textMeshPro;
    private TextMeshProUGUI textMeshProUGUI;

    public void SetTimeToFlick(float timeToFlick)
    {
        this.timeToFlick = timeToFlick;
    }

    private void Start()
    {
        DefineComponentType();
        StartCoroutine(Flash());
    }

    private void DefineComponentType()
    {
        // Parent or children
        textMeshPro = this.GetComponent<TextMeshPro>();
        if (!textMeshPro)
        {
            textMeshPro = this.GetComponentInChildren<TextMeshPro>();
        }

        // For UGUI
        if (!textMeshPro)
        {
            textMeshProUGUI = this.GetComponent<TextMeshProUGUI>();
        }
    }

    // Flashes the alpha of text color
    private IEnumerator Flash()
    {
        while (isLooping)
        {
            // Cancels
            if (textMeshPro)
            {
                string text = textMeshPro.text;
                if (string.IsNullOrEmpty(text) || string.IsNullOrWhiteSpace(text))
                {
                    yield return null;
                }
            }
            else if (textMeshProUGUI)
            {
                string text = textMeshProUGUI.text;
                if (string.IsNullOrEmpty(text) || string.IsNullOrWhiteSpace(text))
                {
                    yield return null;
                }
            }

            // Color
            Color color = (textMeshPro ? textMeshPro.color : textMeshProUGUI.color);
            color.a = (color.a == 1f ? 0f : 1f);

            if (textMeshPro)
            {
                textMeshPro.color = color;
            }
            else if (textMeshProUGUI)
            {
                textMeshProUGUI.color = color;
            }

            yield return new WaitForSeconds(timeToFlick);
            yield return null;
        }
    }
}