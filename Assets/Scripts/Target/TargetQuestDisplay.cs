using TMPro;
using UnityEngine;

public class TargetQuestDisplay : MonoBehaviour
{
    private int score=0;
    [SerializeField] private TextMeshProUGUI scoreDisplay;
    [SerializeField] private TextMeshProUGUI timerDisplay;


    [Header("Timer Settings")]
    [SerializeField] private float timeRemaining = 180f; // 3 minutes in seconds
    private bool isTimeUp = false;

    void Update()
    {
        if (!isTimeUp)
        {
            if (timeRemaining > 0)
            {
                timeRemaining -= Time.deltaTime;
                UpdateTimerDisplay();
            }
            else
            {
                Debug.Log("Time is up! Scoring disabled.");
                timeRemaining = 0;
                isTimeUp = true;
                UpdateTimerDisplay();
            }
        }
    }

    public void AddScore(int amount)
    {
        // The "Gatekeeper" check
        if (isTimeUp) return;

        score += amount;
        scoreDisplay.text = score.ToString();
    }

    private void UpdateTimerDisplay()
    {
        // Convert seconds to Minutes:Seconds format
        float minutes = Mathf.FloorToInt(timeRemaining / 60);
        float seconds = Mathf.FloorToInt(timeRemaining % 60);

        timerDisplay.text = string.Format("{0:00}:{1:00}", minutes, seconds);
    }

    public void ReSetScore()
    {
        score = 0;
        timeRemaining = 180f; // Reset the clock too
        isTimeUp = false;
        scoreDisplay.text = score.ToString();
    }
}
