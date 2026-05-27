using System;
using System.Collections;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
public class TextColor : MonoBehaviour
{
    public Color color;
    private Color currentColor;
    private Color initialColor;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

        initialColor = GetComponent<TextMeshProUGUI>().color;
        currentColor=initialColor;
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void ChangeColor()
    {

        if (currentColor != color)
        {
            currentColor = color;   
        }
        else
        {
            currentColor = initialColor;
        }
    
    }
}
