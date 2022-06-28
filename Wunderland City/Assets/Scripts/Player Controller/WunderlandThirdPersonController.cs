using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

using StarterAssets; // third person controller

// subclass of ThirdPersonController to add additional functionality
//
public class WunderlandThirdPersonController : MonoBehaviour
{
	//public static WunderlandThirdPersonController Instance { get; private set; } // shared by all instances
	private static WunderlandThirdPersonController _instance;

	public static WunderlandThirdPersonController Instance
    {
        get
        {
            if (_instance == null)
            {
                GameObject go = new GameObject("GameManagement");
                go.AddComponent<GameManagement>();
            }
            return _instance;
        }
    }

    [Header("Player")]
    [Tooltip("Move speed of the character in m/s")]
    public float MoveSpeed = 2.0f;
    [Tooltip("Sprint speed of the character in m/s")]
    public float SprintSpeed = 5.335f;
    [Tooltip("How fast the character turns to face movement direction")]
    [Range(0.0f, 0.3f)]
    public float RotationSmoothTime = 0.12f;
    [Tooltip("Acceleration and deceleration")]
    public float SpeedChangeRate = 10.0f;

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

    // cinemachine
    private float _cinemachineTargetYaw;
	private float _cinemachineTargetPitch;

	// audio
	private AudioSource playerAudio;

	[Header("Player and Weapon Sounds")]
	[SerializeField]
	private AudioClip rifleFiredNoise;
	[SerializeField]
	private AudioClip pistolFiredNoise;
	[SerializeField]
	private AudioClip grenadeExplosionNoise;
	[SerializeField]
	private AudioClip weaponNotFoundNoise;

	[SerializeField]
	private AudioClip jumpingNoise;
	[SerializeField]
	private AudioClip walkingNoise;
	//[SerializeField]
	//private AudioClip runningNoise;
	[SerializeField]
	private AudioClip heartbeatNoise;
	[SerializeField]
	private AudioClip dyingNoise;
	
	// player
	private float _speed;
	private float _animationBlend;
	private float _targetRotation = 0.0f;
	private float _rotationVelocity;
	private float _verticalVelocity;
	private float _terminalVelocity = 53.0f;

	// aiming camera (smaller FOV and zoomed in)
	[Header("Aiming Look Speed Percentage")]
	[Tooltip("Set to a Percentage of the ORIGINAL CAMERA LOOK Speed e.g. '0.45' is 45%!")]
	[Range(0.10f,1.0f)]
	[SerializeField]
	private float aimSpeedPercent; // reduce sensitivity when aiming

	// weapons
	[SerializeField]
	private GameObject bullet;
	//TMPro
	[SerializeField]
	private GameObject rifleClip;

	

	[Header("Weapons")]
	[Tooltip("Examine Player Spine and look at Hands to position Weapon Objects!")]
	[SerializeField]
	private GameObject rifle; // setup via inspector

	// weapon effects
	[Header("Weapon Effects")]
	[Tooltip("If changing an effect, ensure it is for the SAME TYPE of effect e.g. bullet impact as was in field before! Add new ones at end!")]
	[SerializeField]
	private GameObject[] weaponEffects; // array setup in editor of effects to add on hitting something

	// weapon status
	[Header("Weapon Status")]
	[Tooltip("has player collected this weapon?")]
	[SerializeField]
	public bool hasRifle; // does player have a rifle

	private bool hasPistol; // does player have a pistol
	private bool hasGrenade; // does player have grenade(s)
	private int numberGrenades; // how many grenades
	
	[Header("Number First Aid Kits")]
	[Tooltip("how many first aid kits held")]
	[SerializeField]
	private int firstAidKitsHeld;
	
	[SerializeField]
	[Tooltip("percent increase on using health kit")]
	private int firstAidKitValue;

	[Header("Number Rifle Clips Held")]
	[Tooltip("how many rifle clips held")]
	[SerializeField]
	private int rifleClipsHeld;
	
	// animation specific additional rotations due to character feature positions
	private float aimRifleYRotationAdd = 30f;
	private float aimRifleRunYRotationAdd = 15f;

	// timeout deltatime
	private float _jumpTimeoutDelta;
	private float _fallTimeoutDelta;

	private bool aimActive = false;

	// animation IDs
	private int _animIDSpeed;
	private int _animIDGrounded;
	private int _animIDJump;
	private int _animIDFreeFall;
	private int _animIDMotionSpeed;

	// Wunderland specific animation IDs
	private int _animIDHasGun;
	private int _animIDHasRifle;
	private int _animIDShootGun;
	private int _animIDAimRifle;
	private int _animIDShootRifle;
	private int _animIDThrowGrenade;
	private int _animIDTalking;
	private int _animIDDancing;

	private Animator _animator;
	private CharacterController _controller;
	private WunderlandStarterAssetsInputs _input;
	private GameObject _mainCamera;
	
	// aim Camera
	[Header("Aim Camera and Aim Reticule")]
	[Tooltip("Place the camera used for Weapon Aiming here!")]
	public GameObject aimCamera; // smaller FOV camera setup for aiming
	[Tooltip("Point To aim FROM (at Player)!")]
	public GameObject reticuleSearching; // image to display when searching
	[Tooltip("Point to aim THROUGH - the Reticule (gunsight)")]
	public GameObject aimFromHere; // start point slightly in front of player for aiming

	// screen centre
	Vector2 screenCentrePoint;
	
	private const float _threshold = 0.01f;

	private bool _hasAnimator;

	private void SetupPlayerVars()
    {
		// setup initial state for player
		hasRifle = false;
		hasPistol = false;
		hasGrenade = false;
		numberGrenades = 0;
    }

	public void SetHasRifle(bool rifleFound)
    {
		hasRifle = rifleFound;
    }

	public void SetFirstAidKitCollected()
    {
		firstAidKitsHeld++;
    }

	public void SetRifleClipCollected()
	{
		rifleClipsHeld++;
	}

	private void Awake()
	{
		// check if we have been created already in another scene
		//_instance = this;
		//DontDestroyOnLoad(gameObject);

		// get a reference to our main camera
		if (_mainCamera == null)
		{
			_mainCamera = GameObject.FindGameObjectWithTag("MainCamera");
		}

		// initialise player variables
		SetupPlayerVars();

		// centre of screen for targetting
		screenCentrePoint = new Vector2(Screen.width * 0.5f, Screen.height * 0.5f);

		if (SceneManager.GetActiveScene().buildIndex >=3)
        {
			hasRifle = true; // always has a rifle from here on
        }
        else
        {
			hasRifle = false;
        }
	}

	private void Start()
	{
		// reset spawn location for player

		transform.position = new Vector3(0, 0, 0); // NEEDED OTHERWISE UNITY THROWS A WOBBLY! DON'T EVER REMOVE THIS LINE!
		transform.Rotate(0, 180, 0);

		_hasAnimator = TryGetComponent(out _animator);
		_controller = GetComponent<CharacterController>();
		
		_input = GetComponent<WunderlandStarterAssetsInputs>();

		// find aimCamera, and aimReticule which need to be turned off initially
		if (aimCamera == null)
		{
			aimCamera = GameObject.FindGameObjectWithTag("AimCamera"); 
		}
			aimCamera.SetActive(false);

		if (reticuleSearching == null)
		{
			reticuleSearching = GameObject.FindGameObjectWithTag("ReticuleSearching"); 
		}
		
		reticuleSearching.SetActive(false);
		TryGetComponent(out playerAudio);  // get audio source

		if (rifle == null)
		{
			rifle = GameObject.FindGameObjectWithTag("Rifle");
		}
		
		rifle.SetActive(false);
		playerAudio.loop = false;
		playerAudio.Stop();
		AssignAnimationIDs();

		// reset our timeouts on start
		_jumpTimeoutDelta = JumpTimeout;
		_fallTimeoutDelta = FallTimeout;

		// move to first position to stop falling thru world on scene entries!
		_controller.Move(transform.position);
		_input.move = Vector2.zero;
	}

	private void TurnOffReticule()
    {
		reticuleSearching.SetActive(false);
    }

	private void TurnOnReticule()
	{
		reticuleSearching.SetActive(true);
	}

	private void Update()
	{
		_hasAnimator = TryGetComponent(out _animator);

		JumpAndGravity();
		GroundedCheck();
		Move();

		// Wunderland specific actions
		HasWeaponCheck();
		AimRifleCheck();
		ShootGunCheck();
		ShootRifleCheck();
		ThrowGrenadeCheck();
		Dancing();
		Talking();
	}

    private void LateUpdate()
	{
		CameraRotation();
	}

	private void AssignAnimationIDs()
	{
		_animIDSpeed = Animator.StringToHash("Speed");
		_animIDGrounded = Animator.StringToHash("Grounded");
		_animIDJump = Animator.StringToHash("Jump");
		_animIDFreeFall = Animator.StringToHash("FreeFall");
		_animIDMotionSpeed = Animator.StringToHash("MotionSpeed");

		// *** Wunderland specific assignments that MUST MATCH EXACTLY
		// the names of the animator variables setup for Player character!
		_animIDHasGun = Animator.StringToHash("HasGun");
		_animIDHasRifle = Animator.StringToHash("HasRifle");
		_animIDShootGun = Animator.StringToHash("ShootGun");
		_animIDAimRifle = Animator.StringToHash("AimRifle");
		_animIDShootRifle = Animator.StringToHash("ShootRifle");
		_animIDThrowGrenade = Animator.StringToHash("ThrowGrenade");
		_animIDTalking = Animator.StringToHash("Talking");
		_animIDDancing = Animator.StringToHash("Dancing");
		// *** Wunderland specific assignments that MUST MATCH EXACTLY!
	}

	private void GroundedCheck()
	{
		// set sphere position, with offset
		Vector3 spherePosition = new Vector3(transform.position.x, transform.position.y - GroundedOffset, transform.position.z);
		Grounded = Physics.CheckSphere(spherePosition, GroundedRadius, GroundLayers, QueryTriggerInteraction.Ignore);

		// update animator if using character
		if (_hasAnimator)
		{
			_animator.SetBool(_animIDGrounded, Grounded);
		}
	}

	private void CameraRotation()
	{
		// if there is an input and camera position is not fixed
		if (_input.look.sqrMagnitude >= _threshold && !LockCameraPosition)
		{
			// check if aiming active
			if (_input.aimRifle || aimActive)
            {
				//Debug.Log("Has Rifle and Aiming!");
				_cinemachineTargetYaw += (_input.look.x * aimSpeedPercent) * Time.deltaTime;
				_cinemachineTargetPitch += (_input.look.y * aimSpeedPercent)* Time.deltaTime;
			}
            else
            {
				_cinemachineTargetYaw += _input.look.x * Time.deltaTime;
				_cinemachineTargetPitch += _input.look.y * Time.deltaTime;
			}
			
		}

		// clamp our rotations so our values are limited 360 degrees
		_cinemachineTargetYaw = ClampAngle(_cinemachineTargetYaw, float.MinValue, float.MaxValue);
		_cinemachineTargetPitch = ClampAngle(_cinemachineTargetPitch, BottomClamp, TopClamp);

		// Cinemachine will follow this target
		CinemachineCameraTarget.transform.rotation = Quaternion.Euler(_cinemachineTargetPitch + CameraAngleOverride, _cinemachineTargetYaw, 0.0f);
	}

	
	private bool idleAudioIsPlaying = true;
	private bool moveAudioIsPlaying = false;

	private void Move()
	{
		// set target speed based on move speed, sprint speed and if sprint is pressed
		float targetSpeed = _input.sprint ? SprintSpeed : MoveSpeed;

		// change sounds here

		// a simplistic acceleration and deceleration designed to be easy to remove, replace, or iterate upon

		// note: Vector2's == operator uses approximation so is not floating point error prone, and is cheaper than magnitude
		// if there is no input, set the target speed to 0
		if (_input.move == Vector2.zero) targetSpeed = 0.0f;

		// a reference to the players current horizontal velocity
		float currentHorizontalSpeed = new Vector3(_controller.velocity.x, 0.0f, _controller.velocity.z).magnitude;

		float speedOffset = 0.1f;
		float inputMagnitude = _input.analogMovement ? _input.move.magnitude : 1f;

			// accelerate or decelerate to target speed
		if (currentHorizontalSpeed < targetSpeed - speedOffset || currentHorizontalSpeed > targetSpeed + speedOffset)
		{
			// creates curved result rather than a linear one giving a more organic speed change
			// note T in Lerp is clamped, so we don't need to clamp our speed
			_speed = Mathf.Lerp(currentHorizontalSpeed, targetSpeed * inputMagnitude, Time.deltaTime * SpeedChangeRate);

			// round speed to 3 decimal places
			_speed = Mathf.Round(_speed * 1000f) / 1000f;
		}
		else
		{
			_speed = targetSpeed;
		}

		_animationBlend = Mathf.Lerp(_animationBlend, targetSpeed, Time.deltaTime * SpeedChangeRate);

		// normalise input direction
		Vector3 inputDirection = new Vector3(_input.move.x, 0.0f, _input.move.y).normalized;

		// note: Vector2's != operator uses approximation so is not floating point error prone, and is cheaper than magnitude
		// if there is a move input rotate player when the player is moving

		//if (_input.move != Vector2.zero) // deleted this line as don't want look to orbit the camera
		{
			_targetRotation = Mathf.Atan2(inputDirection.x, inputDirection.z) * Mathf.Rad2Deg + _mainCamera.transform.eulerAngles.y + (aimActive ? aimRifleYRotationAdd : 0f);

			float rotation = Mathf.SmoothDampAngle(transform.eulerAngles.y, _targetRotation, ref _rotationVelocity, RotationSmoothTime);

			// rotate to face input direction relative to camera position
			transform.rotation = Quaternion.Euler(0.0f, rotation, 0.0f);
		}

		Vector3 targetDirection = Quaternion.Euler(0.0f, _targetRotation, 0.0f) * Vector3.forward;

		// move the player
		_controller.Move(targetDirection.normalized * (_speed * Time.deltaTime) + new Vector3(0.0f, _verticalVelocity, 0.0f) * Time.deltaTime);

		// play sound (if not playing) depending on move speed
		if (_speed <= 0.1f )
        {
			if (!idleAudioIsPlaying)
            {
				playerAudio.Stop();
				playerAudio.clip = heartbeatNoise;
				playerAudio.loop = true;
				idleAudioIsPlaying = true;
				moveAudioIsPlaying = false;
				playerAudio.Play();
            }
        }

		if (_speed >0.1f)
        {
			if (!moveAudioIsPlaying)
            {
				idleAudioIsPlaying = false;
				moveAudioIsPlaying = true;
				playerAudio.Stop();
				playerAudio.clip = walkingNoise;
				playerAudio.loop = true;
				moveAudioIsPlaying = true;
				playerAudio.Play();
			}
		}

		// update animator if using character
		if (_hasAnimator)
		{
			_animator.SetFloat(_animIDSpeed, _animationBlend);
			_animator.SetFloat(_animIDMotionSpeed, inputMagnitude);
		}
	}

	private bool jumpAudioIsPlaying = false;

	private void JumpAndGravity()
	{
		if (Grounded)
		{
			// reset the fall timeout timer
			_fallTimeoutDelta = FallTimeout;

			

			// update animator if using character
			if (_hasAnimator)
			{
				_animator.SetBool(_animIDJump, false);
				_animator.SetBool(_animIDFreeFall, false);
				
				if (jumpAudioIsPlaying)
				{
					jumpAudioIsPlaying = false;
				}
			}

			// stop our velocity dropping infinitely when grounded
			if (_verticalVelocity < 0.0f)
			{
				_verticalVelocity = -2f;
			}

			// Jump
			if (_input.jump && _jumpTimeoutDelta <= 0.0f)
			{
				// the square root of H * -2 * G = how much velocity needed to reach desired height
				_verticalVelocity = Mathf.Sqrt(JumpHeight * -2f * Gravity);

				// update animator if using character
				if (_hasAnimator)
				{
					_animator.SetBool(_animIDJump, true);
					
					if (!jumpAudioIsPlaying)
					{
						playerAudio.PlayOneShot(jumpingNoise);
						jumpAudioIsPlaying = true;
					}
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

			//jumpAudioIsPlaying = false;

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


	private bool WeaponHeld = false;
	
	private void HasWeaponCheck()
	{
		// **** FOR TESTING- RIFLE IS PICKED UP/ENABLED VIA PCIKUP ONLY! ****
		//hasRifle = true;
		// **** FOR TESTING- RIFLE IS PICKED UP/ENABLED VIA PCIKUP ONLY! ****

		// should really change to EquipWeapon as action is to check and equip with weapon
		if (Grounded)
		{
			if (_input.hasGun && hasPistol && !WeaponHeld)
			{
				// update animator if using character
				if (_hasAnimator)
				{
					//gun.SetActive(true);  // to be added later
					_animator.SetBool(_animIDHasGun, true);
				}
			}
			else if (_input.hasRifle && hasRifle && !WeaponHeld)
			{
				// update animator if using character
				if (_hasAnimator)
				{
					rifle.SetActive(true);
					WeaponHeld = true;
					_animator.SetBool(_animIDHasRifle, true);
				}
			}
		}

		if (gamepad != null)
        {
			if (gamepad.buttonWest.wasReleasedThisFrame && !hasRifle)
            {
				// ok - button released, so was pressed before
				playerAudio.PlayOneShot(weaponNotFoundNoise);
			}
		}

		if (Input.GetKeyUp(KeyCode.Alpha1) && !hasRifle)
        {
			// ok - button released, so was pressed before
			playerAudio.PlayOneShot(weaponNotFoundNoise);
        }
	}

	Gamepad gamepad = Gamepad.current;

	// maybe later now - run out of time
	private void ShootGunCheck()
	{
		// actually a pistol
		if (Grounded)
		{
			if (_input.shootGun && hasPistol && WeaponHeld)
			{
				// update animator if using character
				if (_hasAnimator)
				{
					_animator.SetBool(_animIDShootGun, true);
					playerAudio.PlayOneShot(pistolFiredNoise);
				}
			}
		}
	}
	

	[SerializeField]
	[Header("Repeat Fire Delay Active")]
	[Tooltip("Is Repeat Fire delay currently active")]
	bool bShootDelayed = false;

	[SerializeField]
	[Range(0,2)]
	[Header("Repeat Fire Delay Period")]
	[Tooltip("Time before another shot can be fired!")]
	float repeatDelay = 0.5f;

	private void ShootRifleCheck()
	{
		if (gamepad != null)
        {
			if (Grounded)
			{
				if ((gamepad.rightTrigger.wasPressedThisFrame || Input.GetMouseButtonUp(0) || _input.shootRifle) && hasRifle && WeaponHeld && aimActive)
				{
					//Debug.Log("Right Trigger PRESSED!");

					// update animator if using character
					if (!bShootDelayed)
					{
						// ok to fire
						if (_hasAnimator)
						{
							_animator.SetTrigger("ShootingRifle");
							playerAudio.PlayOneShot(rifleFiredNoise);
							Shooting();
							_input.shootRifle = false;
							bShootDelayed = true;
							StartCoroutine("PreventMultiShots");
						}
					}
				}
			}
		}
        else
        {
			if (Grounded)
			{
				if ((Input.GetMouseButtonUp(0) || _input.shootRifle) && hasRifle && WeaponHeld && aimActive)
				{
					//Debug.Log("Right Trigger PRESSED!");

					// update animator if using character
					if (!bShootDelayed)
					{
						// ok to fire
						if (_hasAnimator)
						{
							_animator.SetTrigger("ShootingRifle");
							playerAudio.PlayOneShot(rifleFiredNoise);
							Shooting();
							_input.shootRifle = false;
							bShootDelayed = true;
							StartCoroutine("PreventMultiShots");
						}
					}
				}
			}
		}
	}

	IEnumerator PreventMultiShots() 
	{ 
		// re-enable firing after timeout
		yield return new WaitForSeconds(0.5f);
		bShootDelayed = false;
	}

	private void Shooting()
    {
		Shoot();
    }

	
	// hit type for effects persistence
	enum hitType {stationary, moving};

	private void Shoot()
	{
		Ray ray =  _mainCamera.GetComponent<Camera>().ScreenPointToRay(screenCentrePoint);
		
		Debug.DrawRay(aimFromHere.transform.position, ray.direction, Color.yellow);
		
		RaycastHit hit; // what the raycast hits
		ray = _mainCamera.GetComponent<Camera>().ScreenPointToRay(screenCentrePoint);

		// raycast from aiming point on Player to centre of screen reticule
		if (Physics.Raycast(aimFromHere.transform.position, ray.direction, out hit))
		{
			Debug.Log("Just hit: " + hit.transform.name);
			
			// check if an enemy
			if (hit.transform.gameObject != null)
			{
				GameObject hitObject = hit.transform.gameObject;

				if (hit.transform.gameObject.CompareTag("SpiderQueen") ||
					hit.transform.gameObject.CompareTag("Eyeball") ||
					hit.transform.gameObject.CompareTag("CardGuard"))
				{
					// find the script - a virtual function will add correct damage
					BaseEnemy baseEnemyScript = hitObject.GetComponentInChildren<BaseEnemy>();
					baseEnemyScript.AddDamage();
					AddHitEffect(hit, hitType.moving);
				}
                else
                {
					AddHitEffect(hit, hitType.stationary);
				}
			}
		}
    }

	private void AddHitEffect(RaycastHit hit, hitType effectDuration)
    {
		// add muzzle flash, and hit effect at the hit position (sort of)
		GameObject muzzleFlash = Instantiate(weaponEffects[0], aimFromHere.transform.position, Quaternion.Euler(Vector3.forward)); // Quaternion.identity); // muzzle flash
	    GameObject effect = Instantiate(weaponEffects[1], hit.point, Quaternion.identity); // hit on concrete for now
		
		switch (effectDuration)
        {
			case hitType.moving: Destroy(effect, 0.5f); break;
			case hitType.stationary: Destroy(effect, 5f); break;
			default: break;
		}
	
		Destroy(muzzleFlash, 0.25f);
		Debug.Log("Ray Hit Point: " +  hit.point);
    }


	private void AimRifleCheck()
	{
		if (Grounded)
		{
			if (_input.aimRifle || Input.GetMouseButtonDown(1))
			{
				// checks rifle selected button pressed, and weapon held (has been collected)
				if (hasRifle && WeaponHeld) 
				{
					// update animator if using character
					if (_hasAnimator)
					{
						if (!aimActive)
						{
							_animator.SetBool(_animIDAimRifle, true);
							aimActive = true;
							aimCamera.SetActive(true);
							TurnOnReticule();
							Debug.Log("RETICULE TURNED ON!");
							_input.aimRifle = false;

						}
						else
						{
							_animator.SetBool(_animIDAimRifle, false);
							aimActive = false;
							aimCamera.SetActive(false);
							TurnOffReticule();
							Debug.Log("RETICULE TURNED OFF NOW!");
							_input.aimRifle = false;
						}
					}
					return;
				}
			}
		}
	}

	// maybe later now - run out of time
	private void ThrowGrenadeCheck()
	{
		if (_input.throwGrenade && Grounded && hasGrenade && WeaponHeld)
		{
			// update animator if using character
			if (_hasAnimator)
			{
				_animator.SetBool(_animIDThrowGrenade, true);
			}
		}
	}

	// maybe later now - run out of time
	private void Dancing()
	{
		if (_input.dancing && Grounded)
		{
			// update animator if using character
			if (_hasAnimator)
			{
				_animator.SetBool(_animIDDancing, true);
			}
		}
	}

	// maybe later now - run out of time
	private void Talking()
	{
		if (_input.talking && Grounded)
		{
			// update animator if using character
			if (_hasAnimator)
			{
				_animator.SetBool(_animIDTalking, true);
			}
		}
	}

	private static float ClampAngle(float lfAngle, float lfMin, float lfMax)
	{
		if (lfAngle < -360f) lfAngle += 360f;
		if (lfAngle > 360f) lfAngle -= 360f;
		return Mathf.Clamp(lfAngle, lfMin, lfMax);
	}

	private void OnDrawGizmosSelected()
	{
		Color transparentGreen = new Color(0.0f, 1.0f, 0.0f, 0.35f);
		Color transparentRed = new Color(1.0f, 0.0f, 0.0f, 0.35f);

		if (Grounded) Gizmos.color = transparentGreen;
		else Gizmos.color = transparentRed;

		// when selected, draw a gizmo in the position of, and matching radius of, the grounded collider
		Gizmos.DrawSphere(new Vector3(transform.position.x, transform.position.y - GroundedOffset, transform.position.z), GroundedRadius);
	}

	GameObject player;

	[Header("First Aid Kit Sound")]
	[Tooltip("sound used for first aid kit pickup")]
	[SerializeField]
	private AudioClip firstAidPickupNoise;

	private void OnTriggerEnter(Collider other)
	{
		FirstAid firstAidScript = other.gameObject.GetComponent<FirstAid>();

		// check who entered
		if (other.gameObject.CompareTag("FirstAidKit") && !firstAidScript.pickedUp)
		{
			firstAidScript.pickedUp = true;
			
			GameManagement.Instance.HealthKitsNumber = GameManagement.Instance.HealthKitsNumber + 1;
			GameManagement.Instance.PostMinorDisplayMessage("You Collected: Health Kit");
			playerAudio.PlayOneShot(firstAidPickupNoise);
			Destroy(other.gameObject,1f);
		}
	}
}