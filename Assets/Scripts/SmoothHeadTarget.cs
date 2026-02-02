using UnityEngine;

public class SmoothHeadTarget : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    [SerializeField] private Transform realTarget; // The actual thing to look at
    [SerializeField] private float followSpeed = 5f;

    void Update()
    {
        if (realTarget == null) return;

        // Smoothly move this object toward the real target
        transform.position = Vector3.Lerp(transform.position, realTarget.position, Time.deltaTime * followSpeed);
    }
}
