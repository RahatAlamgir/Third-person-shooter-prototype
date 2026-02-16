using TMPro;
using UnityEngine;

public class CarUIManager : MonoBehaviour
{

    [SerializeField] private GameObject speedMeter;
    private TextMeshProUGUI speedText;
    [SerializeField] private GameObject hpBar;
    private TextMeshProUGUI hpText;
    private void Awake()
    {
        if (speedMeter != null)
        {
            speedText = speedMeter.GetComponentInChildren<TextMeshProUGUI>();
        }
        if (hpBar != null)
        {
            hpText = hpBar.GetComponentInChildren<TextMeshProUGUI>();
        }
    }
    private void OnEnable()
    {
        speedMeter.SetActive(true);
        hpBar.SetActive(true);
    }
    private void OnDisable()
    {
        speedMeter.SetActive(false);
        hpBar.SetActive(false);
    }
    // Update is called once per frame
    public void UpdateUI(float currentSpeed, float currentHp)
    {
        if (speedText != null)
        {
            float displaySpeed = Mathf.Abs(currentSpeed) * 3.6f;
            speedText.text = Mathf.RoundToInt(displaySpeed).ToString() + " KM/H";
        }
        if (hpText != null)
        {
            hpText.text = Mathf.RoundToInt(currentHp).ToString();
        }
    }
}
