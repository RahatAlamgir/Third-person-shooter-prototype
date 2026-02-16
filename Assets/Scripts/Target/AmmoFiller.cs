using DG.Tweening;

using UnityEngine;

public class AmmoFiller : MonoBehaviour , Iinteractable
{

    [SerializeField] private bool canInteract = true;
    [SerializeField] private bool fullAmmoRefill = true;
    [SerializeField] private bool Infinity = true;
    [SerializeField] private PlayerInventoryData inventoryData;

    private ChestAnimator animator;
    

    [Header("Arrow")]
    [SerializeField] private GameObject arrow;
    [SerializeField] private bool marked = true;

    private GameObject activeArrow;

    private void Awake()
    {
        animator = GetComponent<ChestAnimator>();
    }

    private void Start()
    {
        SpawnMarker();
    }

    public void SpawnMarker()
    {
        if (arrow == null || !marked) return;

        // 1. Spawn the arrow slightly above the current object
        Vector3 spawnPos = transform.position + Vector3.up * 1.0f; // Adjust 2.0f as needed
                                                                   // Start with X at 180 degrees
        Quaternion upsideDownRotation = Quaternion.Euler(180, 0, 0);

        activeArrow = Instantiate(arrow, spawnPos, upsideDownRotation);
        activeArrow.transform.SetParent(this.transform);


    }

    public bool CanInteract() => canInteract;
    

    public void Interact(Interactor interactor)
    {
        
        if (inventoryData != null)
        {
            if (fullAmmoRefill)
            {
                inventoryData.ReFillAll();
                
            }
        }
        if (animator != null)
        {
            if(Infinity) animator.ChestOpenClose();
            else animator.ChestOpen();
        }

        

        if (marked)
        {
            Destroy(activeArrow);
            marked = false;
        }
        
        canInteract = Infinity;
        
    }

    public void SetMarked(bool marked)
    {
        this.marked = marked;
    }
    
}
