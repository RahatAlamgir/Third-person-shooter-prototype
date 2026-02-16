using DG.Tweening;
using StarterAssets;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.InputSystem;



public class Car : MonoBehaviour, Iinteractable
{
    
    private GameObject playerModel;
    
    private CarController carController;    // The physics script
    private PlayerInput playerInput;        // The Input component
    private Health health;
    private Rigidbody rb;
    private NavMeshObstacle navMeshObstacle;

    private bool _isBeingDriven = false;



    

    private void Awake()
    {
        playerModel = GameObject.FindGameObjectWithTag("Player");
        carController = GetComponent<CarController>();
        health = GetComponent<Health>();
        rb = GetComponent<Rigidbody>();
        navMeshObstacle = GetComponent<NavMeshObstacle>();
    }
    public bool CanInteract()
    {
        if (_isBeingDriven || health.IsDead()) return false;
        return true;
    }

    public void Interact(Interactor interactor)
    {
        //Debug.Log(interactor.gameObject);
        SetPlayerModel(interactor.gameObject);
        if (!_isBeingDriven)
        {
            EnterCar();
        }
    }

    private void SetPlayerModel(GameObject playerModel)
    {
        this.playerModel = playerModel;
        playerInput = playerModel.GetComponentInChildren<PlayerInput>();
    }

    private void EnterCar()
    {
        
        playerInput.SwitchCurrentActionMap("Car");
        CarComponent(true);
        carController.SetDriving(true);

        PlayerComponent(false); //Disable player components

        // 3. Parent the Player to the Car

        playerModel.transform.SetParent(this.transform);
        playerModel.transform.position = transform.position;
        playerModel.transform.DOLocalRotate(Vector3.zero, 1.35f);

        
        transform.DOPunchScale(new Vector3(0.05f, 0.05f, 0.05f), 0.2f);
    }

    public void ExitCar()
    {
        
        playerInput.SwitchCurrentActionMap("Player");
        carController.SetDriving(false);
        CarComponent(false);

        // 1. Unparent the Player
        playerModel.transform.SetParent(null);

        // 2. Reposition & Re-enable Physics
        playerModel.transform.position = transform.position + (transform.right * 2f);

        PlayerComponent(true); //enable player components

        
    }

    private void CarComponent(bool value)
    {
        _isBeingDriven = value;
        carController.enabled = value;
        navMeshObstacle.carveOnlyStationary = !value;
        rb.isKinematic = !value;
    }

    private void PlayerComponent(bool value)
    {
        // 1. Hide Visuals
        var renderers = playerModel.GetComponentsInChildren<Renderer>();
        foreach (var r in renderers) r.enabled = value;

        
        var charController = playerInput.GetComponentInChildren<CharacterController>();
        if (charController != null) charController.enabled = value;

        var thirdPersonController = playerModel.GetComponentInChildren<ThirdPersonController>();
        if (thirdPersonController != null) thirdPersonController.enabled = value;

        var thirdPersonShooterController = playerModel.GetComponentInChildren<ThirdPersonShooterController>();
        if (thirdPersonShooterController != null)
        {
            thirdPersonShooterController.SetCrosshairVisible(value);
            thirdPersonShooterController.enabled = value;
        }
            

        var interactor = playerModel.GetComponentInChildren<Interactor>();
        if (interactor != null) interactor.enabled = value;
    }



}
