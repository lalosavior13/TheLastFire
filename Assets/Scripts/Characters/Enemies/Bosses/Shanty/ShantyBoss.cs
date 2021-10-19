using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Voidless;

namespace Flamingo
{
[RequireComponent(typeof(Skeleton))]
[RequireComponent(typeof(JumpAbility))]
[RequireComponent(typeof(RigidbodyMovementAbility))]
public class ShantyBoss : Boss
{
	public const int ID_ANIMATIONEVENT_PICKBOMB = 0;
	public const int ID_ANIMATIONEVENT_THROWBOMB = 1; 					/// <summary>Throw Bomb's Animation Event's ID.</summary>
	public const int ID_ANIMATIONEVENT_SWORD_UNSHEATH = 2; 				/// <summary>Sword Un-Sheath's Animation Event's ID.</summary>
	public const int ID_ANIMATIONEVENT_SWORD_SHEATH = 3; 				/// <summary>Sword Sheath's Animation Event's ID.</summary>
	public const int ID_ANIMATIONEVENT_GOIDLE = 4; 						/// <summary>Go Idle's Animation Event's ID.</summary>
	public const int ID_ANIMATIONEVENT_PICKTNT = 5; 					/// <summary>Pick TNT's Animation Event's ID.</summary>
	public const int ID_ANIMATIONEVENT_THROWTNT = 6; 					/// <summary>Throw TNT's Animation Event's ID.</summary>
	public const int ID_ANIMATIONEVENT_REPELBOMB = 7; 					/// <summary>Repel Bomb's Animation Event's ID.</summary>
	public const int ID_ANIMATIONSTATE_INTRO = 0; 						/// <summary>Intro's State ID [for AnimatorController].</summary>
	public const int ID_ANIMATIONSTATE_TIED = 1; 						/// <summary>Intro's State ID [for AnimatorController].</summary>
	public const int ID_ANIMATIONSTATE_IDLE = 2; 						/// <summary>Idle's State ID [for AnimatorController].</summary>
	public const int ID_ANIMATIONSTATE_ATTACK = 3; 						/// <summary>Attack's State ID [for AnimatorController].</summary>
	public const int ID_ANIMATIONSTATE_DAMAGE = 4; 						/// <summary>Damage's State ID [for AnimatorController].</summary>
	public const int ID_ATTACK_SHOOT_AIR = 0; 							/// <summary>Shoot's State ID [for AnimatorController].</summary>
	public const int ID_ATTACL_HIT_TENNIS = 1; 							/// <summary>Tennis Hit's State ID [for AnimatorController].</summary>
	public const int ID_ATTACK_BOMB_THROW = 2; 							/// <summary>Bomb Throw's State ID [for AnimatorController].</summary>
	public const int ID_ATTACK_BARREL_THROW = 3; 						/// <summary>Barrel Throw's State ID [for AnimatorController].</summary>
	public const int ID_DAMAGE_BOMB = 0; 								/// <summary>Bomb Damage's State ID [for AnimatorController].</summary>
	public const int ID_DAMAGE_SWORD = 1; 								/// <summary>Sword Damage's State ID [for AnimatorController].</summary>
	public const int ID_DAMAGE_CRY = 2; 								/// <summary>Cry's State ID [for AnimatorController].</summary>
	public const int ID_DAMAGE_BARREL = 3; 								/// <summary>Barrel Damage's State ID [for AnimatorController].</summary>
	public const int ID_IDLE = 0; 										/// <summary>Intro's State ID [for AnimatorController].</summary>
	public const int ID_LAUGH = 1; 										/// <summary>Intro's State ID [for AnimatorController].</summary>
	public const int ID_TAUNT = 2; 										/// <summary>Intro's State ID [for AnimatorController].</summary>

	[Space(10f)]
	[Header("Shanty's Attributes:")]
	[Space(5f)]
	[SerializeField] private ShantyShip _ship; 							/// <summary>Shanty's Ship.</summary>
	[Space(5f)]
	[Header("Weapons' Atrributes:")]
	[SerializeField] private ContactWeapon _sword; 						/// <summary>Shanty's Sword.</summary>
	[SerializeField] private Transform _falseSword; 					/// <summary>False Sword's Reference (the one stuck to the rigging).</summary>
	[Space(5f)]
	[SerializeField]
	[Range(0.0f, 1.0f)] private float _windowPercentage; 				/// <summary>Time-window before swinging sword (to hit bomb).</summary>
	[Space(5f)]
	[Header("Bomb's Attributes:")]
	[SerializeField] private CollectionIndex _bombIndex; 				/// <summary>Bomb's Index.</summary>
	[SerializeField] private float _bombProjectionTime; 				/// <summary>Bomb's Projection Time.</summary>
	[SerializeField]
	[Range(0.0f, 1.0f)] private float _bombProjectionPercentage; 		/// <summary>Bomb Projection Time's Percentage.</summary>
	[Space(5f)]
	[Header("TNT's Attributes:")]
	[SerializeField] private CollectionIndex _TNTIndex; 				/// <summary>TNT's Index.</summary>
	[SerializeField] private float _TNTProjectionTime; 					/// <summary>TNT's Projection Time.</summary>
	[SerializeField]
	[Range(0.0f, 1.0f)] private float _TNTProjectionPercentage; 		/// <summary>TNT Projection Time's Percentage.</summary>
	[SerializeField]
	[Range(0.0f, 1.0f)] private float _TNTTimeScaleChangeProgress; 		/// <summary>TNT parabolas' progress necessary to slow down time.</summary>
	[SerializeField]
	[Range(0.0f, 1.0f)] private float _TNTThrowTimeScale; 				/// <summary>Time Scale when throwing TNT.</summary>
	[Space(5f)]
	[SerializeField]
	[Range(0.0f, 1.0f)] private float _stage1HealthPercentageLimit; 	/// <summary>Health Limit's Percentage for TNT.</summary>
	[SerializeField]
	[Range(0.0f, 1.0f)] private float _stage2HealthPercentageLimit; 	/// <summary>Health Limit's Percentage for TNT.</summary>
	[Space(5f)]
	[Header("Whack-A-Mole's Attributes:")]
	[SerializeField] private float _vectorPairInterpolationDuration; 	/// <summary>Interpolation duration for Whack-A-Mole's Waypoints.</summary>
	[Space(5f)]
	[Header("Duel's Attributes:")]
	[SerializeField] private float _movementSpeed; 						/// <summary>Movement's Speed.</summary>
	[SerializeField] private float _rotationSpeed; 						/// <summary>Rotation's Speed.</summary>
	[SerializeField] private float _attackDistance; 					/// <summary>Attack's Distance.</summary>
	[Space(5f)]
	[Header("Inmunities:")]
	[SerializeField] private GameObjectTag[] _stage1Inmunities; 		/// <summary>Inmunities on Stage 1.</summary>
	[SerializeField] private GameObjectTag[] _stage2Inmunities; 		/// <summary>Inmunities on Stage 2.</summary>
	[Space(5f)]
	[Header("Animator's Attributes:")]
	[SerializeField] private AnimatorCredential _stateIDCredential; 	/// <summary>State ID's AnimatorCredential.</summary>
	[SerializeField] private AnimatorCredential _attackIDCredential; 	/// <summary>Attack ID's AnimatorCredential.</summary>
	[SerializeField] private AnimatorCredential _idleIDCredential; 		/// <summary>Idle ID's AnimatorCredential.</summary>
	[SerializeField] private AnimatorCredential _vitalityIDCredential; 	/// <summary>Vitality ID's AnimatorCredential.</summary>
	[Space(5f)]
	[Header("Animations:")]
	[SerializeField] private AnimationClip[] tiedAnimations; 			/// <summary>Tied Animations.</summary>
	[SerializeField] private AnimationClip untiedAnnimation; 			/// <summary>Untied's Animation.</summary>
	[SerializeField] private AnimationClip idleAnimation; 				/// <summary>Idle's Animation.</summary>
	[SerializeField] private AnimationClip laughAnimation; 				/// <summary>Laugh's Animation.</summary>
	[SerializeField] private AnimationClip tauntAnimation; 				/// <summary>Taun's Animation.</summary>
	[SerializeField] private AnimationClip tiredAnimation; 				/// <summary>Tired's Animation.</summary>
	[SerializeField] private AnimationClip shootAnimation; 				/// <summary>Shoot's Animation.</summary>
	[SerializeField] private AnimationClip throwBarrelAnimation; 		/// <summary>Throw Barrel's Animation.</summary>
	[SerializeField] private AnimationClip throwBombAnimation; 			/// <summary>Throw Bomb's Animation.</summary>
	[SerializeField] private AnimationClip tenninsHitAnimation; 		/// <summary>tennis Hit's Animation.</summary>
	[SerializeField] private AnimationClip hitBombAnimation; 			/// <summary>Hit Bomb's Animation.</summary>
	[SerializeField] private AnimationClip hitBarrelAnimation; 			/// <summary>Hit Barrel's Animation.</summary>
	[SerializeField] private AnimationClip hitSwordAnimation; 			/// <summary>Hit Sword's Animation.</summary>
	[SerializeField] private AnimationClip cryAnimation; 				/// <summary>Cry's Animation.</summary>
	[SerializeField] private AnimationClip normalAttackAnimation; 		/// <summary>Normal Attack's Animation.</summary>
	[SerializeField] private AnimationClip strongAttackAnimation; 		/// <summary>Strong Attack's Animation.</summary>
	[SerializeField] private AnimationClip backStepAnimation; 			/// <summary>Back-Setp Animation.</summary>
	private Skeleton _skeleton; 										/// <summary>Skeleton's Component.</summary>
	private Coroutine coroutine; 										/// <summary>Coroutine's Reference.</summary>
	private Behavior attackBehavior; 									/// <summary>Attack's Behavior [it is behavior so it can be paused].</summary>
	private Projectile _bomb; 											/// <summary>Bomb's Reference.</summary>
	private Projectile _TNT; 											/// <summary>TNT's Reference.</summary>
	private JumpAbility _jumpAbility; 									/// <summary>JumpAbility's Component.</summary>
	private RigidbodyMovementAbility _movementAbility; 					/// <summary>MovementAbility's Component.</summary>

#region Getters/Setters:
	/// <summary>Gets and Sets ship property.</summary>
	public ShantyShip ship
	{
		get { return _ship; }
		set { _ship = value; }
	}

	/// <summary>Gets bombIndex property.</summary>
	public CollectionIndex bombIndex { get { return _bombIndex; } }

	/// <summary>Gets TNTIndex property.</summary>
	public CollectionIndex TNTIndex { get { return _TNTIndex; } }

	/// <summary>Gets bombProjectionTime property.</summary>
	public float bombProjectionTime { get { return _bombProjectionTime; } }

	/// <summary>Gets bombProjectionPercentage property.</summary>
	public float bombProjectionPercentage { get { return _bombProjectionPercentage; } }

	/// <summary>Gets TNTProjectionTime property.</summary>
	public float TNTProjectionTime { get { return _TNTProjectionTime; } }

	/// <summary>Gets TNTProjectionPercentage property.</summary>
	public float TNTProjectionPercentage { get { return _TNTProjectionPercentage; } }

	/// <summary>Gets TNTTimeScaleChangeProgress property.</summary>
	public float TNTTimeScaleChangeProgress { get { return _TNTTimeScaleChangeProgress; } }

	/// <summary>Gets TNTThrowTimeScale property.</summary>
	public float TNTThrowTimeScale { get { return _TNTThrowTimeScale; } }

	/// <summary>Gets windowPercentage property.</summary>
	public float windowPercentage { get { return _windowPercentage; } }

	/// <summary>Gets stage1HealthPercentageLimit property.</summary>
	public float stage1HealthPercentageLimit { get { return _stage1HealthPercentageLimit; } }

	/// <summary>Gets stage2HealthPercentageLimit property.</summary>
	public float stage2HealthPercentageLimit { get { return _stage2HealthPercentageLimit; } }

	/// <summary>Gets vectorPairInterpolationDuration property.</summary>
	public float vectorPairInterpolationDuration { get { return _vectorPairInterpolationDuration; } }

	/// <summary>Gets movementSpeed property.</summary>
	public float movementSpeed { get { return _movementSpeed; } }

	/// <summary>Gets rotationSpeed property.</summary>
	public float rotationSpeed { get { return _rotationSpeed; } }

	/// <summary>Gets attackDistance property.</summary>
	public float attackDistance { get { return _attackDistance; } }

	/// <summary>Gets sword property.</summary>
	public ContactWeapon sword { get { return _sword; } }

	/// <summary>Gets falseSword property.</summary>
	public Transform falseSword { get { return _falseSword; } }

	/// <summary>Gets stage1Inmunities property.</summary>
	public GameObjectTag[] stage1Inmunities { get { return _stage1Inmunities; } }

	/// <summary>Gets stage2Inmunities property.</summary>
	public GameObjectTag[] stage2Inmunities { get { return _stage2Inmunities; } }

	/// <summary>Gets stateIDCredential property.</summary>
	public AnimatorCredential stateIDCredential { get { return _stateIDCredential; } }

	/// <summary>Gets attackIDCredential property.</summary>
	public AnimatorCredential attackIDCredential { get { return _attackIDCredential; } }

	/// <summary>Gets idleIDCredential property.</summary>
	public AnimatorCredential idleIDCredential { get { return _idleIDCredential; } }

	/// <summary>Gets vitalityIDCredential property.</summary>
	public AnimatorCredential vitalityIDCredential { get { return _vitalityIDCredential; } }

	/// <summary>Gets skeleton Component.</summary>
	public Skeleton skeleton
	{ 
		get
		{
			if(_skeleton == null) _skeleton = GetComponent<Skeleton>();
			return _skeleton;
		}
	}

	/// <summary>Gets and Sets bomb property.</summary>
	public Projectile bomb
	{
		get { return _bomb; }
		private set { _bomb = value; }
	}

	/// <summary>Gets and Sets TNT property.</summary>
	public Projectile TNT
	{
		get { return _TNT; }
		private set { _TNT = value; }
	}

	/// <summary>Gets jumpAbility Component.</summary>
	public JumpAbility jumpAbility
	{ 
		get
		{
			if(_jumpAbility == null) _jumpAbility = GetComponent<JumpAbility>();
			return _jumpAbility;
		}
	}

	/// <summary>Gets movementAbility Component.</summary>
	public RigidbodyMovementAbility movementAbility
	{ 
		get
		{
			if(_movementAbility == null) _movementAbility = GetComponent<RigidbodyMovementAbility>();
			return _movementAbility;
		}
	}
#endregion

	/// <summary>ShantyBoss's instance initialization.</summary>
	protected override void Awake()
	{
		ActivateSword(false);
		jumpAbility.gravityApplier.enabled = false;

		if(tiedAnimations != null) foreach(AnimationClip clip in tiedAnimations)
		{
			Debug.Log("[ShantyBoss] Adding Animation: " + clip.ToString());
			animation.AddClip(clip);
		}

		animation.AddClips(
			untiedAnnimation,
			idleAnimation,
			laughAnimation,
			tauntAnimation,
			tiredAnimation,
			shootAnimation,
			throwBarrelAnimation,
			throwBombAnimation,
			tenninsHitAnimation,
			hitBombAnimation,
			hitBarrelAnimation,
			hitSwordAnimation,
			cryAnimation
		);

		base.Awake();
	}

	/// <summary>ShantyBoss's starting actions before 1st Update frame.</summary>
	protected override void Start ()
	{
		base.Start();
	}

	/// <summary>Activates/Deactivates Sword and False Sword.</summary>
	/// <param name="_activate">Activate Sword? true by default.</param>
	private void ActivateSword(bool _activate = true)
	{
		sword.owner = gameObject;
		sword.ActivateHitBoxes(_activate);
		
		if(_activate)
		{
			sword.transform.position = skeleton.rightHand.position;
			sword.transform.rotation = skeleton.rightHand.rotation;
			sword.transform.parent = skeleton.rightHand;
		}

		sword.gameObject.SetActive(_activate);
		falseSword.gameObject.SetActive(!_activate);
	}

	/// <summary>Enables Physics.</summary>
	/// <param name="_enable">Enable? true by default.</param>
	public void EnablePhysics(bool _enable = true)
	{
		jumpAbility.gravityApplier.useGravity = _enable;
	}

	/// <summary>Begins Attack's Routine.</summary>
	public void BeginAttackRoutine()
	{
		Debug.Log("[ShantyBoss] Beginning Attack Routine at Stage: " + currentStage);
		this.RemoveStates(ID_STATE_IDLE);
		this.AddStates(ID_STATE_ATTACK);
		//animator.SetInteger(stateIDCredential, ID_ANIMATIONSTATE_ATTACK);

		switch(currentStage)
		{
			case STAGE_1:
			if(health.hpRatio <=  stage1HealthPercentageLimit)
				//BeginBombThrowingRoutine();
				BeginTNTThrowingRoutine();
			else
				//BeginTNTThrowingRoutine();
				BeginBombThrowingRoutine();
			break;

			case STAGE_2:
			this.StartCoroutine(WhackAMoleRoutine(), ref behaviorCoroutine);
			break;

			case STAGE_3:
			this.StartCoroutine(DuelRoutine(), ref behaviorCoroutine);
			this.StartCoroutine(RotateTowardsMateo(), ref coroutine);
			break;
		}		
	}

	/// <summary>Moves Shanty into desired direction.</summary>
	/// <param name="_axes">Direction's Axis.</param>
	public void Move(Vector2 _axes)
	{
		/* Steps here:
			- Call the movement's animation.
			- Displace Argel.
		*/
		
	}

#region BombThrowingRoutines:
	/// <summary>Begins the Bomb Throwing Animations.</summary>
	public void BeginBombThrowingRoutine()
	{
		//Debug.Log("[ShantyBoss] Beggining Bombing Routine...");
		ActivateSword(false);
		animation.Play(throwBombAnimation);
		animation.PlayQueued(idleAnimation.name);
		/// During the throw animation, a callback will be invoked that will then invoke ThrowBomb()
		//animator.SetInteger(stateIDCredential, ID_ANIMATIONSTATE_ATTACK);
		//animator.SetInteger(attackIDCredential, ID_ATTACK_BOMB_THROW);
	}

	/// <summary>Picks Bomb.</summary>
	public void PickBomb()
	{
		Debug.Log("[ShantyBoss] Picking up Bomb...");

		bomb = PoolManager.RequestParabolaProjectile(Faction.Enemy, bombIndex, skeleton.rightHand.position, Game.mateo.transform.position, bombProjectionTime, gameObject);
		bomb.activated = false;
		bomb.ActivateHitBoxes(false);
		bomb.transform.parent = skeleton.rightHand;
	}

	/// <summary>Throws Bomb [called after an specific frame of the Bom-Throwing Animation].</summary>
	public void ThrowBomb()
	{
		Debug.Log("[ShantyBoss] Throwing Bomb...");

		bomb.transform.parent = null;
		bomb.OnObjectDeactivation();
		bomb = PoolManager.RequestParabolaProjectile(Faction.Enemy, bombIndex, skeleton.rightHand.position, Game.mateo.transform.position, bombProjectionTime, gameObject);

		bomb.projectileEventsHandler.onProjectileEvent -= OnBombEvent;
		bomb.projectileEventsHandler.onProjectileEvent += OnBombEvent;
		bomb.projectileEventsHandler.onProjectileDeactivated -= OnBombDeactivated;
		bomb.projectileEventsHandler.onProjectileDeactivated += OnBombDeactivated;
		bomb.activated = true;
		bomb.ActivateHitBoxes(true);

		//animator.SetInteger(stateIDCredential, ID_ANIMATIONSTATE_IDLE);
	}
#endregion

#region TNTThrowingRoutines:
	/// <summary>Begins TNT ThrowingRoutine.</summary>
	public void BeginTNTThrowingRoutine()
	{
		//Debug.Log("[ShantyBoss] Beggining TNT Routine");
		/// During the throw animation, a callback will be invoked that will then invoke ThrowTNT()
		//animator.SetInteger(stateIDCredential, ID_ATTACK_BARREL_THROW);
		//animator.SetInteger(attackIDCredential, ID_ANIMATIONSTATE_ATTACK);
		ActivateSword(false);
		animation.Play(throwBarrelAnimation);
		animation.PlayQueued(idleAnimation.name);
	}

	/// <summary>Picks TNT.</summary>
	public void PickTNT()
	{
		//Debug.Log("[ShantyBoss] Picking TNT...");

		TNT = PoolManager.RequestParabolaProjectile(Faction.Enemy, TNTIndex, skeleton.rightHand.position, Game.mateo.transform.position, TNTProjectionTime, gameObject);
		TNT.activated = false;
		TNT.ActivateHitBoxes(false);
		TNT.transform.parent = skeleton.rightHand;

		BombParabolaProjectile TNTBomb = TNT as BombParabolaProjectile;
		TNTBomb.ChangeState(BombState.WickOn);
	}

	/// <summary>Throws TNT.</summary>
	public void ThrowTNT()
	{
		//Debug.Log("[ShantyBoss] Throwing TNT");

		TNT.transform.parent = null;
		TNT.OnObjectDeactivation();
		TNT = PoolManager.RequestParabolaProjectile(Faction.Enemy, TNTIndex, skeleton.rightHand.position, Game.mateo.transform.position, TNTProjectionTime, gameObject);
		TNT.ActivateHitBoxes(true);

		TNT.projectileEventsHandler.onProjectileEvent -= OnBombEvent;
		TNT.projectileEventsHandler.onProjectileEvent += OnBombEvent;
		TNT.projectileEventsHandler.onProjectileDeactivated -= OnBombDeactivated;
		TNT.projectileEventsHandler.onProjectileDeactivated += OnBombDeactivated;
		TNT.activated = true;

		this.StartCoroutine(TNTRoutine(), ref behaviorCoroutine);
		//animator.SetInteger(stateIDCredential, ID_ANIMATIONSTATE_IDLE);
	}
#endregion

#region Callbacks:
	/// <summary>Callback internally called when the Boss advances stage.</summary>
	protected override void OnStageChanged()
	{
		base.OnStageChanged();
		
		switch(currentStage)
		{
			case STAGE_1:
			EnablePhysics(false);
			health.inmunities = stage1Inmunities;
			break;

			case STAGE_2:
			EnablePhysics(false);
			health.inmunities = stage2Inmunities;
			break;

			case STAGE_3:
			EnablePhysics(true);
			break;
		}
	}

	/// <summary>Callback invoked when Shanty ought to be tied.</summary>
	/// <param name="_ship">Ship that will contain Shanty.</param>
	/// <param name="_tiePosition">Tie Position.</param>
	public void OnTie(Transform _ship, Vector3 _tiePosition)
	{
		Debug.Log("[ShantyBoss] Shanty is tied...");
		transform.position = _tiePosition;
		transform.parent = _ship;
		//animator.SetInteger(stateIDCredential, ID_ANIMATIONSTATE_TIED);
		this.StartCoroutine(TieRoutine(), ref behaviorCoroutine);

	}

	/// <summary>Callback invoked when Shaanty ought to be untied.</summary>
	public void OnUntie()
	{
		Debug.Log("[ShantyBoss] Shanty is untied...");
		this.DispatchCoroutine(ref behaviorCoroutine);

		this.StartCoroutine(animation.PlayAnimationAndWait(untiedAnnimation, PlayMode.StopSameLayer, 0.0f,
		()=>
		{
			this.AddStates(ID_STATE_ATTACK);
		}));
	}

	/// <summary>Callback invoked when new state's flags are added.</summary>
	/// <param name="_state">State's flags that were added.</param>
	public override void OnStatesAdded(int _state)
	{
		if((_state | ID_STATE_IDLE) == _state)
		{
			//animator.SetInteger(stateIDCredential, ID_ANIMATIONSTATE_IDLE);
			animation.Play(idleAnimation);

		} else if((_state | ID_STATE_ATTACK) == _state)
		{
			//animator.SetInteger(stateIDCredential, ID_ANIMATIONSTATE_ATTACK);
			BeginAttackRoutine();

		} else if((_state | ID_STATE_HURT) == _state)
		{
			//animator.SetInteger(stateIDCredential, ID_ANIMATIONSTATE_DAMAGE);
			this.RemoveStates(ID_STATE_ATTACK);
			/*this.StartCoroutine(animator.WaitForAnimatorState(0, 0.0f,
			()=>
			{
				Debug.Log("[ShantyBoss] Hurt Animation Ended...");
				this.AddStates(ID_STATE_ATTACK);
			}));*/
		}

		Debug.Log("[ShantyBoss] States: " + StatesToString());
	}

	/// <summary>Callback invoked when new state's flags are removed.</summary>
	/// <param name="_state">State's flags that were removed.</param>
	public override void OnStatesRemoved(int _state)
	{
		if((_state | ID_STATE_HURT) == _state) BeginAttackRoutine();
	}

	/// <summary>Callback invoked when an Animation Event is invoked.</summary>
	/// <param name="_ID">Int argument.</param>
	protected override void OnAnimationIntEvent(int _ID)
	{
		switch(_ID)
		{
			case ID_ANIMATIONEVENT_PICKBOMB:
			ActivateSword(false);
			PickBomb();
			break;

			case ID_ANIMATIONEVENT_THROWBOMB:
			ThrowBomb();
			break;

			case ID_ANIMATIONEVENT_SWORD_UNSHEATH:
			Debug.Log("[ShantyBoss] Time of Unsheath: " + Time.time);
			ActivateSword(true);
			//animator.SetInteger(stateIDCredential, ID_ANIMATIONSTATE_IDLE);
			break;

			case ID_ANIMATIONEVENT_SWORD_SHEATH:
			ActivateSword(false);
			break;

			case ID_ANIMATIONEVENT_GOIDLE:
			//animator.SetInteger(stateIDCredential, ID_ANIMATIONSTATE_IDLE);
			animation.Play(idleAnimation);
			break;

			case ID_ANIMATIONEVENT_PICKTNT:
			PickTNT();
			break;

			case ID_ANIMATIONEVENT_THROWTNT:
			ThrowTNT();
			break;

			case ID_ANIMATIONEVENT_REPELBOMB:
			if(bomb != null) bomb.RequestRepel(gameObject);
			break;

			case 10:
			Debug.Log("[ShantyBoss] Time when animation starts: " + Time.time);
			break;
		}

		Debug.Log("[ShantyBoss] Invoked AnimationEvent with ID: " + _ID);
	}

	/// <summary>Callback invoked when a Bomb Event occurs.</summary>
	/// <param name="_projecile">Bomb's reference.</param>
	/// <param name="_eventID">Bomb's Event ID.</param>
	/// <param name="_info">Additional Trigger2DInformation.</param>
	private void OnBombEvent(Projectile _projectile, int _eventID, Trigger2DInformation _info)
	{
		switch(_eventID)
		{
			case Projectile.ID_EVENT_REPELLED:
			Debug.Log("[ShantyBoss] Bomb Repelled...");
			/*Projectile projectile = _info.collider.GetComponentInParent<Projectile>();
			
			if(projectile == null) return;*/

			//if(_projectile.owner == null || _projectile.owner == gameObject) return;
			Debug.Log("[ShantyBoss] Ought to repel...");
			float durationBeforeSwordSwing = _projectile.parabolaTime * windowPercentage;

			/*this.StartCoroutine(this.WaitSeconds(durationBeforeSwordSwing, 
			()=>
			{
				animation.Play(tenninsHitAnimation);
				//animator.SetInteger(stateIDCredential, ID_ANIMATIONSTATE_ATTACK);
				//animator.SetInteger(attackIDCredential, ID_ATTACL_HIT_TENNIS);
			}));*/
			if(_projectile.owner == gameObject) return;

			ActivateSword(false);
			animation.Stop();
			this.StartCoroutine(animation.PlayAndSincronizeAnimationWithTime(tenninsHitAnimation, 14, _projectile.parabolaTime * bombProjectionPercentage), ref behaviorCoroutine);
			break;
		}
	}

	/// <summary>Event invoked when the projectile is deactivated.</summary>
	/// <param name="_projectile">Projectile that invoked the event.</param>
	/// <param name="_cause">Cause of the deactivation.</param>
	/// <param name="_info">Additional Trigger2D's information.</param>
	private void OnBombDeactivated(Projectile _projectile, DeactivationCause _cause, Trigger2DInformation _info)
	{
		switch(_cause)
		{
			case DeactivationCause.Impacted:
			BeginAttackRoutine();
			break;

			case DeactivationCause.Destroyed:
			/*if(_info.collider.gameObject != gameObject || !this.HasStates(ID_STATE_HURT))
			{
				BeginAttackRoutine();
			}*/
			BeginAttackRoutine();
			break;
		}

		Debug.Log(
			"[ShantyBoss] Bomb deactivation event: "
			+ _cause.ToString()
			+ _info.ToString()
		);
	}

	/// <summary>Callback invoked when a TNT Event occurs.</summary>
	/// <param name="_projecile">TNT's reference.</param>
	/// <param name="_eventID">TNT's Event ID.</param>
	/// <param name="_info">Additional Trigger2DInformation.</param>
	private void OnTNTEvent(Projectile _projectile, int _eventID, Trigger2DInformation _info)
	{
		switch(_eventID)
		{
			case Projectile.ID_EVENT_REPELLED:
			Debug.Log("[ShantyBoss] TNT Repelled...");
			/*Projectile projectile = _info.collider.GetComponentInParent<Projectile>();
			
			if(projectile == null) return;*/

			if(_projectile.owner == null || _projectile.owner == gameObject) return;

			float durationBeforeSwordSwing = _projectile.parabolaTime * windowPercentage;

			this.StartCoroutine(this.WaitSeconds(durationBeforeSwordSwing, 
			()=>
			{
				Debug.Log("[ShantyBoss] Swing!!");
				//animator.SetInteger(stateIDCredential, ID_ANIMATIONSTATE_ATTACK);
				//animator.SetInteger(attackIDCredential, ID_ATTACL_HIT_TENNIS);
				//animation.Play(tenninsHitAnimation);
			}));
			break;
		}
	}

	/// <summary>Callback invoked when a Health's event has occured.</summary>
	/// <param name="_event">Type of Health Event.</param>
	/// <param name="_amount">Amount of health that changed [0.0f by default].</param>
	/// <param name="_object">GameObject that caused the event, null be default.</param>
	protected override void OnHealthEvent(HealthEvent _event, float _amount = 0.0f, GameObject _object = null)
	{
		base.OnHealthEvent(_event, _amount, _object);

		switch(_event)
		{
			case HealthEvent.Depleted:
			//animator.SetInteger(stateIDCredential, ID_ANIMATIONSTATE_DAMAGE);

			if(_object == null) return;

			int damageID = 0;
			AnimationClip damageClip = null;

			if(_object.CompareTag(Game.data.playerWeaponTag))
			{
				damageID = ID_DAMAGE_SWORD;
				damageClip = hitSwordAnimation;

			} else if(_object.CompareTag(Game.data.playerProjectileTag))
			{
				damageID = ID_DAMAGE_BOMB;
				damageClip = hitBombAnimation;

			} else if(_object.CompareTag(Game.data.explodableTag))
			{
				damageID = ID_DAMAGE_BARREL;
				damageClip = hitBarrelAnimation;
			}

			//animator.SetInteger(vitalityIDCredential, damageID);
			animation.Play(damageClip);
			this.RemoveStates(ID_STATE_ATTACK);
			this.AddStates(ID_STATE_HURT);
			break;

			case HealthEvent.FullyDepleted:
			//animator.SetInteger(stateIDCredential, ID_ANIMATIONSTATE_DAMAGE);
			//animator.SetInteger(vitalityIDCredential, ID_DAMAGE_CRY);

			if(currentStage >= stages)
			{
				eventsHandler.InvokeIDEvent(ID_EVENT_BOSS_DEATHROUTINE_BEGINS);
			}
			break;

			case HealthEvent.HitStunEnds:
			this.RemoveStates(ID_STATE_HURT);
			this.ReturnToPreviousState();
			break;
		}
	}
#endregion

	/// <summary>Tie's Coroutine.</summary>
	private IEnumerator TieRoutine()
	{
		SecondsDelayWait wait = new SecondsDelayWait(0.0f);
		AnimationState animationState = null;

		while(true)
		{
			foreach(AnimationClip clip in tiedAnimations)
			{
				animation.Play(clip);
				animationState = animation.GetAnimationState(clip);
				wait.ChangeDurationAndReset(animationState.clip.length);

				while(wait.MoveNext()) yield return null;
			}
		}
	}

	/// <summary>TNT's Routine when it is thrown.</summary>
	private IEnumerator TNTRoutine()
	{
		float waitDuration = TNTProjectionTime * TNTTimeScaleChangeProgress;
		float slowDownDuration = TNTProjectionTime - waitDuration;

		SecondsDelayWait wait = new SecondsDelayWait(waitDuration);

		while(wait.MoveNext()) yield return null;

		Time.timeScale = TNTThrowTimeScale;
		wait.ChangeDurationAndReset(slowDownDuration);

		while(wait.MoveNext()) yield return null;

		Time.timeScale = 1.0f;
	}

	/// <summary>Whack-A-Mole's Routine.</summary>
	private IEnumerator WhackAMoleRoutine()
	{
		SecondsDelayWait wait = new SecondsDelayWait(0.0f);
		Vector3Pair[] waypointsPairs = ShantySceneController.Instance.whackAMoleWaypointsPairs.Randomized();
		AnimationClip clip = null;
		AnimationState animationState = null;
		float t = 0.0f;
		float inverseDuration = 1.0f / vectorPairInterpolationDuration;
		Vector3 a = Vector3.zero;
		Vector3 b = Vector3.zero;

		EnableHurtBoxes(false);

		while(true)
		{
			foreach(Vector3Pair pair in waypointsPairs)
			{
				t = 0.0f;
				a = ship.transform.TransformPoint(pair.a);
				b = ship.transform.TransformPoint(pair.b);

				while(t < 1.0f)
				{
					transform.position = Vector3.Lerp(a, b, t);

					t += (Time.deltaTime * inverseDuration);
					yield return null;
				}

				transform.position = b;
				EnableHurtBoxes(true);
				animation.Play(throwBombAnimation);
				animationState = animation.GetAnimationState(throwBombAnimation);
				wait.ChangeDurationAndReset(animationState.length);

				while(wait.MoveNext()) yield return null;

				t = 0.0f;

				while(t < 1.0f)
				{
					transform.position = Vector3.Lerp(b, a, t);

					t += (Time.deltaTime * inverseDuration);
					yield return null;
				}

				transform.position = a;
				EnableHurtBoxes(false);
			}

			waypointsPairs = waypointsPairs.Randomized();
		}

		yield return null;
	}

	/// <summary>Duel's Routine.</summary>
	private IEnumerator DuelRoutine()
	{
		AnimationClip clip = null;
		AnimationState animationState = null;
		SecondsDelayWait wait = new SecondsDelayWait(0.0f);
		Vector3 direction = Vector3.zero;
		float sqrDistance = attackDistance * attackDistance;

		while(true)
		{
			direction = Game.mateo.transform.position - transform.position;

			if(direction.sqrMagnitude <= sqrDistance)
			{
				animation.Play(normalAttackAnimation);
				animationState = animation.GetAnimationState(normalAttackAnimation);

				wait.ChangeDurationAndReset(normalAttackAnimation.length);

				while(wait.MoveNext()) yield return null; 
			}

			yield return null;
		}
	}

	/// <summary>Rotate Towards Mateo's Routine.</summary>
	private IEnumerator RotateTowardsMateo()
	{
		float x = 0.0f;
		Vector3 direction = Vector3.zero;

		while(true)
		{
			x = Game.mateo.transform.position.x - transform.position.x;
			direction = x > 0.0f ? Vector3.right : Vector3.left;

			transform.rotation = Quaternion.RotateTowards(transform.rotation, Quaternion.LookRotation(direction), rotationSpeed * Time.deltaTime);

			yield return null;
		}
	}

	/// <summary>Front Attack's routine.</summary>
	private IEnumerator FrontAttackRoutine()
	{
		AnimationState animationState = null;
		SecondsDelayWait wait = new SecondsDelayWait(0.0f);

		animation.Play(normalAttackAnimation);
		animationState = animation.GetAnimationState(normalAttackAnimation);
		
		wait.ChangeDurationAndReset(animationState.length);
		
		while(wait.MoveNext())
		{
			/// Displace while doing the attack....
			movementAbility.Move(Vector3.left);
			yield return null;
		}

		yield return null;
	}

	/// <summary>Strong Attack's Routine.</summary>
	private IEnumerator StrongAttackRoutine()
	{
		SecondsDelayWait wait = new SecondsDelayWait(0.0f);
		AnimationState animationState = null;
		float duration = 0.0f;
		float t = 0.0f;

		animation.Play(strongAttackAnimation);
		animationState = animation.GetAnimationState(strongAttackAnimation);

		while(!jumpAbility.HasStates(JumpAbility.STATE_ID_FALLING))
		{
			jumpAbility.Jump(Vector3.up);
			t += Time.deltaTime;
			yield return null;
		}

		duration = Mathf.Max((animationState.length - t), 0.0f);
		wait.ChangeDurationAndReset(duration);

		while(wait.MoveNext()) yield return null;
	}
}
}