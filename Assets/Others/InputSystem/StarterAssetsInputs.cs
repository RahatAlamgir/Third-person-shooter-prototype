using UnityEngine;
#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem;
#endif

namespace StarterAssets
{
	public class StarterAssetsInputs : MonoBehaviour
	{
		[Header("Character Input Values")]
		public Vector2 move;
		public Vector2 look;
		public Vector2 arrow;
		public bool jump;
		public bool sprint;
		public bool aim;
		public bool shoot;
		public bool interact;
		public bool reload;
		public bool crouch;
		public bool throwObject;
		public bool tab;
		public bool esc;
		public bool alt;

        [Header("Car Input Values")]
        public float drive;
        public float steer;
        public bool brake;
        public bool exit;

        [Header("Movement Settings")]
		public bool analogMovement;

		[Header("Mouse Cursor Settings")]
		public bool cursorLocked = true;
		public bool cursorInputForLook = true;

#if ENABLE_INPUT_SYSTEM
		public void OnMove(InputValue value)
		{
			MoveInput(value.Get<Vector2>());
		}

		public void OnLook(InputValue value)
		{
			if(cursorInputForLook)
			{
				LookInput(value.Get<Vector2>());
			}
		}
		public void OnArrow(InputValue value)
		{
			ArrowInput(value.Get<Vector2>());
		}

		public void OnJump(InputValue value)
		{
			JumpInput(value.isPressed);
		}

		public void OnSprint(InputValue value)
		{
			SprintInput(value.isPressed);
		}
		public void OnAim(InputValue value)
		{
			AimInput(value.isPressed);
		}
		public void OnShoot(InputValue value)
		{
			ShootInput(value.isPressed);
		}
		public void OnInteract(InputValue value)
		{
			InteractInput(value.isPressed);
		}
		public void OnReload(InputValue value)
		{
			ReloadInput(value.isPressed);
		}
		public void OnCrouch(InputValue value)
		{
			CrouchInput(value.isPressed);
		}
		public void OnThrow(InputValue value)
		{
			ThrowObjectInput(value.isPressed);
		}
		public void OnTab(InputValue value)
		{
			TabInput(value.isPressed);
		}
		public void OnEsc(InputValue value)
		{
			EscInput(value.isPressed);
		}
		public void OnAlt(InputValue value)
		{
			AltInput(value.isPressed);
		}

		// Car

		public void OnDrive(InputValue value)
        {
			DriveInput(value.Get<float>());
        }

		public void OnSteer(InputValue value)
		{
			SteerInput(value.Get<float>());
		}
		public void OnBrake(InputValue value)
		{
			BrakeInput(value.isPressed);
		}

		public void OnExit(InputValue value)
		{
			ExitInput(value.isPressed);
		}

#endif


        public void MoveInput(Vector2 newMoveDirection)
		{
			move = newMoveDirection;
		} 

		public void LookInput(Vector2 newLookDirection)
		{
			look = newLookDirection;
		}
		public void ArrowInput(Vector2 newArrowDirection)
		{
			arrow = newArrowDirection;
		}

		public void JumpInput(bool newJumpState)
		{
			jump = newJumpState;
		}

		public void SprintInput(bool newSprintState)
		{
			sprint = newSprintState;
		}
		public void AimInput(bool newAimState)
		{
			aim = newAimState;
		}
		public void ShootInput(bool newShootState)
		{
			shoot = newShootState;
		}
		public void InteractInput(bool newInteractState)
		{
			interact = newInteractState;
		}
		public void ReloadInput(bool newReloadState)
		{
			reload = newReloadState;
		}
		public void CrouchInput(bool newCrouchState)
		{
			crouch = newCrouchState;
		}
		public void ThrowObjectInput(bool newThrowState)
		{
			throwObject = newThrowState;
		}
		public void TabInput(bool newTabState)
		{
			tab = newTabState;
		}
		public void EscInput(bool newEscState)
		{
			esc = newEscState;
		}
		public void AltInput(bool newAltState)
		{
			alt = newAltState;
		}

		// Car input

		public void DriveInput(float newDriveState)
		{
			drive = newDriveState;
		}
		public void SteerInput(float newSteerState)
		{
			steer = newSteerState;
		}
		public void BrakeInput(bool newBrakeState)
		{
			brake = newBrakeState;
		}
		public void ExitInput(bool newExitState)
		{
			exit = newExitState;
		}


		private void OnApplicationFocus(bool hasFocus)
		{
			SetCursorState(cursorLocked);
		}

		private void SetCursorState(bool newState)
		{
			Cursor.lockState = newState ? CursorLockMode.Locked : CursorLockMode.None;
		}
	}
	
}