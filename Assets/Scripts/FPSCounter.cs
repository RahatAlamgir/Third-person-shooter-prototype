using UnityEngine;
using TMPro;

public class FPSCounter : MonoBehaviour
{
    private TextMeshProUGUI fpsText;
    private float deltaTime = 0.0f;

    void Awake()
    {
        fpsText = GetComponent<TextMeshProUGUI>();
    }

    void Update()
    {
        // Calculate the smooth delta time
        deltaTime += (Time.unscaledDeltaTime - deltaTime) * 0.1f;

        float msec = deltaTime * 1000.0f;
        float fps = 1.0f / deltaTime;

        // string.Format is better than adding strings with "+"
        fpsText.text = string.Format("{0:0.0} ms ({1:0.} fps)", msec, fps);

        // Color coding for quick feedback
        if (fps < 30) fpsText.color = Color.red;
        else if (fps < 60) fpsText.color = Color.yellow;
        else fpsText.color = Color.green;
    }
}