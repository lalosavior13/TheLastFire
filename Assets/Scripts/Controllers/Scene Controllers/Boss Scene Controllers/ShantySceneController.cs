using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Voidless;

namespace Flamingo
{
public class ShantySceneController : Singleton<ShantySceneController>
{
	[Space(5f)]
	[SerializeField] private ShantyBoss _shanty; 			/// <summary>Captain Shanty's Reference.</summary>
	[Space(5f)]
	[SerializeField] private Vector3 _tiePosition; 			/// <summary>Tie's Position.</summary>
	[Space(5f)]
	[Header("Ship's Attributes:")]
	[SerializeField] private ShantyShip _shantyShip; 		/// <summary>Shanty's Ship.</summary>
	[Space(5f)]
	[Header("Audio's Attributes:")]
	[SerializeField] private CollectionIndex _loopIndex; 	/// <summary>Loop's Index.</summary>
	[Space(5f)]
	[SerializeField] private Transform _stage1Group; 		/// <summary>Stage 1's Group.</summary>
	[SerializeField] private Transform _stage2Group; 		/// <summary>Stage 2's Group.</summary>
	[SerializeField] private Transform _stage3Group; 		/// <summary>Stage 3's Group.</summary>
#if UNITY_EDITOR
	[Space(5f)]
	[Header("Gizmos' Attributes:")]
	[SerializeField] private Color gizmosColor; 			/// <summary>Gizmos' Color.</summary>
#endif

#region Getters/Setters:
	/// <summary>Gets shanty property.</summary>
	public ShantyBoss shanty { get { return _shanty; } }

	/// <summary>Gets tiePosition property.</summary>
	public Vector3 tiePosition { get { return _tiePosition; } }

	/// <summary>Gets shantyShip property.</summary>
	public ShantyShip shantyShip { get { return _shantyShip; } }

	/// <summary>Gets loopIndex property.</summary>
	public CollectionIndex loopIndex { get { return _loopIndex; } }

	/// <summary>Gets stage1Group property.</summary>
	public Transform stage1Group { get { return _stage1Group; } }

	/// <summary>Gets stage2Group property.</summary>
	public Transform stage2Group { get { return _stage2Group; } }

	/// <summary>Gets stage3Group property.</summary>
	public Transform stage3Group { get { return _stage3Group; } }
#endregion
	
#if UNITY_EDITOR
	/// <summary>Draws Gizmos on Editor mode.</summary>
	private void OnDrawGizmos()
	{
		Gizmos.color = gizmosColor;

		Gizmos.DrawWireSphere(tiePosition, 0.5f);
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

			stage1Group.gameObject.SetActive(false);
			stage2Group.gameObject.SetActive(false);
			stage3Group.gameObject.SetActive(false);

			switch(stageID)
			{
				case Boss.STAGE_1:
				stage1Group.gameObject.SetActive(true);
				break;

				case Boss.STAGE_2:
				stage2Group.gameObject.SetActive(true);
				break;

				case Boss.STAGE_3:
				stage3Group.gameObject.SetActive(true);
				break;
			}
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