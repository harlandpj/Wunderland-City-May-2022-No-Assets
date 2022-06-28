using UnityEngine;
#if ENABLE_INPUT_SYSTEM && STARTER_ASSETS_PACKAGES_CHECKED
using UnityEngine.InputSystem;
#endif

namespace StarterAssets
{
	public class StarterAssetsInputs : MonoBehaviour
	{
		[Header("Character Input Values")]
		public Vector2 move;
		public Vector2 look;
		public bool jump;
		public bool sprint;

		// added for Wunderland project
		public bool hasGun;
		public bool hasRifle;
		public bool shootGun;
		public bool shootRifle;
		public bool aimRifle;
		public bool throwGrenade;
		public bool talking;
		public bool dancing;

		[Header("Movement Settings")]
		public bool analogMovement;

#if !UNITY_IOS || !UNITY_ANDROID
		[Header("Mouse Cursor Settings")]
		public bool cursorLocked = true;
		public bool cursorInputForLook = true;
#endif

#if ENABLE_INPUT_SYSTEM && STARTER_ASSETS_PACKAGES_CHECKED
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

		public void OnJump(InputValue value)
		{
			JumpInput(value.isPressed);
		}

		public void OnSprint(InputValue value)
		{
			SprintInput(value.isPressed);
		}

		// Wunderland specific inputations
		public void OnHasGun(InputValue value)
		{
			HasGunInput(value.isPressed);
		}

		public void OnHasRifle(InputValue value)
		{
			HasRifleInput(value.isPressed);
		}

		public void OnShootGun(InputValue value)
		{
			ShootGunInput(value.isPressed);
		}

		public void OnAimRifle(InputValue value)
		{
			AimRifleInput(value.isPressed);
		}

		public void OnShootRifle(InputValue value)
		{
			ShootRifleInput(value.isPressed);
		}

		public void OnThrowGrenade(InputValue value)
		{
			ThrowGrenadeInput(value.isPressed);
		}

		public void OnDancing(InputValue value)
		{
			DancingInput(value.isPressed);
		}

		public void OnTalking(InputValue value)
		{
			TalkingInput(value.isPressed);
		}

#else
	// old input sys if we do decide to have it (most likely wont)...
#endif


		public void MoveInput(Vector2 newMoveDirection)
		{
			move = newMoveDirection;
		} 

		public void LookInput(Vector2 newLookDirection)
		{
			look = newLookDirection;
		}

		public void JumpInput(bool newJumpState)
		{
			jump = newJumpState;
		}

		public void SprintInput(bool newSprintState)
		{
			sprint = newSprintState;
		}

		// wunderland specific additions below
		public void HasGunInput(bool newGunState)
		{
			hasGun = newGunState;
		}

		public void HasRifleInput(bool newRifleState)
		{
			hasRifle = newRifleState;
		}
		
		public void AimRifleInput(bool newAimRifleState)
		{
			aimRifle = newAimRifleState;
		}
		
		public void ShootGunInput(bool newShootGunState)
		{
			shootGun = newShootGunState;
		}

		public void ShootRifleInput(bool newShootRifleState)
		{
			shootRifle = newShootRifleState;
		}

		public void ThrowGrenadeInput(bool newThrowGrenadeState)
		{
			throwGrenade = newThrowGrenadeState;
		}
		
		public void TalkingInput(bool newTalkingState)
		{
			talking = newTalkingState;
		}
		
		public void DancingInput(bool newDancingState)
		{
			dancing = newDancingState;
		}

#if !UNITY_IOS || !UNITY_ANDROID

		private void OnApplicationFocus(bool hasFocus)
		{
			SetCursorState(cursorLocked);
		}

		private void SetCursorState(bool newState)
		{
			Cursor.lockState = newState ? CursorLockMode.Locked : CursorLockMode.None;
		}

#endif

	}
	
}