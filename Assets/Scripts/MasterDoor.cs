using UnityEngine;
using DG.Tweening;

public class MasterDoor : MonoBehaviour, Iinteractable
{
    public enum DoorType { Rotate, Slide }

    [Header("Door Components")]
    [SerializeField] private Transform leftDoor;
    [SerializeField] private Transform rightDoor; // Optional: Leave null for single doors

    [Header("Animation Settings")]
    [SerializeField] private DoorType doorType = DoorType.Rotate;
    [SerializeField] private bool isOpen = false;
    [SerializeField] private float duration = 1.0f;
    [SerializeField] private Ease easeType = Ease.InOutQuad;

    [Header("Movement Logic")]
    [Tooltip("For Rotate: Degrees. For Slide: Meters.")]
    [SerializeField] private float amount = 90f;
    [Tooltip("Direction to move/rotate (e.g., 0,1,0 for Y-axis)")]
    [SerializeField] private Vector3 direction = Vector3.up;
    [SerializeField] private bool flipDoorB = true; // Opens doorB in opposite direction

    [Header("Sound")]
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip openSound;
    [SerializeField] private AudioClip closeSound;

    public void Interact(Interactor interactor)
    {
        isOpen = !isOpen;

        if (audioSource != null)
        {
            AudioClip clip = isOpen ? openSound : closeSound;
            if (clip != null)
            {
                audioSource.clip = clip;
                audioSource.pitch = Random.Range(0.95f, 1.05f);
                audioSource.Play(); // Start the sound
            }
        }

        // Check Door A: Only animate if assigned
        if (leftDoor != null) Animate(leftDoor, direction, false).OnComplete(StopDoorSound);

        // Check Door B: Only animate if assigned
        if (rightDoor != null) Animate(rightDoor, direction, flipDoorB);
        

        // Bonus: Professional Debug warning if you forgot BOTH doors
        if (leftDoor == null && rightDoor == null)
        {
            Debug.LogWarning($"Door {gameObject.name} has no door transforms assigned!");
        }
    }

    private void StopDoorSound()
    {
        if (audioSource != null && audioSource.isPlaying)
        {
            // Use DOFade if you want it to stop smoothly (no "pop" noise)
            audioSource.DOFade(0, 0.1f).OnComplete(() => {
                audioSource.Stop();
                audioSource.volume = 1f; // Reset volume for next time
            });
        }
    }

    private Tweener Animate(Transform target, Vector3 dir, bool invert)
    {
        target.DOKill();

        // Calculate the target state
        float multiplier = (isOpen ? amount : 0f) * (invert ? -1f : 1f);
        Vector3 targetValue = dir * multiplier;

        if (doorType == DoorType.Rotate)
        {
            return target.DOLocalRotate(targetValue, duration).SetEase(easeType);
        }
        else
        {
            // Use DOLocalMove because it works relative to the frame
            return target.DOLocalMove(-targetValue, duration).SetEase(easeType);
        }
    }

    public bool CanInteract() => true;
}

