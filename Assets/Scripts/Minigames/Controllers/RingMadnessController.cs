using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Voidless;

namespace Flamingo
{
    public class RingMadnessController : MonoBehaviour
    {
        [SerializeField] private RingMadness_Minigame _ringMiniGame;
        

        // Start is called before the first frame update
        public RingMadness_Minigame ringMinigame { get { return _ringMiniGame; } }
        private void Awake()
        {
            ringMinigame.Initialize(this);
        }
        void Start()
        {
            
        }

        // Update is called once per frame
        void Update()
        {

        }

        
    }
}
