using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


namespace Flamingo {

    public class BaskatBallMiniGameController : MonoBehaviour
    {
        [SerializeField] private BasketBallMiniGame _basketMiniGame;                    /// <summary> Reference to BasketMiniGame that initialize the minigame       
        [SerializeField] private Vector3 _ballInitialPos;                               /// <summary>Spawn Position to reset the ball on ring passed.</summary>
        [SerializeField] private GameObject _ball;                                      /// <summary>References to the ball.</summary>



        #region Getters/Setters

        /// <summary>Gets and Sets basketMiniGame property.</summary>
        public BasketBallMiniGame basketMiniGame { get { return _basketMiniGame; } }
        
        /// <summary>Get and Sets ballInitialPos property.</summary>
        public Vector3 ballInitialPos
        {
            set { _ballInitialPos = value; }
            get { return _ballInitialPos; }
        }

        /// <summary>Get and Set ball property.</summary>
        public GameObject ball
        {
            set { _ball = value; }
            get { return _ball; }
        }


#endregion

        private void OnDrawGizmos()
        {
            Gizmos.color = Color.blue;
            Gizmos.DrawSphere(ballInitialPos, .5f);
        }
#region Unity Calls
        private void Awake()
        {
            basketMiniGame.Initialize(this);
            
            
            SubscribeBall();
        }

        private void OnDestroy()
        {
            basketMiniGame.Terminate(this);
            Unsubscribe();   
        }
        #endregion

        #region Functions

        /// <summary>Function to subscribe ResetBall event to rings.</summary>
        void SubscribeBall()
        {

            basketMiniGame.RedBasket.onRingPassed += ResetBallOnRingPassed;
            basketMiniGame.BlueBasket.onRingPassed += ResetBallOnRingPassed;
        }

        void Unsubscribe()
        {
           /* basketMiniGame.RedBasket.onRingPassed -= basketMiniGame.OnRedRingPassed;
            basketMiniGame.BlueBasket.onRingPassed -= basketMiniGame.OnBlueRingPassed;*/
            basketMiniGame.RedBasket.onRingPassed -= ResetBallOnRingPassed;
            basketMiniGame.BlueBasket.onRingPassed -= ResetBallOnRingPassed;
        }

        /// <summary>Override delegate for OnRingedPassed .</summary>
        public void ResetBallOnRingPassed(Collider2D _collider)
        {
            ball.GetComponent<Rigidbody2D>().Sleep();
            ball.transform.position = ballInitialPos;
        }

      
#endregion

    }

}
