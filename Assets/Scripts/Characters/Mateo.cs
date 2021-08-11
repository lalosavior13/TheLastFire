using System;
using System.Text;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Voidless;

public enum StareTarget
{
	Boss,
	Player
}

namespace Flamingo
{
[RequireComponent(typeof(RigidbodyMovementAbility))]
[RequireComponent(typeof(JumpAbility))]
[RequireComponent(typeof(DashAbility))]
[RequireComponent(typeof(RotationAbility))]
[RequireComponent(typeof(ShootChargedProjectile))]
[RequireComponent(typeof(TransformDeltaCalculator))]
[RequireComponent(typeof(SensorSystem2D))]
[RequireComponent(typeof(WallEvaluator))]
[RequireComponent(typeof(AnimationAttacksHandler))]
[RequireComponent(typeof(SlopeEvaluator))]
[RequireComponent(typeof(VCameraTarget))]
public class Mateo : Character
{
	public const int ID_STATE_INITIALPOSE = 1 << 4; 							/// <summary>Initial Pose's State ID.</summary>
	public const int ID_INITIALPOSE_STARRINGATPLAYER = 0; 						/// <summary>Starring At Player's Initial Pose's ID.</summary>
	public const int ID_INITIALPOSE_MEDITATING = 1; 							/// <summary>Meditation Initial Pose's ID.</summary>
	public const int ID_STATE_MEDITATING = 1 << 4; 								/// <summary>Meditating's State ID.</summary>
	public const int ID_EVENT_INITIALPOSE_ENDED = 0; 							/// <summary>Mateo Initial-Pose-Finished's Event ID.</summary>

	[Header("Rotations:")]
	[SerializeField] private EulerRotation _stareAtBossRotation; 				/// <summary>Stare at Boss's Rotation.</summary>
	[SerializeField] private EulerRotation _stareAtPlayerRotation; 				/// <summary>SSStare At Player's Rotation.</summary>
	[Header("Hands:")]
	[SerializeField] private Transform _leftHand; 								/// <summary>Left Hand's reference [Fire caster].</summary>
	[SerializeField] private Transform _rightHand; 								/// <summary>Right Hand's reference [Sword holder].</summary>
	[Space(5f)]
	[SerializeField] private float _postMeditationDuration; 					/// <summary>Post-Meditation's Duration.</summary>
	[Space(5f)]
	[Header("Sword's Attributes:")]
	[SerializeField] private Sword _sword; 										/// <summary>Mateo's Sword.</summary>
	[SerializeField] private int _groundedNeutralComboIndex; 					/// <summary>Grounded Neutral Combo's index on the AnimationAttacksHandler.</summary>
	[SerializeField] private int _groundedDirectionalComboIndex; 				/// <summary>Grounded Directional Combo's index on the AnimationAttacksHandler.</summary>
	[SerializeField] private int _airNeutralComboIndex; 						/// <summary>Air Neutral Combo's index on the AnimationAttacksHandler.</summary>
	[SerializeField] private int _airDirectionalComboIndex; 					/// <summary>Air Directional Combo's index on the AnimationAttacksHandler.</summary>
	[SerializeField]
	[Range(0.0f, 1.0f)] private float _animationStateProgress; 					/// <summary>Minimum Animation State Progress to open the next attack's window.</summary>
	[SerializeField] private float _attackDurationWindow; 						/// <summary>Duration Window's for next sword attack.</summary>
	[Space(5f)]
	[SerializeField] private FloatRange _directionalThresholdX; 				/// <summary>Directional Threhold on the X's Axis to perform directional attacks.</summary>
	[SerializeField] private FloatRange _directionalThresholdY; 				/// <summary>Directional Threhold on the Y's Axis to perform directional attacks.</summary>
	[Space(5f)]
	[SerializeField] private float _gravityScale; 								/// <summary>Gravity Scale applied when attacking.</summary>
	[SerializeField] private int _scaleChangePriority; 							/// <summary>Gravity Scale's Change Priority.</summary>
	[Space(5f)]
	[SerializeField] private float _jumpingMovementScale; 						/// <summary>Movement's Scale when Mateo is Jumping.</summary>
	[Space(5f)]
	[SerializeField]
	[Range(0.0f, 1.0f)] private float _dashXThreshold; 							/// <summary>Minimum left axis' X [absolute] value to be able to perform dash.</summary>
	[Space(5f)]
	[Header("Animator's Attributes:")]
	[SerializeField] private Transform _animatorParent; 						/// <summary>Animator's Parent.</summary>
	[SerializeField] private Animator _animator; 								/// <summary>Animator's Component.</summary>
	[SerializeField] private AnimatorCredential _initialPoseCredential; 		/// <summary>Initial Pose's Credential.</summary>
	[SerializeField] private AnimatorCredential _initialPoseIDCredential; 		/// <summary>Initial Pose Id's Credential.</summary>
	[SerializeField] private AnimatorCredential _vitalityIDCredential; 			/// <summary>Vitality ID's Animator Credential.</summary>
	[SerializeField] private AnimatorCredential _leftAxisXCredential; 			/// <summary>Left Axis X's Animator Credential.</summary>
	[SerializeField] private AnimatorCredential _leftAxisYCredential; 			/// <summary>Left Axis Y's Animator Credential.</summary>
	[SerializeField] private AnimatorCredential _rightAxisXCredential; 			/// <summary>Right Axis X's Animator Credential.</summary>
	[SerializeField] private AnimatorCredential _rightAxisYCredential; 			/// <summary>Right Axis Y's Animator Credential.</summary>
	[SerializeField] private AnimatorCredential _jumpStateIDCredential; 		/// <summary>Jump State ID's Animator Credential.</summary>
	[SerializeField] private AnimatorCredential _jumpingIDCredential; 			/// <summary>Jumping ID's Animator Credential.</summary>
	[SerializeField] private AnimatorCredential _groundedCredential; 			/// <summary>Grounded's Animator Credential.</summary>
	[SerializeField] private AnimatorCredential _attackIDCredential; 			/// <summary>Attack ID's Animator Credential.</summary>
	[SerializeField] private AnimatorCredential _attackCredential; 				/// <summary>Attack's Animator Credential.</summary>
	[SerializeField] private AnimatorCredential _shootingStateIDCredential; 	/// <summary>Shooting State ID's Animator Credential.</summary>
	[SerializeField] private AnimatorCredential _dashingCredential; 			/// <summary>Dashing's Animator Credential.</summary>
	[SerializeField] private AnimatorCredential _brakingCredential; 			/// <summary>Brakin'g Animator Credential.</summary>
	[SerializeField] private AnimatorCredential _walledCredential; 				/// <summary>Walled's Animator Credential.</summary>
	[SerializeField] private AnimatorCredential _impactedWithWallCredential; 	/// <summary>Impacted w/ Wall's Animator Credential.</summary>
	[Space(5f)]
	[SerializeField] private TrailRenderer _extraJumpTrailRenderer; 			/// <summary>Extra-Jump's Trail Renderer.</summary>
	private RigidbodyMovementAbility _movementAbility; 							/// <summary>RigidbodyMovementAbility's Component.</summary>
	private RotationAbility _rotationAbility; 									/// <summary>RotationAbility's Component.</summary>
	private JumpAbility _jumpAbility; 											/// <summary>JumpAbility's Component.</summary>
	private ShootChargedProjectile _shootProjectile; 							/// <summary>ShootChargedProjectile's Component.</summary>
	private DashAbility _dashAbility; 											/// <summary>DashAbility's Component.</summary>
	private TransformDeltaCalculator _deltaCalculator; 							/// <summary>TransformDeltaCalculator's Component.</summary>
	private SensorSystem2D _sensorSystem; 										/// <summary>SensorSystem2D's Component.</summary>
	private WallEvaluator _wallEvaluator; 										/// <summary>WallEvaluator's Component.</summary>
	private AnimationAttacksHandler _attacksHandler; 							/// <summary>AnimationAttacksHandler's Component.</summary>
	private SlopeEvaluator _slopeEvaluator; 									/// <summary>SlopeEvaluator's Component.</summary>
	private VCameraTarget _cameraTarget; 										/// <summary>VCameraTarget's Component.</summary>
	private Vector3 _orientation; 												/// <summary>Mateo's Orientation.</summary>
	private Vector2 _leftAxes; 													/// <summary>Left Axes' Value.</summary>
	private Cooldown _postInitialPoseCooldown; 									/// <summary>Post-Meditation's Cooldown.</summary>

#region Getters/Setters:
	/// <summary>Gets stareAtBossRotation property.</summary>
	public EulerRotation stareAtBossRotation { get { return _stareAtBossRotation; } }

	/// <summary>Gets stareAtPlayerRotation property.</summary>
	public EulerRotation stareAtPlayerRotation { get { return _stareAtPlayerRotation; } }

	/// <summary>Gets leftHand property.</summary>
	public Transform leftHand { get { return _leftHand; } }

	/// <summary>Gets rightHand property.</summary>
	public Transform rightHand { get { return _rightHand; } }

	/// <summary>Gets animatorParent property.</summary>
	public Transform animatorParent { get { return _animatorParent; } }

	/// <summary>Gets sword property.</summary>
	public Sword sword { get { return _sword; } }

	/// <summary>Gets postMeditationDuration property.</summary>
	public float postMeditationDuration { get { return _postMeditationDuration; } }

	/// <summary>Gets animationStateProgress property.</summary>
	public float animationStateProgress { get { return _animationStateProgress; } }

	/// <summary>Gets attackDurationWindow property.</summary>
	public float attackDurationWindow { get { return _attackDurationWindow; } }

	/// <summary>Gets gravityScale property.</summary>
	public float gravityScale { get { return _gravityScale; } }

	/// <summary>Gets dashXThreshold property.</summary>
	public float dashXThreshold { get { return _dashXThreshold; } }

	/// <summary>Gets jumpingMovementScale property.</summary>
	public float jumpingMovementScale { get { return _jumpingMovementScale; } }

	/// <summary>Gets directionalThresholdX property.</summary>
	public FloatRange directionalThresholdX { get { return _directionalThresholdX; } }

	/// <summary>Gets directionalThresholdY property.</summary>
	public FloatRange directionalThresholdY { get { return _directionalThresholdY; } }

	/// <summary>Gets groundedNeutralComboIndex property.</summary>
	public int groundedNeutralComboIndex { get { return _groundedNeutralComboIndex; } }

	/// <summary>Gets groundedDirectionalComboIndex property.</summary>
	public int groundedDirectionalComboIndex { get { return _groundedDirectionalComboIndex; } }

	/// <summary>Gets airNeutralComboIndex property.</summary>
	public int airNeutralComboIndex { get { return _airNeutralComboIndex; } }

	/// <summary>Gets airDirectionalComboIndex property.</summary>
	public int airDirectionalComboIndex { get { return _airDirectionalComboIndex; } }

	/// <summary>Gets scaleChangePriority property.</summary>
	public int scaleChangePriority { get { return _scaleChangePriority; } }

	/// <summary>Gets initialPoseCredential property.</summary>
	public AnimatorCredential initialPoseCredential { get { return _initialPoseCredential; } }

	/// <summary>Gets initialPoseIDCredential property.</summary>
	public AnimatorCredential initialPoseIDCredential { get { return _initialPoseIDCredential; } }

	/// <summary>Gets vitalityIDCredential property.</summary>
	public AnimatorCredential vitalityIDCredential { get { return _vitalityIDCredential; } }

	/// <summary>Gets leftAxisXCredential property.</summary>
	public AnimatorCredential leftAxisXCredential { get { return _leftAxisXCredential; } }

	/// <summary>Gets leftAxisYCredential property.</summary>
	public AnimatorCredential leftAxisYCredential { get { return _leftAxisYCredential; } }

	/// <summary>Gets rightAxisXCredential property.</summary>
	public AnimatorCredential rightAxisXCredential { get { return _rightAxisXCredential; } }

	/// <summary>Gets rightAxisYCredential property.</summary>
	public AnimatorCredential rightAxisYCredential { get { return _rightAxisYCredential; } }

	/// <summary>Gets jumpStateIDCredential property.</summary>
	public AnimatorCredential jumpStateIDCredential { get { return _jumpStateIDCredential; } }

	/// <summary>Gets jumpingIDCredential property.</summary>
	public AnimatorCredential jumpingIDCredential { get { return _jumpingIDCredential; } }

	/// <summary>Gets groundedCredential property.</summary>
	public AnimatorCredential groundedCredential { get { return _groundedCredential; } }

	/// <summary>Gets attackIDCredential property.</summary>
	public AnimatorCredential attackIDCredential { get { return _attackIDCredential; } }
	
	/// <summary>Gets attackCredential property.</summary>
	public AnimatorCredential attackCredential { get { return _attackCredential; } }

	/// <summary>Gets shootingStateIDCredential property.</summary>
	public AnimatorCredential shootingStateIDCredential { get { return _shootingStateIDCredential; } }

	/// <summary>Gets dashingCredential property.</summary>
	public AnimatorCredential dashingCredential { get { return _dashingCredential; } }

	/// <summary>Gets brakingCredential property.</summary>
	public AnimatorCredential brakingCredential { get { return _brakingCredential; } }

	/// <summary>Gets impactedWithWallCredential property.</summary>
	public AnimatorCredential impactedWithWallCredential { get { return _impactedWithWallCredential; } }

	/// <summary>Gets walledCredential property.</summary>
	public AnimatorCredential walledCredential { get { return _walledCredential; } }

	/// <summary>Gets extraJumpTrailRenderer property.</summary>
	public TrailRenderer extraJumpTrailRenderer { get { return _extraJumpTrailRenderer; } }

	/// <summary>Gets movementAbility Component.</summary>
	public RigidbodyMovementAbility movementAbility
	{ 
		get
		{
			if(_movementAbility == null) _movementAbility = GetComponent<RigidbodyMovementAbility>();
			return _movementAbility;
		}
	}

	/// <summary>Gets rotationAbility Component.</summary>
	public RotationAbility rotationAbility
	{ 
		get
		{
			if(_rotationAbility == null) _rotationAbility = GetComponent<RotationAbility>();
			return _rotationAbility;
		}
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

	/// <summary>Gets dashAbility Component.</summary>
	public DashAbility dashAbility
	{ 
		get
		{
			if(_dashAbility == null) _dashAbility = GetComponent<DashAbility>();
			return _dashAbility;
		}
	}

	/// <summary>Gets shootProjectile Component.</summary>
	public ShootChargedProjectile shootProjectile
	{ 
		get
		{
			if(_shootProjectile == null) _shootProjectile = GetComponent<ShootChargedProjectile>();
			return _shootProjectile;
		}
	}

	/// <summary>Gets animator Component.</summary>
	public Animator animator
	{ 
		get
		{
			if(_animator == null) _animator = GetComponent<Animator>();
			return _animator;
		}
	}

	/// <summary>Gets deltaCalculator Component.</summary>
	public TransformDeltaCalculator deltaCalculator
	{ 
		get
		{
			if(_deltaCalculator == null) _deltaCalculator = GetComponent<TransformDeltaCalculator>();
			return _deltaCalculator;
		}
	}

	/// <summary>Gets sensorSystem Component.</summary>
	public SensorSystem2D sensorSystem
	{ 
		get
		{
			if(_sensorSystem == null) _sensorSystem = GetComponent<SensorSystem2D>();
			return _sensorSystem;
		}
	}

	/// <summary>Gets wallEvaluator Component.</summary>
	public WallEvaluator wallEvaluator
	{ 
		get
		{
			if(_wallEvaluator == null) _wallEvaluator = GetComponent<WallEvaluator>();
			return _wallEvaluator;
		}
	}

	/// <summary>Gets attacksHandler Component.</summary>
	public AnimationAttacksHandler attacksHandler
	{ 
		get
		{
			if(_attacksHandler == null) _attacksHandler = GetComponent<AnimationAttacksHandler>();
			return _attacksHandler;
		}
	}

	/// <summary>Gets slopeEvaluator Component.</summary>
	public SlopeEvaluator slopeEvaluator
	{ 
		get
		{
			if(_slopeEvaluator == null) _slopeEvaluator = GetComponent<SlopeEvaluator>();
			return _slopeEvaluator;
		}
	}

	/// <summary>Gets cameraTarget Component.</summary>
	public VCameraTarget cameraTarget
	{ 
		get
		{
			if(_cameraTarget == null) _cameraTarget = GetComponent<VCameraTarget>();
			return _cameraTarget;
		}
	}

	/// <summary>Gets and Sets orientation property.</summary>
	public Vector3 orientation
	{
		get { return _orientation; }
		set { _orientation = value; }
	}

	/// <summary>Gets and Sets leftAxes property.</summary>
	public Vector2 leftAxes
	{
		get { return _leftAxes; }
		private set { _leftAxes = value; }
	}

	/// <summary>Gets and Sets postInitialPoseCooldown property.</summary>
	public Cooldown postInitialPoseCooldown
	{
		get { return _postInitialPoseCooldown; }
		private set { _postInitialPoseCooldown = value; }
	}

	/// <summary>Gets directionTowardsBackground property.</summary>
	public Vector3 directionTowardsBackground { get { return stareAtBossRotation * Vector3.forward; } }
#endregion

	/// <summary>Mateo's instance initialization when loaded [Before scene loads].</summary>
	protected override void Awake()
	{
		base.Awake();

		orientation = Vector3.right;
		sword.ActivateHitBoxes(false);

		jumpAbility.onJumpStateChange += OnJumpStateChange;
		wallEvaluator.onWallEvaluatorEvent += OnWallEvaluatorEvent;
		dashAbility.onDashStateChange += OnDashStateChange;
		attacksHandler.onAnimationAttackEvent += OnAnimationAttackEvent;

		postInitialPoseCooldown = new Cooldown(this, postMeditationDuration, OnPostMeditationEnds);
	}

	/// <summary>Updates Mateo's instance at each frame.</summary>
	private void Update()
	{
		Vector3 direction = new Vector3(
			leftAxes.x,
			0.0f,
			leftAxes.y
		);

		Debug.DrawRay(transform.position, direction * 10f, Color.magenta);

		if(leftAxes.x != 0.0f) rotationAbility.RotateTowardsDirection(animatorParent, direction);
		//else rotationAbility.RotateTowards(animatorParent, stareAtPlayerRotation);

		// TEST
		///rotationAbility.RotateTowardsDirection(animatorParent, Vector3.right * leftAxes.x);

		/*Debug.DrawRay(transform.position, Vector3.right * leftAxes.x * 5.0f, Color.magenta);
		Debug.DrawRay(transform.position, Quaternion.LookRotation(Vector3.right * leftAxes.x) * Vector3.forward * 8.0f, Color.yellow);*/

		if(animator == null) return;

		animator.SetBool(walledCredential, wallEvaluator.walled);
		animator.SetBool(brakingCredential, movementAbility.braking);
	}

	/// <summary>Callback for setting up animation IK (inverse kinematics).</summary>
	/// <param name="_layer">The index of the layer on which the IK solver is called.</param>
	private void OnAnimatorIK(int _layer)
	{
		if(animator == null) return;

		/*if(leftAxes.x > 0.0f) rotationAbility.RotateTowardsDirection(animator, Vector3.right * leftAxes.x);
		else rotationAbility.RotateTowards(animator, stareAtPlayerRotation);*/
	}

#region Callbacks:
	/// <summary>Callback invoked when the Left Axes changes.</summary>
	/// <param name="_axes">Left's Axes.</param>
	public void OnLeftAxesChange(Vector2 _axes)
	{
		animator.SetFloat(leftAxisXCredential, _axes.x);
		animator.SetFloat(leftAxisYCredential, _axes.y);
		if(leftAxes.x == 0.0f && leftAxes == _axes) movementAbility.Stop();
		if(jumpAbility.HasAnyOfTheStates(JumpAbility.STATE_ID_FALLING)) jumpAbility.AddGravityScalar(_axes.y);
		leftAxes = _axes;
	}

	/// <summary>Callback invoked when the Right Axes changes.</summary>
	/// <param name="_axes">Right's Axes.</param>
	public void OnRightAxesChange(Vector2 _axes)
	{
		animator.SetFloat(rightAxisXCredential, _axes.x);
		animator.SetFloat(rightAxisYCredential, _axes.y);
	}

	/// <summary>Callback invoked when JumpAbility's State Changes.</summary>
	/// <param name="_stateID">State's ID.</param>
	/// <param name="_jumpLevel">Jump's Level [index].</param>
	private void OnJumpStateChange(int _stateID, int _jumpLevel)
	{
		switch(_stateID)
		{
			case JumpAbility.STATE_ID_GROUNDED:
			animator.SetInteger(jumpStateIDCredential, JumpAbility.STATE_FLAG_GROUNDED);
			CancelSwordAttack();
			if(extraJumpTrailRenderer != null) extraJumpTrailRenderer.enabled = false;
			break;

			case JumpAbility.STATE_ID_JUMPING:
			animator.SetInteger(jumpStateIDCredential, JumpAbility.STATE_FLAG_JUMPING);
			
			if(jumpAbility.currentJumpIndex > 0 && extraJumpTrailRenderer != null)
			{
				extraJumpTrailRenderer.Clear();
				extraJumpTrailRenderer.enabled = true;
			}
			break;

			case JumpAbility.STATE_ID_FALLING:
			//if(_jumpLevel <= 0) jumpAbility.AdvanceJumpIndex();
			animator.SetInteger(jumpStateIDCredential, JumpAbility.STATE_FLAG_FALLING);
			if(extraJumpTrailRenderer != null)
			{
				extraJumpTrailRenderer.enabled = false;
				extraJumpTrailRenderer.Clear();
			}
			break;

			case JumpAbility.STATE_ID_LANDING:
			animator.SetInteger(jumpStateIDCredential, JumpAbility.STATE_FLAG_LANDING);
			break;
	
			default:
			/*if(jumpAbility.HasAnyOfTheStates(JumpAbility.STATE_ID_GROUNDED | JumpAbility.STATE_ID_LANDING))
			{ /// If the jumping ability is either on the ground or landing, evaluate which one to asignate a new ID to the AnimatorController:

				int ID = jumpAbility.HasState(JumpAbility.STATE_ID_GROUNDED) ?
					JumpAbility.STATE_FLAG_GROUNDED
					: JumpAbility.STATE_FLAG_LANDING;

				animator.SetInteger(jumpStateIDCredential, ID);
			}*/
			break;
		}

		animator.SetInteger(jumpingIDCredential, _jumpLevel);
	}

	/// <summary>Callback invoked when a WallEvaluator's event occurs.</summary>
	/// <param name="_event">Event's argument.</param>
	private void OnWallEvaluatorEvent(WallEvaluationEvent _event)
	{
		switch(_event)
		{
			case WallEvaluationEvent.OffWall:
			break;

			case WallEvaluationEvent.Walled:
			if(dashAbility.reachedMinMagnitude)
			{
				dashAbility.CancelDash();
				wallEvaluator.BounceOffWall(-orientation);
				animator.SetBool(impactedWithWallCredential, true);
			}
			break;

			case WallEvaluationEvent.BounceEnds:
			animator.SetBool(impactedWithWallCredential, false);
			break;
		}

		//Debug.Log("[Mateo] WallEvaluator Event invoked: " + _event.ToString());
	}

	/// <summary>Callback invoked whn a Dash State changes.</summary>
	/// <param name="_state">New Entered State.</param>
	private void OnDashStateChange(DashState _state)
	{
		animator.SetBool(dashingCredential, _state == DashState.Dashing);
	}

	/// <summary>Callback invoked whan an Animation Attack event occurs.</summary>
	/// <param name="_state">Animation Attack's Event/State.</param>
	private void OnAnimationAttackEvent(AnimationCommandState _state)
	{
		switch(_state)
		{
			case AnimationCommandState.None:
			sword.ActivateHitBoxes(false);
			break;

		    case AnimationCommandState.Startup:
		    sword.ActivateHitBoxes(false);
		    //jumpAbility.gravityApplier.RequestScaleChange(GetInstanceID(), gravityScale, scaleChangePriority);
		    break;

		    case AnimationCommandState.Active:
		    sword.ActivateHitBoxes(true);
		    break;

		    case AnimationCommandState.Recovery:
		    break;

		    case AnimationCommandState.End:
		    CancelSwordAttack();
		    jumpAbility.gravityApplier.RejectScaleChange(GetInstanceID());
		    break;
		}

		/*Debug.Log("[Mateo] OnAnimationAttackEvent("
			+ _state.ToString()
			+ ")"
			+ ". With AttackHandler ID on "
			+ attacksHandler.attackID);*/
	}

	/// <summary>Callback invoked when the health of the character is depleted.</summary>
	protected override void OnHealthEvent(HealthEvent _event, float _amount = 0.0f)
	{
		base.OnHealthEvent(_event, _amount);

		switch(_event)
		{
			case HealthEvent.Depleted:
			animator.SetInteger(vitalityIDCredential, STATE_FLAG_HURT);
			break;

			case HealthEvent.Replenished:
			case HealthEvent.HitStunEnds:
			animator.SetInteger(vitalityIDCredential, STATE_FLAG_ALIVE);
			break;

			case HealthEvent.InvincibilityEnds:
			animator.SetInteger(vitalityIDCredential, STATE_FLAG_ALIVE);
			break;

			case HealthEvent.FullyDepleted:
			animator.SetInteger(vitalityIDCredential, STATE_FLAG_DEAD);
			//animator.SetAllLayersWeight(0.0f);
			break;
		}
	}
#endregion

#region Commands:
	/// <summary>Makes Mateo Perform its initial pose.</summary>
	/// <param name="_perform">Perform Initial Pose? True by default.</param>
	/// <param name="_ID">Initial Pose's ID [Staring at Player's Pose ID by default].</param>
	public void PerformInitialPose(bool _perform = true, int _initialPoseID = ID_INITIALPOSE_STARRINGATPLAYER)
	{
		if(_perform)
		{
			this.AddStates(ID_STATE_MEDITATING);
			animator.SetTrigger(initialPoseCredential);
			animator.SetInteger(initialPoseIDCredential, _initialPoseID);
			if(postInitialPoseCooldown != null) postInitialPoseCooldown.End();
		}
		else
		{
			this.RemoveStates(ID_STATE_MEDITATING);

			if(postInitialPoseCooldown != null && !postInitialPoseCooldown.onCooldown)
			postInitialPoseCooldown.Begin();
			InvokeIDEvent(ID_EVENT_INITIALPOSE_ENDED);
		}
	}

	/// <summary>Moves Player towards given displacement axes.</summary>
	/// <param name="_axes">Displacement axes.</param>
	/// <param name="_scale">Displacement's Scale [1.0f by default].</param>
	public void Move(Vector2 _axes, float _scale = 1.0f)
	{
		Vector2 direction = wallEvaluator.GetWallHitInfo().point - (Vector2)transform.position;

		/* Move If:
			- Mateo is not hurt
			- Mateo is not landing.
			- Mateo is not attacking while grounded.
			- Mateo is not dashing.
			- Mateo is not bouncing.
			- Mateo is not walled and not trying to walk towards the wall.
			- Mateo is not on its initial pose.
		*/
		if(this.HasStates(ID_STATE_HURT)
		|| jumpAbility.HasStates(JumpAbility.STATE_ID_LANDING)
		|| (jumpAbility.grounded && attacksHandler.state != AttackState.None)
		|| dashAbility.state == DashState.Dashing
		|| wallEvaluator.state == WallEvaluationEvent.Bouncing
		|| (wallEvaluator.walled && Mathf.Sign(_axes.x) == Mathf.Sign(direction.x))
		|| postInitialPoseCooldown.onCooldown) return;

		if(this.HasStates(ID_STATE_MEDITATING))
		{
			PerformInitialPose(false);
			return;
		}

		float scale = (jumpAbility.HasStates(JumpAbility.STATE_ID_JUMPING) ? jumpingMovementScale : 1.0f) * _scale;

		//transform.rotation = Quaternion.Euler(0.0f, _axes.x < 0.0f ? 180.0f : 0.0f, 0.0f);
		movementAbility.Move(slopeEvaluator.normalAdjuster.right.normalized * _axes.magnitude, scale, Space.World);
		slopeEvaluator.normalAdjuster.forward = _axes.x > 0.0f ? Vector3.forward : Vector3.back;
		orientation = _axes.x > 0.0f ? Vector3.right : Vector3.left;
	}

	/// <summary>Performs Jump.</summary>
	/// <param name="_axes">Additional Direction's Axes.</param>
	public void Jump(Vector2 _axes)
	{
		if(this.HasStates(ID_STATE_HURT)
		|| (jumpAbility.grounded && attacksHandler.state != AttackState.None)) return;

		jumpAbility.Jump(_axes);

		/*if(jumpAbility.currentJumpIndex > 0 && extraJumpTrailRenderer != null)
		{
			extraJumpTrailRenderer.Clear();
			extraJumpTrailRenderer.enabled = true; 
		}*/
	}

	/// <summary>Cancels Jump.</summary>
	public void CancelJump()
	{
		jumpAbility.CancelJump();
	}

	/// <summary>Performs Dash.</summary>
	public void Dash()
	{
		if(this.HasStates(ID_STATE_HURT) || Mathf.Abs(leftAxes.x) < Mathf.Abs(dashXThreshold)) return;

		dashAbility.Dash(orientation);
	}

	/// <summary>Performs Sword's Attack.</summary>
	/// <param name="_axes">Left Axes.</param>
	public void SwordAttack(Vector2 _axes)
	{
		/* Attack If Either:
			- Mateo is not hurt.
			- Mateo is not already attacking.
			- Mateo is not on waiting state.
			- Mateo is not bouncing from a wall.
			- Mateo is not landing.
		*/
		if(this.HasStates(ID_STATE_HURT)
		|| attacksHandler.state == AttackState.Attacking
		|| attacksHandler.state == AttackState.Waiting
		|| wallEvaluator.state == WallEvaluationEvent.Bouncing
		|| jumpAbility.HasStates(JumpAbility.STATE_ID_LANDING)) return;
			

		int index = 0;
		bool applyDirectional = directionalThresholdX.ValueOutside(leftAxes.x) || directionalThresholdY.ValueOutside(leftAxes.y);
		bool grounded = jumpAbility.grounded;

		if(grounded) index = applyDirectional ? groundedDirectionalComboIndex : groundedNeutralComboIndex;
		else index = applyDirectional ? airDirectionalComboIndex : airNeutralComboIndex;

		if(attacksHandler.BeginAttack(index))
		{
			sword.ActivateHitBoxes(true);
			animator.SetBool(attackCredential, true);
			animator.SetInteger(attackIDCredential, attacksHandler.attackID);
		}
	}

	/// <summary>Cancels Attacks.</summary>
	public void CancelSwordAttack()
	{
		attacksHandler.CancelAttack();
		sword.ActivateHitBoxes(false);
		jumpAbility.gravityApplier.RejectScaleChange(GetInstanceID());
		animator.SetBool(attackCredential, false);
		animator.SetInteger(attackIDCredential, attacksHandler.attackID);
	}

	/// <summary>Charges Fire.</summary>
	/// <param name="_axes">Axes' Argument.</param>
	public void ChargeFire(Vector3 _axes)
	{
		if(this.HasStates(ID_STATE_HURT)) return;

		int chargeStateID = shootProjectile.OnCharge(_axes);
		animator.SetInteger(shootingStateIDCredential, chargeStateID);
	}

	/// <summary>Charges Fire.</summary>
	/// <param name="_shootResult">Was the shoot made? false by default.</param>
	public void DischargeFire(bool _shootResult = false)
	{
		if(shootProjectile.onCooldown) return;

		shootProjectile.OnDischarge();
		int stateID = _shootResult ? ShootChargedProjectile.STATE_ID_RELEASED : ShootChargedProjectile.STATE_ID_UNCHARGED;
		animator.SetInteger(shootingStateIDCredential, stateID);
	}

	/// <summary>Releases Fire.</summary>
	/// <param name="_axes">Axes' Argument.</param>
	public void ReleaseFire(Vector3 _axes)
	{
		if(this.HasStates(ID_STATE_HURT)) return;

		bool result = shootProjectile.Shoot(leftHand.position, _axes);
		if(result) animator.SetInteger(shootingStateIDCredential, ShootChargedProjectile.STATE_ID_RELEASED);
		DischargeFire(result);
	}

	/// <summary>Callback invoking when the Post-Meditation cooldown ends.</summary>
	private void OnPostMeditationEnds()
	{
		this.RemoveStates(ID_STATE_MEDITATING);
	}

	/// <summary>Changes rotation towards given target.</summary>
	/// <param name="_target">Target to stare at.</param>
	public void StareTowards(StareTarget _target = StareTarget.Boss)
	{
		if(animator != null) animator.transform.rotation = _target == StareTarget.Boss ? stareAtBossRotation : stareAtPlayerRotation;
	}
#endregion

/// \TODO Eventually Remove:
#region TESTs:
	public void Hurt()
	{
		health.GiveDamage(0.5f);
		Debug.Log("[Mateo] Take Damage Astro Boy! Health's Information: " + health.ToString());
	}

	public void Kill()
	{
		Debug.Log("[Mateo] Die Astro Boy! Health's Information: " + health.ToString());
		health.GiveDamage(Mathf.Infinity);
	}

	public void Revive()
	{
		Debug.Log("[Mateo] I forgive you Astro Boy, come back! Health's Information: " + health.ToString());
		health.ReplenishHealth(Mathf.Infinity);
	}
#endregion
}
}