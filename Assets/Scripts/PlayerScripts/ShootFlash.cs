using UnityEngine;
using DG.Tweening; // Using DOTween for efficiency

public class ShootFlash : MonoBehaviour
{
    [SerializeField] private float duration = 0.05f;
    [SerializeField] private float maxIntensity = 20f;
    private Light flashLight;

    void Awake()
    {
        flashLight = GetComponent<Light>();
    }

    // This runs every time the Pooler pulls this light out of the "Warehouse"
    void OnEnable()
    {
        // 1. Reset intensity immediately
        flashLight.intensity = maxIntensity;

        // 2. Use DOTween to fade to 0 over the duration
        // This is smoother and faster than Update()
        flashLight.DOIntensity(0, duration)
            .SetEase(Ease.InQuad)
            .OnComplete(() => {
                // 3. Return to pool by deactivating
                gameObject.SetActive(false);
            });
    }

    // We don't need Update() anymore! 
    // This saves CPU cycles because the script isn't "thinking" every frame.
}
