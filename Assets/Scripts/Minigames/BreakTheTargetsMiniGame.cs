using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Voidless;
using System;

using Random = UnityEngine.Random;

namespace Flamingo
{
    [Serializable]
    public class BreakTheTargetsMiniGame : MiniGame
    {
        public Clock clock;
        Destroy destroy;
        [SerializeField] private CollectionIndex impactParticleEffectIndex;
        [SerializeField] private CollectionIndex targetIndex;
        [SerializeField] public List<PoolGameObject> targetList = new List<PoolGameObject>(); 	/// <summary>Target's Index.</summary>
        [SerializeField] private int _targetAmount;
        [SerializeField] private float _overlapRadius;
        [SerializeField] private LayerMask _targetMask;
        [SerializeField] private float _nextSpawn;  /// <summary>Next spawn of target</summary>
        [SerializeField] private float _spawnRate;  /// <summary>How often can it appear </summary>
        [SerializeField] private float _endGame;
        [SerializeField] private TextMesh _timerTxt;


        private Vector3 SpawnPoint;   /// <summary>Spawn's Points.</summary>
        private float _new_x; /// <summary>Position of target in X  /// </summary>
        private float _new_y; /// <summary>Position of target in Y  /// </summary>
        private float _new_z = -1f; /// <summary>Position of target in Z  /// </summary>
        private float _score; /// <summary>Score of how many targets have been destroyed  /// </summary>



        #region Set&Get

        public float nextSpawn
        {
            get { return _nextSpawn; }
            set { _nextSpawn = value; }
        }

        public float spawnRate
        {
            get { return _spawnRate; }
            set { _spawnRate = value; }
        }

        public float endGame
        {
            get { return _endGame; }
            set { _endGame = value; }
        }

        public TextMesh timerTxt
        {
            get { return _timerTxt; }
            set { timerTxt = value; }
        }

        public float new_x
        {
            get { return _new_x; }
            set { _new_x = value; }
        }

        public float new_y
        {
            get { return _new_y; }
            set { _new_y = value; }
        }

        public float new_z { get { return _new_z; } }

        public float score
        {
            get { return _score; }
            set { _score = value; }
        }

        public int targetAmount
        {
            get { return _targetAmount; }
            set { _targetAmount = value; }
        }

        public float overlapRadius
        {
            get { return _overlapRadius; }
            set { _overlapRadius = value; }
        }

        public LayerMask targetMask
        {
            get { return _targetMask; }
            set { _targetMask = value; }
        }

        #endregion

        /// <summary>Instantiate the target at a specific position on the map./// </summary>
        public void RandomTargets()
        {
            if (Time.time > nextSpawn)
            {
                nextSpawn = Time.time + spawnRate;


                do
                {
                    new_x = Random.Range(99, 117);
                    new_y = Random.Range(-1, 5.5f);
                    SpawnPoint = new Vector3(new_x, new_y, new_z);


                    if (Physics2D.OverlapCircle(new Vector2(new_x, new_y), overlapRadius, targetMask))
                    {
                        Debug.Log("Overlaping");
                        continue;
                    }
                    else
                    {
                        PoolGameObject TempTarget = PoolManager.RequestPoolGameObject(targetIndex, SpawnPoint, Quaternion.identity);
                        TempTarget.onPoolObjectDeactivation += OnTargetDestroyed;

                        targetList.Add(TempTarget);
                    }

                } while (targetList.Count < targetAmount);


            }



        }

        void OnTargetDestroyed(IPoolObject poolObject)
        {
            
            PoolGameObject target2 = poolObject as PoolGameObject;
          
            poolObject.onPoolObjectDeactivation -= OnTargetDestroyed;
        
         //   PoolManager.RequestParticleEffect(impactParticleEffectIndex, SpawnPoint, Quaternion.identity);
           
            score++;
        }


      

        protected override IEnumerator MiniGameCoroutine()
        {
            while (score < endGame)
            {
                yield return new WaitForEndOfFrame();
                clock.Update(Time.deltaTime);
                RandomTargets();

                timerTxt.text = clock.ToString();
            }


            yield return null;
        }



    }
}