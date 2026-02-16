using DG.Tweening;
using UnityEngine;

public class MoveRotate : MonoBehaviour
{
    [Header("Moving")]
    [SerializeField] private bool isMoving = false;
    [SerializeField] private float moveDistance = 5f;
    [SerializeField] private float moveSpeed = 1f;

    [SerializeField] private Vector3 moveAxis = Vector3.right;
    [SerializeField] private Ease moveMotion = Ease.Linear;
    [SerializeField] private LoopType MoveLoopType = LoopType.Yoyo;
    [SerializeField] private int numberOfLoop = -1;


    [Header("Rotating")]
    [SerializeField] private bool isRotating = false;
    [SerializeField] private float rotationCompleteDuration = 1f;
    
    [SerializeField] private Vector3 rotateAxis = Vector3.up;

    [SerializeField][Range(-360,360)] private float rotationAngle = 360;
    [SerializeField] private Ease rotationMotion = Ease.Linear;
    [SerializeField] private LoopType rotationLoopType = LoopType.Incremental;
    [SerializeField] private RotateMode rotationMode = RotateMode.FastBeyond360;

    [SerializeField] private int numberOfRotation = -1;
    

    private void OnEnable()
    {
        transform.DOKill();

        if (isMoving)
        {
            MoveObject();
        }
        if (isRotating)
        {
            RotateObject();
        }
    }

    private void MoveObject()
    {
        // Avoid division by zero
        float duration = moveSpeed <= 0 ? moveDistance : moveDistance / moveSpeed;

        transform.DOMove(moveAxis * moveDistance, duration)
            .SetRelative(true) // Safe: Moves relative to current position
            .SetLoops(numberOfLoop, MoveLoopType)
            .SetEase(moveMotion);
    }

    private void RotateObject()
    {
        transform.DORotate(rotateAxis * rotationAngle, rotationCompleteDuration, rotationMode)
            .SetRelative(true) // Safe: Rotates relative to current rotation
            .SetEase(rotationMotion)
            .SetLoops(numberOfRotation, rotationLoopType);
    }

    private void OnDisable()
    {
        transform.DOKill();
    }
    private void OnDestroy()
    {
        transform.DOKill();
    }
}
