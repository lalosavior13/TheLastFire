using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Voidless;

namespace Flamingo
{
[Serializable] public struct Vector3Pair { public Vector3 a; public Vector3 b; }

public class ShantySceneController : Singleton<ShantySceneController>
{
	[Space(5f)]
	[SerializeField] private ShantyBoss _shanty; 								/// <summary>Captain Shanty's Reference.</summary>
	[Space(5f)]
	[SerializeField] private Vector3 _tiePosition; 								/// <summary>Tie's Position.</summary>
	[Space(5f)]
	[Header("Ship's Attributes:")]
	[SerializeField] private ShantyShip _shantyShip; 							/// <summary>Shanty's Ship.</summary>
	[Space(5f)]
	[Header("Audio's Attributes:")]
	[SerializeField] private CollectionIndex _loopIndex; 						/// <summary>Loop's Index.</summary>
	[Space(5f)]
	[SerializeField] private Transform[] _stage1ObjectsToDeactivate; 			/// <summary>Stage 1's Objects to deactivate.</summary>
	[SerializeField] private Transform[] _stage2ObjectsToDeactivate; 			/// <summary>Stage 2's Objects to deactivate.</summary>
	[SerializeField] private Transform[] _stage3ObjectsToDeactivate; 			/// <summary>Stage 3's Objects to deactivate.</summary>
	[SerializeField] private Transform _stage1Group; 							/// <summary>Stage 1's Group.</summary>
	[SerializeField] private Transform _stage2Group; 							/// <summary>Stage 2's Group.</summary>
	[SerializeField] private Transform _stage3Group; 							/// <summary>Stage 3's Group.</summary>
	[Space(5f)]
	[Header("Camera Boundaries' Modifiers:")]
	[SerializeField] private Camera2DBoundariesModifier _stage1CameraSettings; 	/// <summary>Camera Settings for Stage 1.</summary>
	[SerializeField] private Camera2DBoundariesModifier _stage2CameraSettings; 	/// <summary>Camera Settings for Stage 2.</summary>
	[SerializeField] private Camera2DBoundariesModifier _stage3CameraSettings; 	/// <summary>Camera Settings for Stage 3.</summary>
	[Space(5f)]
	[Header("Boundaries:")]
	[SerializeField] private ScenarioBoundariesContainer _dockBoundaries; 		/// <summary>Dock's Boundaries.</summary>
	[SerializeField] private ScenarioBoundariesContainer _shipBoundaries; 		/// <summary>Ship's Boundaries.</summary>
	[Space(5f)]
	[Header("Spawn Positions:")]
	[SerializeField] private Vector3 _stage1MateoPosition; 						/// <summary>Spawn Position for Mateo at Stage 1.</summary>
	[SerializeField] private Vector3 _stage2MateoPosition; 						/// <summary>Spawn Position for Mateo at Stage 2.</summary>
	[SerializeField] private Vector3 _stage3MateoPosition; 						/// <summary>Spawn Position for Mateo at Stage 3.</summary>
	[SerializeField] private Vector3 _stage1ShantyPosition; 					/// <summary>Spawn Position for Shanty at Stage 1.</summary>
	[SerializeField] private Vector3 _stage2ShantyPosition; 					/// <summary>Spawn Position for Shanty at Stage 2.</summary>
	[SerializeField] private Vector3 _stage3ShantyPosition; 					/// <summary>Spawn Position for Shanty at Stage 3.</summary>
	[Space(5f)]
	[Header("Stage 2's Attributes:")]
	[SerializeField] private float _waitBeforeFade; 							/// <summary>Wait's duration before screen fade.</summary>
	[SerializeField] private float _fadeInDuration; 							/// <summary>Fade-In's Duration.</summary>
	[SerializeField] private float _fadeOutDuration; 							/// <summary>Fade-Out's Duration.</summary>
	[SerializeField] private Vector3Pair[] _whackAMoleWaypointsPairs; 			/// <summary>Waypoint pairs for the Whack-a-Mole phase.</summary>
	[Space(5f)]
	[Header("Particle Effects:")]
	[SerializeField] private Vector3[] _smokeSpawnPositions; 					/// <summary>Spawn Positions for the Some's ParticleEffect.</summary>
	[SerializeField] private CollectionIndex _smokeEffectIndex; 				/// <summary>Smoke ParticleEffect's Index.</summary>
	[Space(5f)]
	[SerializeField] private TransformData _stage2ShipTransformData; 			/// <summary>Stage 2's Ship Transform Data.</summary>
#if UNITY_EDITOR
	[Space(5f)]
	[Header("Gizmos' Attributes:")]
	[SerializeField] private Color gizmosColor; 								/// <summary>Gizmos' Color.</summary>
#endif

#region Getters/Setters:
	/// <summary>Gets shanty property.</summary>
	public ShantyBoss shanty { get { return _shanty; } }

	/// <summary>Gets tiePosition property.</summary>
	public Vector3 tiePosition { get { return _tiePosition; } }

	/// <summary>Gets smokeSpawnPositions property.</summary>
	public Vector3[] smokeSpawnPositions { get { return _smokeSpawnPositions; } }

	/// <summary>Gets waitBeforeFade property.</summary>
	public float waitBeforeFade { get { return _waitBeforeFade; } }

	/// <summary>Gets fadeInDuration property.</summary>
	public float fadeInDuration { get { return _fadeInDuration; } }

	/// <summary>Gets fadeOutDuration property.</summary>
	public float fadeOutDuration { get { return _fadeOutDuration; } }

	/// <summary>Gets whackAMoleWaypointsPairs property.</summary>
	public Vector3Pair[] whackAMoleWaypointsPairs { get { return _whackAMoleWaypointsPairs; } }

	/// <summary>Gets shantyShip property.</summary>
	public ShantyShip shantyShip { get { return _shantyShip; } }

	/// <summary>Gets loopIndex property.</summary>
	public CollectionIndex loopIndex { get { return _loopIndex; } }

	/// <summary>Gets smokeEffectIndex property.</summary>
	public CollectionIndex smokeEffectIndex { get { return _smokeEffectIndex; } }

	/// <summary>Gets stage1Group property.</summary>
	public Transform stage1Group { get { return _stage1Group; } }

	/// <summary>Gets stage2Group property.</summary>
	public Transform stage2Group { get { return _stage2Group; } }

	/// <summary>Gets stage3Group property.</summary>
	public Transform stage3Group { get { return _stage3Group; } }

	/// <summary>Gets stage1MateoPosition property.</summary>
	public Vector3 stage1MateoPosition { get { return _stage1MateoPosition; } }

	/// <summary>Gets stage2MateoPosition property.</summary>
	public Vector3 stage2MateoPosition { get { return _stage2MateoPosition; } }

	/// <summary>Gets stage3MateoPosition property.</summary>
	public Vector3 stage3MateoPosition { get { return _stage3MateoPosition; } }

	/// <summary>Gets stage1ShantyPosition property.</summary>
	public Vector3 stage1ShantyPosition { get { return _stage1ShantyPosition; } }

	/// <summary>Gets stage2ShantyPosition property.</summary>
	public Vector3 stage2ShantyPosition { get { return _stage2ShantyPosition; } }

	/// <summary>Gets stage3ShantyPosition property.</summary>
	public Vector3 stage3ShantyPosition { get { return _stage3ShantyPosition; } }

	/// <summary>Gets dockBoundaries property.</summary>
	public ScenarioBoundariesContainer dockBoundaries { get { return _dockBoundaries; } }

	/// <summary>Gets shipBoundaries property.</summary>
	public ScenarioBoundariesContainer shipBoundaries { get { return _shipBoundaries; } }

	/// <summary>Gets stage2ShipTransformData property.</summary>
	public TransformData stage2ShipTransformData { get { return _stage2ShipTransformData; } }

	/// <summary>Gets stage1CameraSettings property.</summary>
	public Camera2DBoundariesModifier stage1CameraSettings { get { return _stage1CameraSettings; } }

	/// <summary>Gets stage2CameraSettings property.</summary>
	public Camera2DBoundariesModifier stage2CameraSettings { get { return _stage2CameraSettings; } }

	/// <summary>Gets stage3CameraSettings property.</summary>
	public Camera2DBoundariesModifier stage3CameraSettings { get { return _stage3CameraSettings; } }
#endregion
	
#if UNITY_EDITOR
	/// <summary>Draws Gizmos on Editor mode.</summary>
	private void OnDrawGizmos()
	{
		Gizmos.color = gizmosColor;

		Gizmos.DrawWireSphere(tiePosition, 0.5f);

		if(smokeSpawnPositions != null) foreach(Vector3 position in smokeSpawnPositions)
		{
			Gizmos.DrawWireSphere(position, 0.5f);
		}

		if(whackAMoleWaypointsPairs != null && shantyShip != null) foreach(Vector3Pair pair in whackAMoleWaypointsPairs)
		{
			Gizmos.DrawWireSphere(shantyShip.transform.TransformPoint(pair.a), 0.5f);
			Gizmos.DrawWireSphere(shantyShip.transform.TransformPoint(pair.b), 0.5f);
		}

		Gizmos.DrawWireSphere(stage1MateoPosition, 0.5f);
		Gizmos.DrawWireSphere(stage2MateoPosition, 0.5f);
		Gizmos.DrawWireSphere(stage3MateoPosition, 0.5f);
		Gizmos.DrawWireSphere(stage1ShantyPosition, 0.5f);
		Gizmos.DrawWireSphere(stage2ShantyPosition, 0.5f);
		Gizmos.DrawWireSphere(stage3ShantyPosition, 0.5f);
	}
#endif

	/// <summary>ShantySceneController's instance initialization.</summary>
	private void Awake()
	{
		AudioController.Play(SourceType.Loop, 0, loopIndex);
		
		if(shanty != null)
		{
			shanty.onIDEvent += OnShantyIDEvent;
			if(shantyShip != null) shanty.ship = shantyShip;
		}
	}

	/// <summary>ShantySceneController's starting actions before 1st Update frame.</summary>
	private void Start ()
	{
		Introduction();	
	}

	/// <summary>Ties Shanty into rope and docks ship.</summary>
	private void Introduction()
	{
		if(shanty == null
		|| shanty.animator == null
		|| shantyShip == null) return;

		shantyShip.ropeHitBox.onTriggerEvent2D -= OnRopeHit; 			/// Just in case...
		shantyShip.ropeHitBox.onTriggerEvent2D += OnRopeHit;

		shanty.OnTie(shantyShip.transform, tiePosition);
		shantyShip.GoToState(ShantyShip.ID_STATE_DOCKED);
	}

#region Callbacks:
	/// <summary>Callback invoked when Shanty invokes an ID's Event.</summary>
	/// <param name="_ID">Event's ID.</param>
	private void OnShantyIDEvent(int _ID)
	{
		switch(_ID)
		{
			case Boss.ID_EVENT_STAGE_CHANGED:
			if(stage1Group == null
			|| stage2Group == null
			|| stage3Group == null) return;

			int stageID = shanty.currentStage;

			Debug.Log("[ShantySceneController] Shanty Stage Changed to: " + stageID);

			switch(stageID)
			{
				case Boss.STAGE_1:
				dockBoundaries.Enable(true);
				shipBoundaries.Enable(false);
				break;

				case Boss.STAGE_2:
				ParticleEffect effect = null;
				ParticleSystem.MainModule mainModule = default(ParticleSystem.MainModule);
				
				shanty.ChangeState(Enemy.ID_STATE_ALIVE | Enemy.ID_STATE_IDLE);
				Game.EnablePlayerControl(false);
				dockBoundaries.Enable(false);
				shipBoundaries.Enable(true);

				foreach(Vector3 position in smokeSpawnPositions)
				{
					effect = PoolManager.RequestParticleEffect(smokeEffectIndex, position, Quaternion.identity);
				}
				mainModule = effect.systems[0].main;

				this.StartCoroutine(this.WaitSeconds(waitBeforeFade, ()=>
				{
					Game.gameplayGUIController.screenFaderGUI.FadeIn(Color.white, fadeInDuration,
					()=>
					{
						shantyShip.transform.position = stage2ShipTransformData.position;
						shantyShip.transform.rotation = stage2ShipTransformData.rotation;
						ActivateStage(stageID);
						this.StartCoroutine(this.WaitSeconds(mainModule.startLifetime.constantMax, ()=>
						{
							Game.gameplayGUIController.screenFaderGUI.FadeOut(Color.white, fadeOutDuration,
							()=>
							{
								Game.EnablePlayerControl(true);
								shanty.BeginAttackRoutine();
							});
						}));
					});
				}));
				ActivateStage(stageID);
				break;

				case Boss.STAGE_3:
				dockBoundaries.Enable(true);
				shipBoundaries.Enable(false);
				shantyShip.gameObject.SetActive(false);
				ActivateStage(stageID);
				break;
			}
			break;
		}
	}

	/// <summary>Activates Stage.</summary>
	/// <param name="index">Stage's Index.</param>
	private void ActivateStage(int index)
	{
		stage1Group.gameObject.SetActive(false);
		stage2Group.gameObject.SetActive(false);
		stage3Group.gameObject.SetActive(false);

		switch(index)
		{
			case Boss.STAGE_1:
			stage1Group.SetActive(true);
			break;

			case Boss.STAGE_2:
			stage2Group.SetActive(true);
			break;

			case Boss.STAGE_3:
			stage3Group.SetActive(true);
			break;

			default:
			Debug.LogError("[ShantySceneController] Imbecile, you provided the wrong stage number (" + index + ") when activating Stage...");
			break;
		}
	}

	/// <summary>Event invoked when this Hit Collider2D intersects with another GameObject.</summary>
	/// <param name="_collider">Collider2D that was involved on the Hit Event.</param>
	/// <param name="_eventType">Type of the event.</param>
	/// <param name="_hitColliderID">Optional ID of the HitCollider2D.</param>
	private void OnRopeHit(Collider2D _collider, HitColliderEventTypes _eventType, int _ID = 0)
	{
		GameObject obj = _collider.gameObject;

		if(obj.CompareTag(Game.data.playerWeaponTag) || obj.CompareTag(Game.data.playerProjectileTag))
		{
			shantyShip.ropeHitBox.onTriggerEvent2D -= OnRopeHit; 		/// Just in case...
			shantyShip.ropeHitBox.gameObject.SetActive(false);

			shanty.OnUntie();
		}
	}
#endregion

}
}