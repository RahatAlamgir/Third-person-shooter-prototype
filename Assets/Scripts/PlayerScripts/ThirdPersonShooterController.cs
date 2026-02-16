using DG.Tweening;
using StarterAssets;
using System.Collections.Generic;
using Unity.Cinemachine;

using UnityEngine;
using UnityEngine.Animations.Rigging;
using UnityEngine.UI;



public class ThirdPersonShooterController : MonoBehaviour 
{
    [Header("Aim")]
    [SerializeField] private CinemachineCamera aimVirtualCamera;
    [SerializeField] private float sensitivity = 1.0f;
    [SerializeField] private float aimSensitivity = 0.5f;
    [SerializeField] private LayerMask aimColliderLayerMask; // Simplified mask
    [SerializeField] private Transform debugTransform;
    [SerializeField] private float turnSpeed = 20f;
    [SerializeField] private Image crossHair;

    [Header("Rigs")]
    [SerializeField] private Rig aimRig;
    [SerializeField] private Rig holdGunRig;
    [SerializeField] private Rig reloadRig;
    [SerializeField] private float transitionSpeed = 10f;

    [Header("Character")]
    [SerializeField] private CharacterManager characterManager;
    


    [Header("Grenade")]
    [SerializeField] private GameObject grenade1;
    [SerializeField] private Transform grenadeSpawnPoint;

    [Header("Rifle PreFabs")]
    [SerializeField] private Rifle rifle;
    [SerializeField] private GameObject rightHandRifle;
    [SerializeField] private GameObject leftHandRifle;
    [SerializeField] private bool _isBulletInventoryEmpty=false;
    [SerializeField] private bool _isMagzineEmpty = false;

    // --- CACHED REFERENCES ---
    private StarterAssetsInputs input;
    private ThirdPersonController thirdPersonController;
    private CharacterController characterController;
    private Health health;
    private Camera mainCam;
    private Vector3 aimDirection;
    
    

    private float _fireDelayTimer = 0f;


    private float startOffset = 3.8f;


    private bool _sprint;
    private bool _aim;
    private bool _shoot;
    private bool _throw;
    private bool _reload;
    private bool _sliding;


    [Header("UI")]
    [SerializeField] private GameObject _UIManager;
    [SerializeField] private GameObject _UI;

    [Header("ScriptableObject")]
    [SerializeField] private PlayerInventoryData playerInventoryData;

    
    

    private void Awake()
    {

        if(_UIManager!=null) _UIManager.SetActive(true);
        if (_UI != null) _UI.SetActive(true);

        input = GetComponent<StarterAssetsInputs>();
        thirdPersonController = GetComponent<ThirdPersonController>();
        characterController = GetComponent<CharacterController>();
        health = GetComponent<Health>();
        

        // CACHE THE CAMERA: Calling Camera.main every frame is very expensive
        mainCam = Camera.main;

        
    }

    void Update()
    {
        InputBool();
        RigHandler();

        if (playerInventoryData != null )
        {
            UpDatePlayerData();
        }

        // 2. Optimized Character Swap

        CharacterCustomize();
        // 3. Optimized Aim Raycast
        HandleAimRaycast(out Vector3 mouseWorldPosition);



        // 4. Update Weapon 
        if (!_sprint && !_throw && !_reload && !_sliding)
        {
            AimShootStates(mouseWorldPosition);
        } else if (_sprint || _throw)
        {
            ResetCamara();
        }
        if(_throw || _reload) PlayerAutoTurn();


    }
    private void InputBool()
    {
        _aim = input.aim;
        _shoot = input.shoot;
        _throw = input.throwObject;
        _sprint = input.sprint;
        _reload = input.reload;
        _sliding = thirdPersonController.GetIsSlideing();
        _isBulletInventoryEmpty = playerInventoryData.IsBullelEmpty();
        _isMagzineEmpty = rifle.IsMagazineEmpty();
    }

    private void CharacterCustomize()
    {
        Vector2 arrowKey = input.arrow;
        input.arrow = Vector2.zero;

        // 1. Only run if Alt is held AND an arrow is pressed
        if (arrowKey != Vector2.zero)
        {
            // Horizontal (Left/Right) = Materials
            if (arrowKey.x != 0)
                characterManager.CycleMaterial((int)Mathf.Sign(arrowKey.x));


            // Vertical (Up/Down) = Characters
            if (arrowKey.y != 0)        
                characterManager.CycleCharacter((int)Mathf.Sign(arrowKey.y));

        }
        
    }
    private void HandleAimRaycast(out Vector3 mouseWorldPosition)
    {
        // Vector2 screenCenter = new Vector2(Screen.width / 2f, Screen.height / 2f);
        // Better: Use ViewportPointToRay to avoid Screen.width math every frame
        Ray ray = mainCam.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));
        ray.origin += ray.direction * startOffset;

        mouseWorldPosition = Vector3.zero;

        if (Physics.Raycast(ray, out RaycastHit raycastHit, 999f, aimColliderLayerMask))
        {
            IDamageAble damageAble = raycastHit.collider.GetComponentInParent<IDamageAble>();
            if (damageAble != null)
            {
                int objectType = damageAble.ObjectType();
                if (objectType == 1 || objectType == 3) crossHair.color = Color.green;
                else crossHair.color = Color.red;
                    
            } else
            {
                crossHair.color = Color.white;
            }

            mouseWorldPosition = raycastHit.point;
            if (debugTransform) debugTransform.position = raycastHit.point;
            Debug.DrawRay(ray.origin, ray.direction*50f, Color.red);
        }
        
        // Calculate aim direction once per frame
        Vector3 worldAimTarget = mouseWorldPosition;
        worldAimTarget.y = transform.position.y;
        aimDirection = (worldAimTarget - transform.position).normalized;
    }

    private void AimShootStates(Vector3 mouseWorldPosition)
    {
        if(_aim || _shoot) PlayerAutoTurn();

        if (_aim)
        {
            if (!aimVirtualCamera.gameObject.activeSelf) aimVirtualCamera.gameObject.SetActive(true);

            thirdPersonController.SetSensitivity(aimSensitivity);
            //PlayerAutoTurn();
            _fireDelayTimer = 0.3f;
        }
        else
        {
            ResetCamara();
        }
        if (_shoot && !_isMagzineEmpty )
        {
            // 1. Increment the timer
            _fireDelayTimer += Time.deltaTime;

            // 2. Only fire after the 0.2s animation "buffer"
            if (_fireDelayTimer >= 0.3f)
            {
                rifle.RifleShoot(mouseWorldPosition);
            }
        }
        else
        {
            // 3. Reset everything when the button is released
            _fireDelayTimer = 0f;         
        }

        if (_isMagzineEmpty && !_isBulletInventoryEmpty) input.ReloadInput(true);
    }
    private void ResetCamara()
    {
        if (aimVirtualCamera.gameObject.activeSelf) aimVirtualCamera.gameObject.SetActive(false);
        thirdPersonController.SetSensitivity(sensitivity);
        thirdPersonController.SetRotated(true);
    }

    private void PlayerAutoTurn()
    {
        thirdPersonController.SetRotated(false);
        transform.forward = Vector3.Slerp(transform.forward, aimDirection, Time.deltaTime * turnSpeed);
    }

    public void GrenadeThrow()
    {
        //Reset shooting delay
        _fireDelayTimer = 0f;
        // POOLING OPPORTUNITY: If you throw many grenades, pool this!
        GameObject grenade = Instantiate(grenade1, grenadeSpawnPoint.position, Quaternion.identity);
        Rigidbody rb = grenade.GetComponent<Rigidbody>();

        Vector3 throwDirection = mainCam.transform.forward;
        throwDirection.y += 0.2f;

        float baseThrowForce = 12f;
        rb.linearVelocity = characterController.velocity + (throwDirection.normalized * baseThrowForce);
    }

    public void GrenadeThrowRifleSwapHand()
    {
        // Avoid calling SetActive every frame - only call on state change
        
        if (rightHandRifle.activeSelf)
        {
            rightHandRifle.SetActive(false);
            leftHandRifle.SetActive(true);
        } else
        {
            rightHandRifle.SetActive(true);
            leftHandRifle.SetActive(false);
        }

        
    }

    private void RigHandler()
    {
        float targetAim = 0f;
        float gunHold = 0f;
        

        if (!_reload && !_throw)
        {
            targetAim = _aim|| _shoot ? 1f : 0f;
            gunHold = _aim ? 0f : 1f;
            
        }
        if (_sprint)
        {
            targetAim = 0f;
            gunHold = 1f;
        }

        // Smooth weight transitions
        aimRig.weight = Mathf.MoveTowards(aimRig.weight, targetAim, Time.deltaTime * transitionSpeed);
        //aimRig.weight = targetAim;
        holdGunRig.weight = Mathf.MoveTowards(holdGunRig.weight, gunHold, Time.deltaTime * transitionSpeed);
    }

    public void SetCrosshairVisible(bool value)
    {
        if (crossHair != null) crossHair.enabled = value;

    }

    private void UpDatePlayerData()
    {
        if(health!=null) playerInventoryData.health = Mathf.RoundToInt(health.GetHealth());

    }

    public bool IsDead()
    {
       if(health!=null) return health.IsDead();
       return false;
    }
}
