using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class FPSDisplay : MonoBehaviour
{
    private TextMeshProUGUI fpsText; // or public Text fpsText; if not using TextMeshPro
    private float deltaTime = 0.0f;

    void Update()
    {
        fpsText = GetComponent<TextMeshProUGUI>(); 
        deltaTime += (Time.deltaTime - deltaTime) * 0.1f;
        float fps = 1.0f / deltaTime;
        fpsText.text = "FPS: " + Mathf.Ceil(fps).ToString();
    }
}
