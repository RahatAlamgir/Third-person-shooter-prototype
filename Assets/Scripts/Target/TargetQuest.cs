using UnityEngine;

public class TargetQuest : MonoBehaviour, Iinteractable
{

    [SerializeField] private bool hasQuest = true;
    [SerializeField] private TargetQuestDisplay targetQuestDisplay;
    [Header("Arrow")]
    [SerializeField] private GameObject arrow;
    [SerializeField] private bool marked = true;

    private GameObject activeArrow;

    private void Start()
    {
        SpawnMarker();
    }

    public void SpawnMarker()
    {
        if (arrow == null || !marked) return;

        // 1. Spawn the arrow slightly above the current object
        Vector3 spawnPos = transform.position + Vector3.up * 2.5f; // Adjust 2.0f as needed
                                                                   // Start with X at 180 degrees
        Quaternion upsideDownRotation = Quaternion.Euler(180, 0, 0);

        activeArrow = Instantiate(arrow, spawnPos, upsideDownRotation);
        activeArrow.transform.SetParent(this.transform);


    }
    public bool CanInteract() => true;

    public void Interact(Interactor interactor)
    {
        if (targetQuestDisplay != null)
            targetQuestDisplay.ReSetScore();

        if (marked)
        {
            Destroy(activeArrow);
            marked = false;
        }
    }

    
    
}
