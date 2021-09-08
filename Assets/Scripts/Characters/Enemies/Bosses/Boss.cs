using System;
using System.Text;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Voidless;

namespace Flamingo
{
public class Boss : Enemy
{
	public event OnIDEvent onIDEvent; 										/// <summary>OnIDEvent's delegate.</summary>

	public const int ID_EVENT_STAGE_CHANGED = 0; 							/// <summary>Stage Changed's Event ID.</summary>
	public const int ID_EVENT_BOSS_DEATHROUTINE_BEGINS = 1; 				/// <summary>Death Routine Begins' Event ID.</summary>
	public const int ID_EVENT_BOSS_DEATHROUTINE_ENDS = 2; 					/// <summary>Death Routine Ends' Event ID.</summary>
	public const int ID_EVENT_PLAYERSIGHTED_BEGINS = 3; 					/// <summary>Player Sighted Begin's Event ID.</summary>
	public const int ID_EVENT_PLAYERSIGHTED_ENDS = 4; 						/// <summary>Player Sighted Ends's Event ID.</summary>
	public const int STAGE_1 = 1; 											/// <summary>Stage 1's ID.</summary>
	public const int STAGE_2 = 2; 											/// <summary>Stage 2's ID.</summary>
	public const int STAGE_3 = 3; 											/// <summary>Stage 3's ID.</summary>
	public const int STAGE_4 = 4; 											/// <summary>Stage 4's ID.</summary>
	public const int STAGE_5 = 5; 											/// <summary>Stage 5's ID.</summary>

	[Space(5f)]
	[Header("Boss' Attributes:")]
	[SerializeField] private int _stages; 									/// <summary>Boss' Stages.</summary>
	[SerializeField] private float[] _healthDistribution; 					/// <summary>Health Distribution across the Stages.</summary>
	[SerializeField] private RandomDistributionSystem _distributionSystem; 	/// <summary>Distribution System.</summary>
	[SerializeField] private Animator _animator; 							/// <summary>Animator's Component.</summary>
	private int _currentStage; 

	/// <summary>Gets and Sets stages property.</summary>
	public int stages
	{
		get { return _stages; }
		set { _stages = value; }
	}

	/// <summary>Gets and Sets currentStage property.</summary>
	public int currentStage
	{
		get { return _currentStage; }
		set { _currentStage = value; }
	}

	/// <summary>Gets and Sets healthDistribution property.</summary>
	public float[] healthDistribution
	{
		get { return _healthDistribution; }
		set { _healthDistribution = value; }
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

#region UnityMethods:
	/// <summary>Resets Boss's instance to its default values.</summary>
	public override void Reset()
	{
		base.Reset();
		currentStage = 0;
		AdvanceStage();
	}

	/// <summary>Callback internally called right after Awake.</summary>
	protected override void Awake()
	{
		base.Awake();
		currentStage = 0;
		AdvanceStage();
	}

	/// <summary>Callback internally called right after Start.</summary>
	protected override void Start()
	{
		base.Start();
	}
#endregion

	/// <summary>Advances Stage.</summary>
	protected void AdvanceStage()
	{
		if(healthDistribution == null || currentStage >= healthDistribution.Length) return;

		currentStage = Mathf.Min(currentStage, stages);
		health.SetMaxHP(healthDistribution[currentStage++], true);
		OnStageChanged();
	}

	/// <summary>Callback internally called when the Boss advances stage.</summary>
	protected virtual void OnStageChanged()
	{
		if(onIDEvent != null) onIDEvent(ID_EVENT_STAGE_CHANGED);
	}

	/// <summary>Begins Death's Routine.</summary>
	protected virtual void BeginDeathRoutine()
	{
		Debug.Log("[Boss] Beginning Death's Routine...");
		this.RemoveStates(ID_STATE_ALIVE);
		if(onIDEvent != null) onIDEvent(ID_EVENT_BOSS_DEATHROUTINE_BEGINS);
		
		this.StartCoroutine(DeathRoutine(OnDeathRoutineEnds));

	}

	/// <summary>Callback invoked after the Death's routine ends.</summary>
	protected virtual void OnDeathRoutineEnds()
	{
		//OnObjectDeactivation();
		if(onIDEvent != null) onIDEvent(ID_EVENT_BOSS_DEATHROUTINE_ENDS);
		eventsHandler.InvokeEnemyDeactivationEvent(this, DeactivationCause.Destroyed);
	} 

	/// <summary>Callback invoked when a Health's event has occured.</summary>
	/// <param name="_event">Type of Health Event.</param>
	/// <param name="_amount">Amount of health that changed [0.0f by default].</param>
	protected override void OnHealthEvent(HealthEvent _event, float _amount = 0.0f)
	{
		Debug.Log("[Boss] Health Event: " + _event.ToString() + ", at Stage: " + currentStage);

		switch(_event)
		{
			case HealthEvent.FullyDepleted:
			if(currentStage < stages) AdvanceStage();
			else BeginDeathRoutine();
			break;
		}
	}

	/// <returns>String representing enemy's stats.</returns>
	public override string ToString()
	{
		StringBuilder builder = new StringBuilder();

		builder.AppendLine(base.ToString());
		builder.Append("Current Stage: ");
		builder.Append(currentStage.ToString());

		return builder.ToString();
	}

	/// <summary>Death's Routine.</summary>
	/// <param name="onDeathRoutineEnds">Callback invoked when the routine ends.</param>
	protected virtual IEnumerator DeathRoutine(Action onDeathRoutineEnds)
	{
		yield return null;
		if(onDeathRoutineEnds != null) onDeathRoutineEnds();
	}
}
}