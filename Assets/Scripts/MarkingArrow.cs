using DG.Tweening;
using UnityEngine;

public class MarkingArrow : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private float rotationDuration = 3f; // Degrees per second
    [SerializeField] private float hoverDistance = 0.5f; // How high it floats
    [SerializeField] private float hoverDuration = 2f; // Time for one full bob
    private void Start()
    {
        transform.DOLocalRotate(new Vector3(0, 360, 0), rotationDuration, RotateMode.FastBeyond360)
         .SetRelative(true) // Makes it easier to handle starting rotations
         .SetEase(Ease.Linear)
         .SetLoops(-1, LoopType.Incremental);

        
        transform.DOMoveY(transform.position.y + hoverDistance, hoverDuration)
            .SetEase(Ease.InOutSine) 
            .SetLoops(-1, LoopType.Yoyo);
    }

    
    private void OnDestroy()
    {
        transform.DOKill();
    }

    public void ObjectDestory()
    {
        Destroy(gameObject);
    }
}
