﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Voidless;

namespace Flamingo {
    public class BreakTheTargetsController : MonoBehaviour
    {
        [SerializeField] private BreakTheTargetsMiniGame _breakTheTargetsMiniGame;
        public BreakTheTargetsMiniGame breakTheTargetsMiniGame { get { return _breakTheTargetsMiniGame; } }

        private void Start()
        {

        }
        void Awake()
        {
            breakTheTargetsMiniGame.Initialize(this, MiniGameEvent);
        }


        public void MiniGameEvent(MiniGame miniGame, int ID)
        {
            Debug.Log("Initialize");

        }

        public void OnDestroy()
        {
            breakTheTargetsMiniGame.Terminate(this);

        }

        public void EndMiniGameBtt()
        {
            breakTheTargetsMiniGame.Terminate(this);
        }


      
        /*
        public void ClearList (){
            breakTheTargetsMiniGame.targetList.Clear();      
        }*/


    }



}
