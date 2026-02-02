using System.Collections;
using UnityEngine;
#if ENABLE_INPUT_SYSTEM 
using UnityEngine.InputSystem;
#endif

/* Note: animations are called via the controller for both the character and capsule using animator null checks
 */

namespace StarterAssets
{
    [RequireComponent(typeof(CharacterController))]
#if ENABLE_INPUT_SYSTEM 
    [RequireComponent(typeof(PlayerInput))]
#endif
    public class ThirdPersonController : MonoBehaviour
    {
        [Header("Player")]
        [Tooltip("Move speed of the character in m/s")]
        public float MoveSpeed = 6f;

        [Tooltip("Sprint speed of the character in m/s")]
        public float SprintSpeed = 12f;

        [Tooltip("How fast the character turns to face movement direction")]
        [Range(0.0f, 0.3f)]
        public float RotationSmoothTime = 0.12f;

        [Tooltip("Acceleration and deceleration")]
        public float SpeedChangeRate = 10.0f;

        public AudioClip LandingAudioClip;
        public AudioClip[] FootstepAudioClips;
        [Range(0, 1)] public float FootstepAudioVolume = 0.5f;

        [Space(10)]
        [Tooltip("The height the player can jump")]
        public float JumpHeight = 1.2f;

        [Tooltip("The character uses its own gravity value. The engine default is -9.81f")]
        public float Gravity = -15.0f;

        [Space(10)]
        [Tooltip("Time required to pass before being able to jump again. Set to 0f to instantly jump again")]
        public float JumpTimeout = 0.50f;

        [Tooltip("Time required to pass before entering the fall state. Useful for walking down stairs")]
        public float FallTimeout = 0.15f;

        [Header("Player Grounded")]
        [Tooltip("If the character is grounded or not. Not part of the CharacterController built in grounded check")]
        public bool Grounded = true;

        [Tooltip("Useful for rough ground")]
        public float GroundedOffset = -0.14f;

        [Tooltip("The radius of the grounded check. Should match the radius of the CharacterController")]
        public float GroundedRadius = 0.28f;

        [Tooltip("What layers the character uses as ground")]
        public LayerMask GroundLayers;

        [Header("Cinemachine")]
        [Tooltip("The follow target set in the Cinemachine Virtual Camera that the camera will follow")]
        public GameObject CinemachineCameraTarget;

        [Tooltip("How far in degrees can you move the camera up")]
        public float TopClamp = 70.0f;

        [Tooltip("How far in degrees can you move the camera down")]
        public float BottomClamp = -30.0f;

        [Tooltip("Additional degress to override the camera. Useful for fine tuning camera position when locked")]
        public float CameraAngleOverride = 0.0f;

        [Tooltip("For locking the camera position on all axis")]
        public bool LockCameraPosition = false;
        private bool _isResettingCamera = false;
        [SerializeField] private float cameraResetSpeed = 15f;

        [SerializeField] private float sensitivity = 1f;

        [Header("Crouch Settings")]
        [SerializeField] private float standingHeight = 1.8f;
        [SerializeField] private float crouchingHeight = 1.0f;
        [SerializeField] private float timeToCrouch = 0.2f;
        private Coroutine crouchRoutine;

        [Header("Slide Settings")]
        [SerializeField] private float sprintTimeRequiredForSlide = 2.0f;
        [SerializeField] private float slideSpeed = 10f;
        [SerializeField] private float slideDuration = 1.0f;

        [Header("ScriptableObject")]
        [SerializeField] private PlayerInventoryData playerInventoryData;


        

        private float _sprintTimer = 0f;
        private bool isSliding = false;

        // cinemachine
        private float _cinemachineTargetYaw;
        private float _cinemachineTargetPitch;

        // player
        private float _speed;
        private float _animationBlend;
        private float _targetRotation = 0.0f;
        private float _rotationVelocity;
        private float _verticalVelocity;
        private float _terminalVelocity = 53.0f;

        // timeout deltatime
        private float _jumpTimeoutDelta;
        private float _fallTimeoutDelta;

        // animation IDs
        private int _animIDSpeed;
        private int _animIDGrounded;
        private int _animIDJump;
        private int _animIDFreeFall;
        private int _animIDMotionSpeed;
        private int _animIDAim;
        private int _animIDShoot;
        private int _animIDReload;
        private int _animIDCrouch;
        private int _animIDGrenade;
        private int _animIDSlide;

#if ENABLE_INPUT_SYSTEM 
        private PlayerInput _playerInput;
#endif

        [Header ("Others")]
        private Animator _animator;
        [SerializeField] private CharacterController _controller;
        [SerializeField] private StarterAssetsInputs _input;
        private GameObject _mainCamera;
        [SerializeField] private ThirdPersonShooterController playerController;

        private const float _threshold = 0.01f;

        private bool _hasAnimator;

        private bool isCrouch = false;
        private bool rotated = true;
        private bool _isReloading = false;
        private bool _isThrowGrenade = false;


        [SerializeField] private Rifle rifle;

        



        private bool IsCurrentDeviceMouse
        {
            get
            {
#if ENABLE_INPUT_SYSTEM
                return _playerInput.currentControlScheme == "KeyboardMouse";
#else
				return false;
#endif
            }
        }


        private void Awake()
        {
            // get a reference to our main camera
            //playerController = GetComponent<ThirdPersonShooterController>();
            if (_mainCamera == null)
            {
                _mainCamera = GameObject.FindGameObjectWithTag("MainCamera");
            }
        }

        private void Start()
        {
            _cinemachineTargetYaw = CinemachineCameraTarget.transform.rotation.eulerAngles.y;

            _hasAnimator = TryGetComponent(out _animator);
            //_controller = GetComponent<CharacterController>();
            //_input = GetComponent<StarterAssetsInputs>();
            
#if ENABLE_INPUT_SYSTEM 
            _playerInput = GetComponent<PlayerInput>();
#else
			Debug.LogError( "Starter Assets package is missing dependencies. Please use Tools/Starter Assets/Reinstall Dependencies to fix it");
#endif

            AssignAnimationIDs();

            // reset our timeouts on start
            _jumpTimeoutDelta = JumpTimeout;
            _fallTimeoutDelta = FallTimeout;
        }

        private void Update()
        {

            JumpAndGravity();
            GroundedCheck();

            if (Grounded && !_input.sprint && !isSliding)
            {
                if (!_input.reload && !_input.throwObject)
                {
                    AimShoot();
                }
                else if (_input.reload && !_input.throwObject) 
                {
                    ResetAimShootAnimation();
                    Reload();
                }
                else if (!_input.reload && _input.throwObject)
                {
                    ResetAimShootAnimation();
                    GrenadeThrow();
                }

            } 
            else if (_input.sprint || !Grounded)
            {
                ResetAimShootAnimation();
                isCrouch = false;
                //updateCrouch();
                ToggleCrouchState();
            }
            Crouch();
            Move();

            if ( _input.move != Vector2.zero && Grounded)
            {
                float chargeSpeed = _input.sprint ? 1.0f : 0.5f;

                _sprintTimer += Time.deltaTime * chargeSpeed;

            }
            else
            {
                _sprintTimer = 0f; // Reset timer if they stop sprinting or stop moving
            }

        }

        private void AimShoot()
        {

            if (_input.shoot || _input.aim )
            {
                _animator.SetBool(_animIDAim, _input.aim);
                if(!rifle.IsMagazineEmpty())
                    _animator.SetBool(_animIDShoot, _input.shoot);
                else
                {
                    _animator.SetBool(_animIDShoot, false);
                }
                    SetMaxSpeed(4f, 4f);    
            }
            else
            {
                ResetAimShootAnimation();
            }
        }

        private void ResetAimShootAnimation()
        {
            _animator.SetBool(_animIDShoot, false);
            _animator.SetBool(_animIDAim, false);
            ResetMaxSpeed();
        }
        private void Reload()
        {
            if (!_isReloading && !_isThrowGrenade)
            {   
                StartCoroutine(ReloadCoroutine());
            }
            
        }
        private IEnumerator ReloadCoroutine()
        {
            _isReloading = true;
            SetMaxSpeed(3f, 5f);
            _animator.SetBool(_animIDReload, true);

            yield return new WaitForSeconds(2.2f);

            
            rifle.Reload();
            yield return new WaitForSeconds(0.2f);
            if(isCrouch) yield return new WaitForSeconds(1.0f);
            _animator.SetBool(_animIDReload, false);
            ResetMaxSpeed();
            _input.reload = false; // Reset the input at the end
            _isReloading = false;
            _input.throwObject = false;
        }

        private void GrenadeThrow()
        {
            if (!_isThrowGrenade && !_isReloading)
            {
                StartCoroutine(ThrowGrenadeCoroutine());
            }
            
        }
        private IEnumerator ThrowGrenadeCoroutine()
        {
            _isThrowGrenade = true;
            SetRotated(false);
            SetMaxSpeed(1f, 1.5f);
            playerController.GrenadeThrowRifleSwapHand();

            _animator.SetBool(_animIDGrenade, true);
            yield return new WaitForSeconds(1.8f);

            playerController.GrenadeThrow();

            yield return new WaitForSeconds(0.6f);

            _animator.SetBool(_animIDGrenade, false);
            yield return new WaitForSeconds(0.2f);
            
            SetRotated(true);
            ResetMaxSpeed();
            _input.throwObject = false;
            playerController.GrenadeThrowRifleSwapHand();
            _isThrowGrenade = false;
            _input.reload = false;
            
        }

       
        private void Crouch()
        {
            if (!_input.crouch || !Grounded) return;

            // 1. Check for Slide condition
            if (_sprintTimer >= sprintTimeRequiredForSlide && !isCrouch)
            {
                StartCoroutine(PerformSlide());
            }
            // 2. Check for Standing Up condition
            else if (isCrouch && !CanStandUp())
            {
                Debug.Log("Ceiling too low!");
            }
            // 3. Normal Toggle
            else
            {
                isCrouch = !isCrouch;
                ToggleCrouchState();
            }

            _input.crouch = false;
        }

        public bool GetIsSlideing()
        {
            return isSliding;
        }
        private void ToggleCrouchState()
        {
            // Speed and Animator
            if (isCrouch) SetMaxSpeed(3f, 6f);
            else ResetMaxSpeed();

            _animator.SetBool(_animIDCrouch, isCrouch);

            // Reuse the same resize logic
            StartResize(isCrouch ? crouchingHeight : standingHeight);
        }

        // HELPER: One place to handle Coroutine management
        private void StartResize(float target)
        {
            if (crouchRoutine != null) StopCoroutine(crouchRoutine);
            crouchRoutine = StartCoroutine(AnimateHeight(target));
        }

        private IEnumerator AnimateHeight(float targetHeight)
        {
            float timer = 0;
            float startHeight = _controller.height;

            while (timer < timeToCrouch)
            {
                timer += Time.deltaTime;
                ApplyHeight(Mathf.Lerp(startHeight, targetHeight, timer / timeToCrouch));
                yield return null;
            }
            ApplyHeight(targetHeight);
        }

        // HELPER: The actual math for the CharacterController (Single Source of Truth)
        private void ApplyHeight(float height)
        {
            _controller.height = height;
            _controller.center = new Vector3(_controller.center.x, height / 2f, _controller.center.z);
        }

        private IEnumerator PerformSlide()
        {
            isSliding = true;
            isCrouch = true;
            _sprintTimer = 0f;

            _animator.SetBool(_animIDSlide, true);

            // Start the height shrink using the shared logic
            StartResize(crouchingHeight);

            float timer = 0f;
            while (timer < slideDuration)
            {
                timer += Time.deltaTime;
                _controller.Move(transform.forward * slideSpeed * Time.deltaTime);
                yield return null;
            }

            // Slide finished - try to stand up
            isSliding = false;
            _animator.SetBool(_animIDSlide, false);

            if (CanStandUp())
            {
                isCrouch = false;
                ToggleCrouchState();
            }
        }
        private bool CanStandUp()
        {
            // Start the sphere slightly above the current crouched center
            float sphereRadius = _controller.radius * 0.85f; // Slightly thinner than player
            Vector3 start = transform.position + Vector3.up * (crouchingHeight - sphereRadius);

            // Total distance from current crouch height to full standing height
            float distance = standingHeight - crouchingHeight + sphereRadius;

            // The ~PlayerLayer excludes the player (assuming Player is on its own layer)
            bool isBlocked = Physics.SphereCast(start, sphereRadius, Vector3.up, out RaycastHit hit, distance);

            // DEBUG: This lets you see the check in the Scene View
            Color debugColor = isBlocked ? Color.red : Color.green;
            Debug.DrawRay(start, Vector3.up * distance, debugColor);

            return !isBlocked;
        }

        private void LateUpdate()
        {
            if (_input.alt)
            {
                _isResettingCamera = true;
                _input.alt = false;
            }

            // Stop resetting if the player tries to move the camera manually
            if (_input.look.sqrMagnitude > 0.01f)
            {
                _isResettingCamera = false;
            }

            CameraRotation();
        }

        private void AssignAnimationIDs()
        {
            _animIDSpeed = Animator.StringToHash("Speed");
            _animIDGrounded = Animator.StringToHash("Grounded");
            _animIDJump = Animator.StringToHash("Jump");
            _animIDFreeFall = Animator.StringToHash("FreeFall");
            _animIDMotionSpeed = Animator.StringToHash("MotionSpeed");
            _animIDAim = Animator.StringToHash("Aim");
            _animIDShoot = Animator.StringToHash("Shoot");
            _animIDReload = Animator.StringToHash("Reload");
            _animIDCrouch = Animator.StringToHash("Crouch");
            _animIDGrenade = Animator.StringToHash("Grenade");
            _animIDSlide = Animator.StringToHash("Slide");
        }

        private void GroundedCheck()
        {
            // set sphere position, with offset
            Vector3 spherePosition = new Vector3(transform.position.x, transform.position.y - GroundedOffset,
                transform.position.z);
            Grounded = Physics.CheckSphere(spherePosition, GroundedRadius, GroundLayers,
                QueryTriggerInteraction.Ignore);

            // update animator if using character
            if (_hasAnimator)
            {
                _animator.SetBool(_animIDGrounded, Grounded);
            }
        }
        private void CameraRotation()
        {
            if (_isResettingCamera)
            {
                // Smoothly interpolate the values toward the target
                // Mathf.LerpAngle is crucial for Yaw to handle the 360 to 0 degree jump
                _cinemachineTargetYaw = Mathf.LerpAngle(_cinemachineTargetYaw, transform.eulerAngles.y, Time.deltaTime * cameraResetSpeed);
                _cinemachineTargetPitch = Mathf.Lerp(_cinemachineTargetPitch, 0f, Time.deltaTime * cameraResetSpeed);

                // Stop resetting once we are very close to the target
                if (Mathf.Abs(Mathf.DeltaAngle(_cinemachineTargetYaw, transform.eulerAngles.y)) < 0.1f)
                {
                    _isResettingCamera = false;
                }
            }
            else if (_input.look.sqrMagnitude >= _threshold && !LockCameraPosition)
            {
                // Normal camera movement code (Your existing logic)
                float deltaTimeMultiplier = IsCurrentDeviceMouse ? 1.0f : Time.deltaTime;
                _cinemachineTargetYaw += _input.look.x * deltaTimeMultiplier * sensitivity;
                _cinemachineTargetPitch += _input.look.y * deltaTimeMultiplier * sensitivity;
            }

            // Rest of your existing CameraRotation logic...
            _cinemachineTargetYaw = ClampAngle(_cinemachineTargetYaw, float.MinValue, float.MaxValue);
            _cinemachineTargetPitch = ClampAngle(_cinemachineTargetPitch, BottomClamp, TopClamp);

            CinemachineCameraTarget.transform.rotation = Quaternion.Euler(_cinemachineTargetPitch + CameraAngleOverride,
                _cinemachineTargetYaw, 0.0f);
        }
        private void CameraRotationOld()
        {
            if (_input.alt)
            {
                // Change 10f to a higher number to make it faster/sharper
                _cinemachineTargetYaw = Mathf.LerpAngle(_cinemachineTargetYaw, transform.eulerAngles.y, Time.deltaTime * 10f);
                _cinemachineTargetPitch = Mathf.Lerp(_cinemachineTargetPitch, 0f, Time.deltaTime * 10f);
                _input.alt = false;
            }
            // if there is an input and camera position is not fixed
            if (_input.look.sqrMagnitude >= _threshold && !LockCameraPosition)
            {
                //Don't multiply mouse input by Time.deltaTime;
                float deltaTimeMultiplier = IsCurrentDeviceMouse ? 1.0f : Time.deltaTime;

                _cinemachineTargetYaw += _input.look.x * deltaTimeMultiplier * sensitivity;
                _cinemachineTargetPitch += _input.look.y * deltaTimeMultiplier * sensitivity;
            }

            // clamp our rotations so our values are limited 360 degrees
            _cinemachineTargetYaw = ClampAngle(_cinemachineTargetYaw, float.MinValue, float.MaxValue);
            _cinemachineTargetPitch = ClampAngle(_cinemachineTargetPitch, BottomClamp, TopClamp);

            // Cinemachine will follow this target
            CinemachineCameraTarget.transform.rotation = Quaternion.Euler(_cinemachineTargetPitch + CameraAngleOverride,
                _cinemachineTargetYaw, 0.0f);
        }
       
        private void Move()
        {
            // set target speed based on move speed, sprint speed and if sprint is pressed
            float targetSpeed = _input.sprint ? SprintSpeed : MoveSpeed;

            // a simplistic acceleration and deceleration designed to be easy to remove, replace, or iterate upon

            // note: Vector2's == operator uses approximation so is not floating point error prone, and is cheaper than magnitude
            // if there is no input, set the target speed to 0
            if (_input.move == Vector2.zero) targetSpeed = 0.0f;

            // a reference to the players current horizontal velocity
            float currentHorizontalSpeed = new Vector3(_controller.velocity.x, 0.0f, _controller.velocity.z).magnitude;

            float speedOffset = 0.0f;
            float inputMagnitude = _input.analogMovement ? _input.move.magnitude : 1f;

            // accelerate or decelerate to target speed
            if (currentHorizontalSpeed < targetSpeed - speedOffset ||
                currentHorizontalSpeed > targetSpeed + speedOffset)
            {
                // creates curved result rather than a linear one giving a more organic speed change
                // note T in Lerp is clamped, so we don't need to clamp our speed
                _speed = Mathf.Lerp(currentHorizontalSpeed, targetSpeed * inputMagnitude,
                    Time.deltaTime * SpeedChangeRate);

                // round speed to 3 decimal places
                _speed = Mathf.Round(_speed * 1000f) / 1000f;
            }
            else
            {
                _speed = targetSpeed;
            }

            _animationBlend = Mathf.Lerp(_animationBlend, targetSpeed, Time.deltaTime * SpeedChangeRate);
            if (_animationBlend < 0.01f) _animationBlend = 0f;

            // normalise input direction
            Vector3 inputDirection = new Vector3(_input.move.x, 0.0f, _input.move.y).normalized;

            // note: Vector2's != operator uses approximation so is not floating point error prone, and is cheaper than magnitude
            // if there is a move input rotate player when the player is moving
            if (_input.move != Vector2.zero)
            {
                _targetRotation = Mathf.Atan2(inputDirection.x, inputDirection.z) * Mathf.Rad2Deg +
                                  _mainCamera.transform.eulerAngles.y;
                float rotation = Mathf.SmoothDampAngle(transform.eulerAngles.y, _targetRotation, ref _rotationVelocity,
                    RotationSmoothTime);

                // rotate to face input direction relative to camera position
                if (rotated)
                {
                    transform.rotation = Quaternion.Euler(0.0f, rotation, 0.0f);
                }

            }


            Vector3 targetDirection = Quaternion.Euler(0.0f, _targetRotation, 0.0f) * Vector3.forward;

            // move the player
            if (isCrouch && (_isThrowGrenade || _isReloading))
            {

            }
            else
            {
                _controller.Move(targetDirection.normalized * (_speed * Time.deltaTime) +
                            new Vector3(0.0f, _verticalVelocity, 0.0f) * Time.deltaTime);
            }


            // update animator if using character
            if (_hasAnimator)
            {
                _animator.SetFloat(_animIDSpeed, _animationBlend);
                _animator.SetFloat(_animIDMotionSpeed, inputMagnitude);
            }
        }

        private void JumpAndGravity()
        {
            if (Grounded && CanStandUp())
            {
                // reset the fall timeout timer
                _fallTimeoutDelta = FallTimeout;

                // update animator if using character
                if (_hasAnimator)
                {
                    _animator.SetBool(_animIDJump, false);
                    _animator.SetBool(_animIDFreeFall, false);
                }

                // stop our velocity dropping infinitely when grounded
                if (_verticalVelocity < 0.0f)
                {
                    _verticalVelocity = -2f;
                }

                // Jump
                if (_input.jump && _jumpTimeoutDelta <= 0.0f && !isSliding)
                {
                    // the square root of H * -2 * G = how much velocity needed to reach desired height
                    _verticalVelocity = Mathf.Sqrt(JumpHeight * -2f * Gravity);

                    // update animator if using character
                    if (_hasAnimator)
                    {
                        _animator.SetBool(_animIDJump, true);
                    }
                }

                // jump timeout
                if (_jumpTimeoutDelta >= 0.0f)
                {
                    _jumpTimeoutDelta -= Time.deltaTime;
                }
            }
            else
            {
                // reset the jump timeout timer
                _jumpTimeoutDelta = JumpTimeout;

                // fall timeout
                if (_fallTimeoutDelta >= 0.0f)
                {
                    _fallTimeoutDelta -= Time.deltaTime;
                }
                else
                {
                    // update animator if using character
                    if (_hasAnimator)
                    {
                        _animator.SetBool(_animIDFreeFall, true);
                    }
                }

                // if we are not grounded, do not jump
                _input.jump = false;
            }

            // apply gravity over time if under terminal (multiply by delta time twice to linearly speed up over time)
            if (_verticalVelocity < _terminalVelocity)
            {
                _verticalVelocity += Gravity * Time.deltaTime;
            }
        }

        private static float ClampAngle(float lfAngle, float lfMin, float lfMax)
        {
            if (lfAngle < -360f) lfAngle += 360f;
            if (lfAngle > 360f) lfAngle -= 360f;
            return Mathf.Clamp(lfAngle, lfMin, lfMax);
        }

        //private void OnDrawGizmosSelected()
        //{
        //    Color transparentGreen = new Color(0.0f, 1.0f, 0.0f, 0.35f);
        //    Color transparentRed = new Color(1.0f, 0.0f, 0.0f, 0.35f);

        //    if (Grounded) Gizmos.color = transparentGreen;
        //    else Gizmos.color = transparentRed;

        //    // when selected, draw a gizmo in the position of, and matching radius of, the grounded collider
        //    Gizmos.DrawSphere(
        //        new Vector3(transform.position.x, transform.position.y - GroundedOffset, transform.position.z),
        //        GroundedRadius);
        //}

        private void OnFootstep(AnimationEvent animationEvent)
        {
            if (animationEvent.animatorClipInfo.weight > 0.5f)
            {
                if (FootstepAudioClips.Length > 0)
                {
                    var index = Random.Range(0, FootstepAudioClips.Length);
                    AudioSource.PlayClipAtPoint(FootstepAudioClips[index], transform.TransformPoint(_controller.center), FootstepAudioVolume);
                }
            }
        }

        private void OnLand(AnimationEvent animationEvent)
        {
            if (animationEvent.animatorClipInfo.weight > 0.2f)
            {
                AudioSource.PlayClipAtPoint(LandingAudioClip, transform.TransformPoint(_controller.center), FootstepAudioVolume);
            }
        }
        public void SetSensitivity(float value)
        {
            sensitivity = value;
        }
        public void SetRotated(bool value)
        {
            rotated = value;
        }
        public void SetMaxSpeed(float MoveSpeed, float SprintSpeed)
        {
            this.MoveSpeed = MoveSpeed;
            this.SprintSpeed = SprintSpeed;
        }
        public void ResetMaxSpeed()
        {
            MoveSpeed = 6f;
            SprintSpeed = 12f;
        }
    }
}