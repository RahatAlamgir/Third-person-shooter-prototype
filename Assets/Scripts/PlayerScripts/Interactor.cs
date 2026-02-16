using StarterAssets;
using UnityEngine;

public class Interactor : MonoBehaviour
{
    [SerializeField] private float _castDistance = 5f;
    [SerializeField] private GameObject interactUI; //(E) Text/Image h

    [SerializeField] private LayerMask interactableLayer;

    private bool _isShowingUI;

    private StarterAssetsInputs input;
    private Camera mainCam;
    private float startOffset = 3.8f;

    private void OnDisable()
    {
        ToggleUI(false);
    }


    private void Start()
    {
        input = GetComponent<StarterAssetsInputs>();
        mainCam = Camera.main;
        _castDistance -= startOffset;
    }
    void Update()
    {
        // 1. ALWAYS check if looking at something to show/hide UI
        if (DoInteraction(out Iinteractable lookingAt))
        {
            ToggleUI(lookingAt.CanInteract());
        }
        else
        {
            
            ToggleUI(false);
        }

        // 2. Handle the click
        if (input.interact)
        {
            if (lookingAt != null && lookingAt.CanInteract())
            {
                lookingAt.Interact(this);
            }
            input.interact = false;
        }
    }
    private void ToggleUI(bool show)
    {
        if (_isShowingUI == show) return; // Skip if no change

        _isShowingUI = show;
        interactUI.SetActive(show);
    }

    private bool DoInteraction(out Iinteractable interactable)
    {
        
        interactable = null;

        // Create a ray from the center of the screen
        Ray ray = mainCam.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0)) ;
        ray.origin += ray.direction * startOffset;
        

        // Debug line (only visible in Scene view)
        Debug.DrawRay(ray.origin, ray.direction * _castDistance, Color.green);

        if (Physics.Raycast(ray, out RaycastHit hitInfo, _castDistance, interactableLayer))
        {
            interactable = hitInfo.collider.GetComponentInParent<Iinteractable>();
            return interactable != null;
        }
        return false;
    }
}
