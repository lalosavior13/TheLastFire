using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Voidless;

namespace Flamingo
{
[CreateAssetMenu]
public class DevilBehavior : DestinoScriptableCoroutine
{
	[SerializeField] private CollectionIndex _arrowProjectileIndex; 	/// <summary>Arrow Projectile's Index.</summary>
	[SerializeField] private IntRange _limits; 							/// <summary>Arrows' Limits per round.</summary>
	[SerializeField] private BoundaryWaypointsContainer _waypoints; 	/// <summary>Arrow's Waypoints.</summary>
	[SerializeField] private float[] _projectilesSpawnRates; 			/// <summary>Arrow Projectile's Spawn Rate.</summary>
	[Space(5f)]
	[Header("Devil Scenery's Attributes:")]
	[SerializeField] private Vector3 _ceilingSpawnPoint; 				/// <summary>Ceiling's Spawn Point.</summary>
	[SerializeField] private Vector3 _ceilingDestinyPoint; 				/// <summary>Ceiling's Destiny Point.</summary>
	[SerializeField] private Vector3 _leftTowerSpawnPoint; 				/// <summary>Left Tower's Spawn Point.</summary>
	[SerializeField] private Vector3 _rightTowerSpawnPoint; 			/// <summary>Right Tower's Spawn Point.</summary>
	[SerializeField] private Vector3 _leftTowerDestinyPoint; 			/// <summary>Left Tower's Destiny Point.</summary>
	[SerializeField] private Vector3 _rightTowerDestinyPoint; 			/// <summary>Right Tower's Destiny Point.</summary>
	[SerializeField] private float _towerInterpolationDuration; 		/// <summary>Towers' Fall Duration.</summary>
	[SerializeField] private float _towerShakeDuration; 				/// <summary>Towers' Shake Duration.</summary>
	[SerializeField] private float _towerShakeSpeed; 					/// <summary>Towers' Shake Speed.</summary>
	[SerializeField] private float _towerShakeMagnitude; 				/// <summary>Towers' Shake Megnitude.</summary>
	[SerializeField] private float _towerHP; 							/// <summary>Towers' starting HP.</summary>
	[SerializeField] private float _ceilingHP; 							/// <summary>Ceiling's HP.</summary>
#if UNITY_EDITOR
	[Space(5f)]
	[Header("Gizmos' Attributes:")]
	[SerializeField] private Color gizmosColor; 						/// <summary>Gizmos' Color.</summary>
	[SerializeField] private float gizmosRadius; 						/// <summary>Gizmos' Radius.</summary>
#endif

	/// <summary>Gets arrowProjectileIndex property.</summary>
	public CollectionIndex arrowProjectileIndex { get { return _arrowProjectileIndex; } }

	/// <summary>Gets limits property.</summary>
	public IntRange limits { get { return _limits; } }

	/// <summary>Gets waypoints property.</summary>
	public BoundaryWaypointsContainer waypoints { get { return _waypoints; } }

	/// <summary>Gets projectilesSpawnRates property.</summary>
	public float[] projectilesSpawnRates { get { return _projectilesSpawnRates; } }

	/// <summary>Gets ceilingSpawnPoint property.</summary>
	public Vector3 ceilingSpawnPoint { get { return _ceilingSpawnPoint; } }

	/// <summary>Gets ceilingDestinyPoint property.</summary>
	public Vector3 ceilingDestinyPoint { get { return _ceilingDestinyPoint; } }

	/// <summary>Gets leftTowerSpawnPoint property.</summary>
	public Vector3 leftTowerSpawnPoint { get { return _leftTowerSpawnPoint; } }

	/// <summary>Gets leftTowerDestinyPoint property.</summary>
	public Vector3 leftTowerDestinyPoint { get { return _leftTowerDestinyPoint; } }

	/// <summary>Gets rightTowerSpawnPoint property.</summary>
	public Vector3 rightTowerSpawnPoint { get { return _rightTowerSpawnPoint; } }

	/// <summary>Gets rightTowerDestinyPoint property.</summary>
	public Vector3 rightTowerDestinyPoint { get { return _rightTowerDestinyPoint; } }

	/// <summary>Gets towerInterpolationDuration property.</summary>
	public float towerInterpolationDuration { get { return _towerInterpolationDuration; } }

	/// <summary>Gets towerShakeDuration property.</summary>
	public float towerShakeDuration { get { return _towerShakeDuration; } }

	/// <summary>Gets towerShakeSpeed property.</summary>
	public float towerShakeSpeed { get { return _towerShakeSpeed; } }

	/// <summary>Gets towerShakeMagnitude property.</summary>
	public float towerShakeMagnitude { get { return _towerShakeMagnitude; } }

	/// <summary>Gets towerHP property.</summary>
	public float towerHP { get { return _towerHP; } }

	/// <summary>Gets ceilingHP property.</summary>
	public float ceilingHP { get { return _ceilingHP; } }

	/// <summary>Callback invoked when drawing Gizmos.</summary>
	protected override void OnDrawGizmos()
	{
#if UNITY_EDITOR
		waypoints.OnDrawGizmos();

		Gizmos.color = gizmosColor;

		Gizmos.DrawWireSphere(ceilingSpawnPoint, gizmosRadius);
		Gizmos.DrawWireSphere(ceilingDestinyPoint, gizmosRadius);
		Gizmos.DrawWireSphere(leftTowerSpawnPoint, gizmosRadius);
		Gizmos.DrawWireSphere(rightTowerSpawnPoint, gizmosRadius);
		Gizmos.DrawWireSphere(leftTowerDestinyPoint, gizmosRadius);
		Gizmos.DrawWireSphere(rightTowerDestinyPoint, gizmosRadius);
#endif
	}

	/// <summary>Coroutine's IEnumerator.</summary>
	/// <param name="boss">Object of type T's argument.</param>
	public override IEnumerator Routine(DestinoBoss boss)
	{
		if(!run)
		{
			InvokeCoroutineEnd();
			yield break;
		}
		
		int length = limits.Random();
		int count = 3; // For left and right tower and the ceiling.
		float spawnRate = projectilesSpawnRates[Mathf.Clamp(boss.currentStage, 0, projectilesSpawnRates.Length)];
		float t = 0.0f;
		float inverseDuration = 1.0f / towerInterpolationDuration;
		OnPoolObjectDeactivation onPoolObjectDeactivation = (_poolObject)=> { count--; };
		Health ceiling = DestinoSceneController.Instance.devilCeiling;
		Health leftTower = DestinoSceneController.Instance.leftDevilTower;
		Health rightTower = DestinoSceneController.Instance.rightDevilTower;
		HitCollider2D ceilingHurtBox = ceiling.GetComponentInChildren<HitCollider2D>();
		HitCollider2D leftTowerHurtBox = leftTower.GetComponentInChildren<HitCollider2D>();
		HitCollider2D rightTowerHurtBox = rightTower.GetComponentInChildren<HitCollider2D>();
		SecondsDelayWait wait = new SecondsDelayWait(0.0f);
		OnHealthInstanceEvent onHealthEvent = (_health, _event, _amount)=>
		{
			switch(_event)
			{
				case HealthEvent.FullyDepleted:
				_health.gameObject.SetActive(false);
				count--;

				/// BEGIN Rather quick solution:
				Vector3 destiny = Vector3.zero;

				if(_health == ceiling) destiny = ceilingSpawnPoint;
				if(_health == leftTower) destiny = leftTowerSpawnPoint;
				if(_health == rightTower) destiny = rightTowerSpawnPoint;
				/// END Rather quick solution...

				boss.StartCoroutine(_health.transform.DisplaceToPosition(destiny, towerInterpolationDuration));
				break;
			}
		};

		// Invoke Ceiling & Towers:
		ceiling.gameObject.SetActive(true);
		leftTower.gameObject.SetActive(true);
		rightTower.gameObject.SetActive(true);
		ceiling.transform.position = ceilingSpawnPoint;
		leftTower.transform.position = leftTowerSpawnPoint;
		rightTower.transform.position = rightTowerSpawnPoint;
		ceiling.SetMaxHP(ceilingHP, true);
		leftTower.SetMaxHP(towerHP, true);
		rightTower.SetMaxHP(towerHP, true);
		ceiling.onHealthInstanceEvent -= onHealthEvent;
		leftTower.onHealthInstanceEvent -= onHealthEvent;
		rightTower.onHealthInstanceEvent -= onHealthEvent;
		ceiling.onHealthInstanceEvent += onHealthEvent;
		leftTower.onHealthInstanceEvent += onHealthEvent;
		rightTower.onHealthInstanceEvent += onHealthEvent;

		/// Lerp Ceiling & Towers:
		while(t < 1.0f)
		{
			ceiling.transform.position = Vector3.Lerp(ceilingSpawnPoint, ceilingDestinyPoint, t);
			leftTower.transform.position = Vector3.Lerp(leftTowerSpawnPoint, leftTowerDestinyPoint, t);
			rightTower.transform.position = Vector3.Lerp(rightTowerSpawnPoint, rightTowerDestinyPoint, t);

			t += (Time.deltaTime * inverseDuration);
			yield return null;
		}

		leftTower.transform.position = leftTowerDestinyPoint;
		rightTower.transform.position = rightTowerDestinyPoint;
		ceilingHurtBox.transform.position = ceilingHurtBox.transform.position.WithZ(0.0f);
		leftTowerHurtBox.transform.position = leftTowerHurtBox.transform.position.WithZ(0.0f);
		rightTowerHurtBox.transform.position = rightTowerHurtBox.transform.position.WithZ(0.0f);

		/// Shake Ceiling & Towers when they reach their destiny positions.
		IEnumerator ceilingShake = ceiling.transform.ShakePosition(towerShakeDuration, towerShakeSpeed, towerShakeMagnitude);
		IEnumerator leftTowerShake = leftTower.transform.ShakePosition(towerShakeDuration, towerShakeSpeed, towerShakeMagnitude);
		IEnumerator rightTowerShake = rightTower.transform.ShakePosition(towerShakeDuration, towerShakeSpeed, towerShakeMagnitude);
		
		/// Flash's Test:
		/*FlashWhenReceivingDamage f = leftTower.GetComponent<FlashWhenReceivingDamage>();
		IEnumerator r = f.Routine();
		while(r.MoveNext()) yield return null;*/

		while(ceilingShake.MoveNext()
		|| leftTowerShake.MoveNext()
		|| rightTowerShake.MoveNext()) yield return null;

		/// Invoke Devils' Projectiles
		for(int i = 0; i < length; i++)
		{
			Ray ray = waypoints.GetArrowOriginAndDirection();
			Projectile arrowProjectile = PoolManager.RequestProjectile(Faction.Enemy, arrowProjectileIndex, ray.origin, ray.direction);
			arrowProjectile.transform.rotation = VQuaternion.RightLookRotation(ray.direction);
			/*arrowProjectile.onPoolObjectDeactivation -= onPoolObjectDeactivation;
			arrowProjectile.onPoolObjectDeactivation += onPoolObjectDeactivation;*/

			float cooldownDuration = arrowProjectile.cooldownDuration;

			if(cooldownDuration > 0.0f)
			{
				wait.ChangeDurationAndReset(cooldownDuration);
				while(wait.MoveNext()) yield return null;
			}
			else yield return null;
		}

		while(count > 0) yield return null;

		t = 0.0f;

		while(t < 1.0f)
		{
			leftTower.transform.position = Vector3.Lerp(leftTowerDestinyPoint, leftTowerSpawnPoint, t);
			rightTower.transform.position = Vector3.Lerp(rightTowerDestinyPoint, rightTowerSpawnPoint, t);

			t += (Time.deltaTime * inverseDuration);
			yield return null;
		}

		leftTower.gameObject.SetActive(false);
		rightTower.gameObject.SetActive(false);

		InvokeCoroutineEnd();
	}

	private IEnumerator ReturnSceneryObject()
	{
		float t = 0.0f;
		float inverseDuration = 1.0f / towerInterpolationDuration;

		while(t < 1.0f)
		{
			t += (Time.deltaTime * inverseDuration);
			yield return null;
		}
	}

	/// <summary>Finishes the Routine.</summary>
	/// <param name="boss">Object of type T's argument.</param>
	public override void FinishRoutine(DestinoBoss boss)
	{

	}
}
}