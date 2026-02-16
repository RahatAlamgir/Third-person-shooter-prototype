using UnityEngine;
using System.Collections;

public class ObjectTakeDamage : MonoBehaviour, IDamageAble
{
    [SerializeField] private GameObject RedMesh;
    [SerializeField] private float delay = 0.5f;

    public void TakeDamage(float amount)
    {
        // 1. Health reduction logic goes here

        // 2. Start the flash effect
        if (RedMesh != null)
        {
            // Stop the previous flash if we get hit again quickly
            StopAllCoroutines();
            StartCoroutine(FlashRedEffect());
        }
    }

    IEnumerator FlashRedEffect()
    {
        // Show the red mesh
        RedMesh.SetActive(true);

        // Wait for the delay (e.g., 0.5 seconds)
        yield return new WaitForSeconds(delay);

        // Hide the red mesh
        RedMesh.SetActive(false);

        // The Coroutine finishes here and stops using CPU!
    }
    public int ObjectType() => 4;
    public bool IsDead() => false;
}
