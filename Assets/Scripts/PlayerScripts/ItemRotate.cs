using DG.Tweening;
using UnityEngine;

public class ItemRotate : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created

    [SerializeField] private bool isRotate = true;
    [SerializeField] private bool isEquipped = false;
    
    [SerializeField] private BoxCollider boxCollider;

    [Header("Settings")]
    [SerializeField] private float rotationDuration = 3f; // Degrees per second
    [SerializeField] private float hoverDistance = 0.5f; // How high it floats
    [SerializeField] private float hoverDuration = 2f; // Time for one full bob

    //[SerializeField] private Rigidbody rb;
    //private bool hasLanded = false;

    private void Awake()
    {
        boxCollider = GetComponent<BoxCollider>();
        //rb = GetComponent<Rigidbody>();
    }
    void Start()
    {
        
        if (isEquipped)
        {
            SetIsRotate(false);
            boxCollider.enabled = false;
            // Later, when you pick it up:
            
        }
        if (isRotate)
        {

            StartRotationAnimation();
        }
    }

    private void StartRotationAnimation()
    {
        // 1. Kill any existing tweens on this object to prevent overlaps
        //OnDisable();
        boxCollider.enabled = true;
        // 2. Create the Infinite Rotation
        // 1. Rotate 360 degrees on Y axis
        // 2. Loop infinitely (-1)
        // 3. Move incrementally (keep spinning)
        // 4. SetEase(Ease.Linear) makes it a constant speed (no slow down/speed up)
        transform.DORotate(new Vector3(0, 360, 0), rotationDuration, RotateMode.FastBeyond360)
            //.SetSpeedBased()
            .SetEase(Ease.Linear)
            .SetLoops(-1, LoopType.Incremental);

        // 3. Create the Hover (Bobbing) Effect
        // Yoyo moves it up, then back down.
        transform.DOMoveY(transform.position.y + hoverDistance, hoverDuration)
            .SetEase(Ease.InOutSine) // InOutSine makes the top/bottom turns feel soft
            .SetLoops(-1, LoopType.Yoyo);
    }

    private void OnDisable()
    {
        // Clean up when the object is hidden or destroyed
        transform.DOKill();
    }

    

    public void SetIsRotate(bool isRotate)
    {
        this.isRotate = isRotate;
        if (!isRotate) OnDisable();
        else StartRotationAnimation();
    }
    
    public bool GetIsEqupe()
    {
        return this.isEquipped;
    }
    public void SetIsEquipped(bool isEquipped)
    {
        this.isEquipped = isEquipped;
    }
    
}
