using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Voidless;

namespace Flamingo
{
[RequireComponent(typeof(Skeleton))]
public class ShantyBoss : Boss
{
	public const int ID_ANIMATIONEVENT_THROWBOMB = 0; 					/// <summary>Throw Bomb's Animation Event's ID.</summary>
	public const int ID_ANIMATIONEVENT_SWORD_UNSHEATH = 1; 				/// <summary>Sword Un-Sheath's Animation Event's ID.</summary>
	public const int ID_ANIMATIONEVENT_SWORD_SHEATH = 2; 				/// <summary>Sword Sheath's Animation Event's ID.</summary>
	public const int ID_ANIMATIONEVENT_GOIDLE = 3; 						/// <summary>Go Idle's Animation Event's ID.</summary>
	public const int ID_STATE_INTRO = 0; 								/// <summary>Intro's State ID [for AnimatorController].</summary>
	public const int ID_STATE_TIED = 1; 								/// <summary>Intro's State ID [for AnimatorController].</summary>
	public const int ID_STATE_IDLE = 2; 								/// <summary>Idle's State ID [for AnimatorController].</summary>
	public const int ID_STATE_ATTACK = 3; 								/// <summary>Attack's State ID [for AnimatorController].</summary>
	public const int ID_STATE_DAMAGE = 4; 								/// <summary>Damage's State ID [for AnimatorController].</summary>
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

	[SerializeField] private CollectionIndex _bombIndex; 				/// <summary>Bomb's Index.</summary>
	[SerializeField] private float _projectionTime; 					/// <summary>Bomb's Projection Time.</summary>
	[SerializeField] private ContactWeapon _sword; 						/// <summary>Shanty's Sword.</summary>
	[SerializeField] private Transform _falseSword; 					/// <summary>False Sword's Reference (the one stuck to the rigging).</summary>
	[Space(5f)]
	[SerializeField]
	[Range(0.0f, 1.0f)] private float _windowPercentage; 				/// <summary>Time-window before swinging sword (to hit bomb).</summary>
	[Space(5f)]
	[Header("Animator's Attributes:")]
	[SerializeField] private AnimatorCredential _stateIDCredential; 	/// <summary>State ID's AnimatorCredential.</summary>
	[SerializeField] private AnimatorCredential _attackIDCredential; 	/// <summary>Attack ID's AnimatorCredential.</summary>
	[SerializeField] private AnimatorCredential _idleIDCredential; 		/// <summary>Idle ID's AnimatorCredential.</summary>
	[SerializeField] private AnimatorCredential _vitalityIDCredential; 	/// <summary>Vitality ID's AnimatorCredential.</summary>
	private Skeleton _skeleton; 										/// <summary>Skeleton's Component.</summary>
	private Coroutine coroutine; 										/// <summary>Coroutine's Reference.</summary>

#region Getters/Setters:
	/// <summary>Gets bombIndex property.</summary>
	public CollectionIndex bombIndex { get { return _bombIndex; } }

	/// <summary>Gets projectionTime property.</summary>
	public float projectionTime { get { return _projectionTime; } }

	/// <summary>Gets sword property.</summary>
	public ContactWeapon sword { get { return _sword; } }

	/// <summary>Gets falseSword property.</summary>
	public Transform falseSword { get { return _falseSword; } }

	/// <summary>Gets windowPercentage property.</summary>
	public float windowPercentage { get { return _windowPercentage; } }

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
#endregion

	/// <summary>ShantyBoss's instance initialization.</summary>
	protected override void Awake()
	{
		base.Awake();

		ActivateSword(false);
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

	/// <summary>Begins Attack's Routine.</summary>
	public void BeginAttackRoutine()
	{
		switch(currentStage)
		{
			case STAGE_1:
			animator.SetInteger(stateIDCredential, ID_STATE_ATTACK);

			BeginBombThrowingRoutine();
			break;

			case STAGE_2:
			break;

			case STAGE_3:
			break;
		}

		
	}

#region BombThrowingRoutines:
	/// <summary>Begins the Bomb Throwing Animations.</summary>
	public void BeginBombThrowingRoutine()
	{
		/// During the throw animation, a callback will be invoked that will then invoke ThrowBomb()
		animator.SetInteger(stateIDCredential, ID_STATE_ATTACK);
		animator.SetInteger(attackIDCredential, ID_ATTACK_BOMB_THROW);
	}

	/// <summary>Throws Bomb [called after an specific frame of the Bom-Throwing Animation].</summary>
	public void ThrowBomb()
	{
		Projectile bomb = PoolManager.RequestParabolaProjectile(Faction.Enemy, bombIndex, skeleton.rightHand.position, Game.mateo.transform.position, projectionTime, gameObject);
		bomb.projectileEventsHandler.onProjectileEvent -= OnBombEvent;
		bomb.projectileEventsHandler.onProjectileEvent += OnBombEvent;

		animator.SetInteger(stateIDCredential, ID_STATE_IDLE);
	}
#endregion

#region TNTThrowingRoutines:
	/// <summary>Begins TNT ThrowingRoutine.</summary>
	public void BeginTNTThrowingRoutine()
	{

	}

	/// <summary>Picks TNT.</summary>
	public void PickTNT()
	{

	}

	/// <summary>Throws TNT.</summary>
	public void ThrowTNT()
	{

	}
#endregion

#region Callbacks:
	/// <summary>Callback invoked when Shanty ought to be tied.</summary>
	/// <param name="_ship">Ship that will contain Shanty.</param>
	/// <param name="_tiePosition">Tie Position.</param>
	public void OnTie(Transform _ship, Vector3 _tiePosition)
	{
		Debug.Log("[ShantyBoss] Shanty is tied...");
		transform.position = _tiePosition;
		transform.parent = _ship;
		animator.SetInteger(stateIDCredential, ID_STATE_TIED);
	}

	/// <summary>Callback invoked when Shaanty ought to be untied.</summary>
	public void OnUntie()
	{
		Debug.Log("[ShantyBoss] Shanty is untied...");
		BeginAttackRoutine();
	}

	/// <summary>Callback invoked when an Animation Event is invoked.</summary>
	/// <param name="_ID">Int argument.</param>
	protected override void OnAnimationIntEvent(int _ID)
	{
		switch(_ID)
		{
			case ID_ANIMATIONEVENT_THROWBOMB:
			ThrowBomb();
			break;

			case ID_ANIMATIONEVENT_SWORD_UNSHEATH:
			ActivateSword(true);
			animator.SetInteger(stateIDCredential, ID_STATE_IDLE);
			break;

			case ID_ANIMATIONEVENT_SWORD_SHEATH:
			ActivateSword(false);
			break;

			case ID_ANIMATIONEVENT_GOIDLE:
			animator.SetInteger(stateIDCredential, ID_STATE_IDLE);
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

			if(_projectile.owner == null || _projectile.owner == gameObject) return;

			float durationBeforeSwordSwing = _projectile.parabolaTime * windowPercentage;

			this.StartCoroutine(this.WaitSeconds(durationBeforeSwordSwing, 
			()=>
			{
				animator.SetInteger(stateIDCredential, ID_STATE_ATTACK);
				animator.SetInteger(attackIDCredential, ID_ATTACL_HIT_TENNIS);
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
			case HealthEvent.FullyDepleted:
			animator.SetInteger(stateIDCredential, ID_STATE_DAMAGE);

			if(_object == null) return;

			int damageID = 0;

			if(_object.CompareTag(Game.data.playerWeaponTag))
			{

			} else if(_object.CompareTag(Game.data.playerWeaponTag))
			{

			} else if(_object.CompareTag(Game.data.playerProjectileTag))
			{

			} else if(_object.CompareTag(Game.data.explodableTag))
			{

			}

			animator.SetInteger(vitalityIDCredential, damageID);
			break;
		}
	}
#endregion

}
}