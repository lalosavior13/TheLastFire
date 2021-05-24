using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Voidless;

namespace Flamingo
{
[CreateAssetMenu]
public class GameData : ScriptableObject
{
	public const string PATH_SCENE_TOLOAD = "SceneToLoad"; 						/// <summary>Scene to Load's Path on the Player Prefs [or default].</summary>
	public const string PATH_SCENE_DEFAULT = "Scene_LoadingScreen"; 			/// <summary>Default Scene to Load's Path.</summary>
	public const string PATH_SCENE_LOADING = "Scene_LoadingScreen"; 			/// <summary>Loading Scene's Path.</summary>

#region Properties:
	[Header("Configurations:")]
	[SerializeField] [Range(0, 60)] private int _frameRate; 					/// <summary>Game's Frame rate.</summary>
	[Space(5f)]
	[Header("Projectiles:")]
	[SerializeField] private Projectile[] _playerProjectiles; 					/// <summary>Player's Projectiles.</summary>
	[SerializeField] private Projectile[] _enemyProjectiles; 					/// <summary>Enemy's Projectiles.</summary>
	[SerializeField] private HomingProjectile[] _enemyHomingProjectiles; 		/// <summary>Enemy's Homing Projectiles.</summary>
	[SerializeField] private ParabolaProjectile[] _enemyParabolaProjectiles; 	/// <summary>Enemy's Parabola Projectiles.</summary>
	[SerializeField] private PoolGameObject[] _poolObjects; 					/// <summary>Pool GameObjects.</summary>
	[Space(5f)]
	[Header("Audios:")]
	[SerializeField] private FiniteStateAudioClip[] _FSMLoops; 					/// <summary>Finite-State's Loop Effects.</summary>
	[SerializeField] private AudioClip[] _loops; 								/// <summary>Loop Effects.</summary>
	[SerializeField] private AudioClip[] _soundEffects; 						/// <summary>Sounds' Effects.</summary>
	[Space(5f)]
	[Header("Particle Effects:")]
	[SerializeField] private ParticleEffect[] _particleEffects; 				/// <summary>Particle Effects.</summary>
#endregion
	private float _idealDeltaTime; 												/// <summary>Ideal delta time.</summary>

#region Getters:
	/// <summary>Gets frameRate property.</summary>
	public int frameRate { get { return _frameRate; } }

	/// <summary>Gets idealDeltaTime property.</summary>
	public float idealDeltaTime
	{
		get
		{
			if(_idealDeltaTime == 0.0f) _idealDeltaTime = 1.0f / (frameRate > 0 ? (float)frameRate : 60.0f);
			return _idealDeltaTime;
		}
	}

	/// <summary>Gets playerProjectiles property.</summary>
	public Projectile[] playerProjectiles { get { return _playerProjectiles; } }

	/// <summary>Gets enemyProjectiles property.</summary>
	public Projectile[] enemyProjectiles { get { return _enemyProjectiles; } }

	/// <summary>Gets enemyHomingProjectiles property.</summary>
	public HomingProjectile[] enemyHomingProjectiles { get { return _enemyHomingProjectiles; } }

	/// <summary>Gets enemyParabolaProjectiles property.</summary>
	public ParabolaProjectile[] enemyParabolaProjectiles { get { return _enemyParabolaProjectiles; } }

	/// <summary>Gets poolObjects property.</summary>
	public PoolGameObject[] poolObjects { get { return _poolObjects; } }

	/// <summary>Gets FSMLoops property.</summary>
	public FiniteStateAudioClip[] FSMLoops { get { return _FSMLoops; } }

	/// <summary>Gets loops property.</summary>
	public AudioClip[] loops { get { return _loops; } }

	/// <summary>Gets soundEffects property.</summary>
	public AudioClip[] soundEffects { get { return _soundEffects; } }

	/// <summary>Gets particleEffects property.</summary>
	public ParticleEffect[] particleEffects { get { return _particleEffects; } }
#endregion

	/// <summary>Initializes Game's Data.</summary>
	public void Initialize()
	{
		Application.targetFrameRate = frameRate;
	}
}
}