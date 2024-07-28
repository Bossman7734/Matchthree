using System;
using UnityEngine;
using TMPro;

public class TextReplacer : MonoBehaviour
{
    // Reference to the TextMeshPro component that will have its text replaced
    public TextMeshProUGUI targetTMP;

    // Reference to the TextMeshPro component from which text will be taken
    public TextMeshProUGUI sourceTMP;

    private void Awake()
    {
        ReplaceText();
    }

    // Method to replace text
    public void ReplaceText()
    {
        if (targetTMP != null && sourceTMP != null)
        {
            targetTMP.text = sourceTMP.text;
        }
        else
        {
            Debug.LogWarning("TextMeshPro references are not assigned.");
        }
    }
}