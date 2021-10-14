using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Voidless;

namespace Flamingo
{
    [RequireComponent(typeof(ImpactEventHandler))]
    public class Destroy : PoolGameObject
    {
        BreakTheTargetsController btm;

        [SerializeField] private GameObjectTag[] _impactTags; 	/// <summary>Impacts' Tags.</summary>
        private ImpactEventHandler _impactHandler;


        /// <summary>Gets impactHandler Component.</summary>
        public ImpactEventHandler impactHandler
        {
            get
            {
                if (_impactHandler == null) _impactHandler = GetComponent<ImpactEventHandler>();
                return _impactHandler;
            }
        }
        void Start()
        {
            btm = FindObjectOfType<BreakTheTargetsController>();
            StartCoroutine(DestroyObj());
        }

      
        IEnumerator DestroyObj()
        {
            yield return new WaitForSeconds(1.8f);
           // btm.ClearList();
            impactHandler.eventsHandler.InvokeDeactivationEvent(DeactivationCause.Destroyed);
           
            OnObjectDeactivation();


          //  return;
          //  Destroy(this.gameObject);
        }
 
    }

}