using DG.Tweening; 
using StarterAssets;

using Unity.Cinemachine;
using UnityEngine;




public class CarController : MonoBehaviour
{
    [Header("Input Reference")]
    [SerializeField] private StarterAssetsInputs inputs; // Player here
    [SerializeField] private bool isDriving = false;

    [Header("Wheel Colliders")]
    [SerializeField] private WheelCollider flCol; 
    [SerializeField] private WheelCollider frCol;
    [SerializeField] private WheelCollider blCol; 
    [SerializeField] private WheelCollider brCol;

    [Header("Wheel Visuals")]
    [SerializeField] private Transform flModel; 
    [SerializeField] private Transform frModel;
    [SerializeField] private Transform blModel; 
    [SerializeField] private Transform brModel;

    [Header("Light")]
    [SerializeField] private GameObject backLight;
    [SerializeField] private GameObject frontLight;

    [Header("Camera Settings")]
    [SerializeField] private GameObject camPosition;
    [SerializeField] private CinemachineCamera carCam;
    [SerializeField] private float sensitivity = 1.0f;
    [SerializeField] private float topClamp = 70.0f;
    [SerializeField] private float bottomClamp = -30.0f;
    [SerializeField] private float cameraAngleOverride = 0f;

    private float _cinemachineTargetYaw;
    private float _cinemachineTargetPitch;
    private const float _threshold = 0.01f;

   
    [Header("Settings")]
    [SerializeField] private float maxSpeedMS = 30f; // Approx 108 km/h
    [SerializeField] private float accelerationTorque = 2500f;
    [SerializeField] private float brakeForce = 3000f;
    [SerializeField] private float maxSteerAngle = 35f;
    [SerializeField] private float steerSpeedReduction = 0.5f; // How much to slow down when turning
    

    [Header("Damage System")]

    [SerializeField] private float minSpeedForDamage = 5f; // Minimum m/s to cause damage
    [SerializeField] private float damageScale = 2.5f;
    [SerializeField] private float enemyDamageScale = 5f;

    

    [Header("UI")]
    [SerializeField] private CarUIManager carUIManager;
    

    private Car car;
    private Health health;

    

    private void Awake()
    {
        car = GetComponent<Car>();
        health = GetComponent<Health>();
        
        
    }

    private void OnEnable()
    {
        BackLight(true);
        FrontLight(true);

        // FORCE the camera to target the car root or a specific target on the car
        carCam.Follow = camPosition.transform;
        //carCam.LookAt = camPosition.transform;

        if (carUIManager != null) carUIManager.enabled = true;

        carCam.Priority = 30;
    }
    private void OnDisable()
    {
        BackLight(false);
        FrontLight(false);
        if (carUIManager != null) carUIManager.enabled = false;
        carCam.Priority = 0;
    }

    private void FixedUpdate()
    {
        if (!isDriving || inputs == null) return;

        HandleCameraRotation();

        // 1. Calculate Current Speed (m/s)
        // Using localVelocity.z tells us speed in the direction the car is facing
        Rigidbody rb = GetComponent<Rigidbody>();
        float currentSpeed = transform.InverseTransformDirection(rb.linearVelocity).z;

        
        
        if (carUIManager != null)
        {
            carUIManager.UpdateUI(currentSpeed, health.GetHealth());
        }
        // 2. Handle Steering with Speed Reduction
        // Real cars slow down slightly when turning sharply
        float steerInput = inputs.steer;
        float steerAngle = steerInput * maxSteerAngle;
        flCol.steerAngle = steerAngle;
        frCol.steerAngle = steerAngle;

        // 3. Smart Motor Torque (Max Speed Limiting)
        float driveInput = inputs.drive;
        float currentTorque = 0;


        // Only apply torque if we are below max speed
        if (Mathf.Abs(currentSpeed) < maxSpeedMS)
        {
            currentTorque = driveInput * accelerationTorque;

            // Reduce torque while steering to simulate tire scrub/friction
            float steerFactor = Mathf.Lerp(1f, steerSpeedReduction, Mathf.Abs(steerInput));
            currentTorque *= steerFactor;
        }
        else
        {
            // Speed Limiter: Apply 0 torque or very slight negative torque
            currentTorque = 0;
        }

        // Apply to Rear Wheels
        blCol.motorTorque = currentTorque;
        brCol.motorTorque = currentTorque;

        // 4. Handle Braking & Visuals (Keep your existing code below)
        float currentBrake = inputs.brake ? brakeForce : 0f;
        ApplyBrakes(currentBrake);

        UpdateWheelPos(flCol, flModel);
        UpdateWheelPos(frCol, frModel);
        UpdateWheelPos(blCol, blModel);
        UpdateWheelPos(brCol, brModel);
    }

    private void Update()
    {
        if (inputs.exit)
        {
            //ApplyBrakes(brakeForce);
            car.ExitCar();
            inputs.exit = false;
        }
        
        
        
    }
    private void ApplyBrakes(float force)
    {
        flCol.brakeTorque = force; 
        frCol.brakeTorque = force;
        blCol.brakeTorque = force; 
        brCol.brakeTorque = force;
    }

    private void UpdateWheelPos(WheelCollider col, Transform trans)
    {
        Vector3 pos; Quaternion rot;
        col.GetWorldPose(out pos, out rot);
        trans.position = pos;
        trans.rotation = rot;
    }

    public bool IsDriving() => isDriving;

    public void SetDriving(bool isDriving)
    {
        this.isDriving = isDriving;
    }

    private void BackLight(bool value)
    {
        if(backLight!= null) backLight.gameObject.SetActive(value);
    }

    private void FrontLight(bool value)
    {
        if(frontLight!=null) frontLight.gameObject.SetActive(value);
    }



    private void OnCollisionEnter(Collision collision)
    {
        
        float impactSpeed = collision.relativeVelocity.magnitude;

        if (impactSpeed > minSpeedForDamage)
        {

            IDamageAble target = collision.gameObject.GetComponentInParent<IDamageAble>();

            if (target != null)
            {
                // Calculate damage for the enemy based on impact speed
                // You might want a different multiplier for enemies than for the car
                float damageToEnemy = (impactSpeed - minSpeedForDamage) * enemyDamageScale;
                target.TakeDamage(damageToEnemy);

                Debug.Log($"Hit {collision.gameObject.name} for {damageToEnemy} damage!");
            }
            // Check where the hit happened
            foreach (ContactPoint contact in collision.contacts)
            {
                // If the hit is on the bottom of the car, ignore it or reduce it
                // transform.InverseTransformPoint converts the hit to "Car Space"
                Vector3 localHitPoint = transform.InverseTransformPoint(contact.point);

                if (localHitPoint.y < 0.2f) // The hit is near the wheels/undercarriage
                {
                    continue;
                }
                if (target != null)
                {
                    if (target.ObjectType() == 1 || target.ObjectType() == 2) break;
                }
                ApplyDamage((impactSpeed - minSpeedForDamage) * damageScale);
                break; // Only apply damage once per collision
            }
        }
    }

    private void ApplyDamage(float amount)
    {
        
        health.TakeDamage(amount);
        

        // Performance Impact: Reduce engine power as health drops
        if (health.GetHealth() < health.GetDamageThreshold())
        {
            
            accelerationTorque = 500f;
            Debug.Log("Engine is failing...");
        }
        

        if (health.IsDead())
        {
            //Totaled();
        }
    }

    private void Totaled()
    {
        isDriving = false;
        accelerationTorque = 0;
        ApplyBrakes(brakeForce); 
        
    }

    private void HandleCameraRotation()
    {
        // 1. Get input from your existing inputs reference
        // (Assuming carController.inputs or similar is accessible)
        Vector2 lookInput = inputs.look;

        if (lookInput.sqrMagnitude >= _threshold)
        {
            // Mouse uses 1.0f multiplier, controllers use Time.deltaTime
            // If you only care about mouse for now, use 1.0f
            float deltaTimeMultiplier = 1.0f;

            _cinemachineTargetYaw += lookInput.x * deltaTimeMultiplier * sensitivity;
            _cinemachineTargetPitch -= lookInput.y * deltaTimeMultiplier * sensitivity;
        }

        // 2. Clamp the angles
        _cinemachineTargetYaw = ClampAngle(_cinemachineTargetYaw, float.MinValue, float.MaxValue);
        _cinemachineTargetPitch = ClampAngle(_cinemachineTargetPitch, bottomClamp, topClamp);

        // 3. Apply to the camPosition object
        // This is the object your Cinemachine Camera is following/looking at
        camPosition.transform.rotation = Quaternion.Euler(
            _cinemachineTargetPitch + cameraAngleOverride,
            _cinemachineTargetYaw,
            0.0f
        );
    }

    private static float ClampAngle(float lfAngle, float lfMin, float lfMax)
    {
        if (lfAngle < -360f) lfAngle += 360f;
        if (lfAngle > 360f) lfAngle -= 360f;
        return Mathf.Clamp(lfAngle, lfMin, lfMax);
    }


}