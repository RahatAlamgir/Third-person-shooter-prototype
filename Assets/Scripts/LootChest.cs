using UnityEngine;
using DG.Tweening;

public class LootChest : MonoBehaviour, Iinteractable
{
    [Header("Components")]
    [SerializeField] private Transform lid;
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip openSound;

    [Header("Settings")]
    [SerializeField] private float openAngle = -100f; // Usually negative to flip backwards
    [SerializeField] private float duration = 0.75f;
    [SerializeField] private bool isOpen = false;

    public void Interact(Interactor interactor)
    {
        if (isOpen) return; // Usually, chests stay open once looted

        isOpen = true;

        // 1. Sound
        if (audioSource != null && openSound != null)
        {
            audioSource.PlayOneShot(openSound);
        }

        // 2. Animation with "Back" Ease (The Bounce)
        // Ease.OutBack makes it go slightly past the target and spring back
        lid.DOLocalRotate(new Vector3(openAngle, 0, 0), duration)
           .SetEase(Ease.OutBack);

        // 3. Trigger the Loot
        GiveLoot();
    }

    private void GiveLoot()
    {
        transform.DOShakePosition(0.2f, 0.1f);
        Debug.Log("Chest opened! Spawning items...");
        // This is where you would call your inventory system
    }

    public bool CanInteract() => !isOpen;
}