using UnityEngine;
using UnityEngine.UI;

using System.Collections; // Required for Coroutines

public class HealthBar : MonoBehaviour
{
    [SerializeField] private Slider slider;
    [SerializeField] private GameObject visualRoot; // Drag the "Canvas" or "Panel" here

    private Coroutine hideCoroutine;

    private void OnEnable() => BillboardManager.Billboards.Add(transform);
    private void OnDisable() => BillboardManager.Billboards.Remove(transform);

    private void Start()
    {
        // Hide it immediately when the game starts
        visualRoot.SetActive(false);
    }

    public void SetMaxHealth(float health)
    {
        slider.maxValue = health;
        slider.value = health;
    }

    public void SetHealth(float health)
    {
        // 1. Update the data
        slider.value = health;

        // 2. Ensure the visual container is active so we can see it
        if (!visualRoot.activeSelf)
        {
            visualRoot.SetActive(true);
        }

        // 3. Handle the timer
        if (hideCoroutine != null) StopCoroutine(hideCoroutine);
        hideCoroutine = StartCoroutine(HideAfterDelay(5f));
    }

    private IEnumerator HideAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        visualRoot.SetActive(false); // Only the visuals go to sleep
    }
}
