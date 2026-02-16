using UnityEngine;
using TMPro;
using DG.Tweening; // Let's use DOTween for the fade!

public class DamagePopup : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI damageText;
    [SerializeField] private float floatSpeed = 1.0f;
    [SerializeField] private float fadeDuration = 0.8f;

    private Transform camTransform;
    private Vector3 startScale;
    

    void Awake()
    {
        // Cache the camera once during Awake, not every frame
        camTransform = Camera.main.transform;
        startScale = transform.localScale;
    }

    // This runs EVERY time the object is taken out of the pool
    void OnEnable()
    {
        // 1. Reset visual state
        transform.DOKill();
        damageText.DOKill();

        transform.localScale = startScale;
        damageText.alpha = 1f;

        

        RotateToCamera();

        // 2. DOTween Juice: Fade out and then Deactivate
        damageText.DOFade(0, fadeDuration)
            .SetDelay(0.2f)
            .OnComplete(() => gameObject.SetActive(false));

        // 3. Optional: Little "Pop" effect when it appears
        
    }

    
    public void SetValue(float amount, float damageMultiplier = 1)
    {
        damageText.text = amount.ToString();
        
        if (damageMultiplier <= 1)
        {
            SetColorFontSize(Color.white, 36);
        } else if (damageMultiplier <= 2)
        {
            SetColorFontSize(Color.yellow, 40);
        }
        else
        {
            SetColorFontSize(Color.red, 46);
        }
    }
    private void SetColorFontSize(Color color ,float fontSize, bool shake = false)
    {
        damageText.color = color;
        damageText.fontSize = fontSize;
        transform.DOPunchScale(Vector3.one * 0.2f, 0.2f);
    }

    void LateUpdate()
    {

        RotateToCamera();
        // Use the cached camTransform - much faster than Camera.main
        // transform.LookAt(transform.position + camTransform.forward);

        // Float upwards
        transform.position += Vector3.up * floatSpeed * Time.deltaTime;
    }
    private void RotateToCamera()
    {
        if (camTransform == null) return;

        // Using transform.forward = camTransform.forward is often more stable 
        // for UI popups than LookAt, which can flip objects.
        transform.forward = camTransform.forward;
    }

    
}