using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Voidless;
using System;


namespace Flamingo
{
   
[Serializable]    
public class BasketBallMiniGame : MiniGame
{

        [SerializeField] private Ring blueBasket;                                /// <summary>Basket for blue player.</summary>
        [SerializeField]private Ring redBasket;                                 /// <summary>Basket for red player.</summary>
        private int blueScore;                                                  /// <summary>Blue player score.</summary>
        private int redScore;                                                   /// <summary>Red player score.</summary>
        [Header("Time Variables")]
        [SerializeField] public Clock basketClock;                              /// <summary>Clock class for the minigame.</summary>
        [SerializeField] private float _limitMinutesAmount;                     /// <summary>Limit time for the minigame.</summary>

        [Header("UI")]
        [SerializeField] private TextMesh _timeText;                            /// <summary>Text mesh to show the time left of the minigame.</summary>
        [SerializeField] private TextMesh _blueScoreText;                       /// <summary>Text mesh to show the score for blue player.</summary>
        [SerializeField] private TextMesh _redScoreText;                        /// <summary>Text mesh to show the score for red player.</summary>
        private string _winnerName;



#region Set/Get


        /// <summary>Gets and Sets BlueBasket property.</summary>
        public Ring BlueBasket
        {
            get { return blueBasket; }
            set { blueBasket = value; }
        }
        /// <summary>Gets and Sets RedBasket property.</summary>
        public Ring RedBasket
        {
            get { return redBasket; }
            set { redBasket = value; }
        }
        /// <summary>Gets and Sets BlueScore property.</summary>
        public int BlueScore
        {
            get { return blueScore; }
            set {blueScore = value; }
        }
        /// <summary>Gets and Sets RedScore property.</summary>
        public int RedScore
        {
            get { return redScore; }
            set { redScore = value; }
        }
        /// <summary>Gets and Sets limitMinutes for minigame property.</summary>
        public float limitMinutesAmount
        {
            get { return _limitMinutesAmount; }
            set { _limitMinutesAmount = value; }
        }
        /// <summary>Gets and Sets Timer Text property.</summary>
        public TextMesh timeText
        {
            set { _timeText = value; }
            get { return _timeText; }
        }
        /// <summary>Gets and Sets blueScore text property.</summary>
        public TextMesh blueScoreText
        {
            set { _blueScoreText = value; }
            get { return _blueScoreText; }
        }
        /// <summary>Gets and Sets redScore text property.</summary>
        public TextMesh redScoreText
        {
            set { _redScoreText = value; }
            get { return _redScoreText; }
        }
#endregion



#region Events Callbacks  
        /// <summary>Callback for onRingPassed's Event if its the red basket.</summary>
        /// <param name="_collider">Collider that passed the ring.</param>
        public void OnRedRingPassed(Collider2D _collider)
        {
           
            blueScore++;
            blueScoreText.text = blueScore.ToString();
            Debug.Log("BLUE MAKES A SCORE BABYYY : " + blueScore);

        }
        /// <summary>Callback for onRingPassed's Event if its the blue basket.</summary>
        /// <param name="_collider">Collider that passed the ring.</param>
        public void OnBlueRingPassed(Collider2D _collider)
        {
            redScore++;
            redScoreText.text = redScore.ToString();
            Debug.Log("RED MAKES A SCORE BABYYY : " + redScore);

        }
#endregion

        /// <summary>Basket ball Mini-Game's Coroutine.</summary>
        
        protected override IEnumerator MiniGameCoroutine()
        {
            redBasket.onRingPassed += OnRedRingPassed;
            blueBasket.onRingPassed += OnBlueRingPassed;
      
            while (basketClock.minutes < limitMinutesAmount)
            {
                yield return new WaitForEndOfFrame();
                basketClock.Update(Time.deltaTime);
                timeText.text = basketClock.ToString();
            }    
            timeText.text = "Game Over \n" + ReturnWinner()  ;


            //Desuscribirse
            redBasket.onRingPassed -= OnRedRingPassed;
            blueBasket.onRingPassed -= OnBlueRingPassed;
           

            yield return null;
           
        }

   
private string ReturnWinner()
        {
            if(redScore>blueScore)
            {
                _winnerName = "Red player wins!";
            }
            else if(blueScore>redScore)
            {
                _winnerName = "Blue player wins!";
            }
            else if (redScore == blueScore)
            {
                _winnerName = "TIE!";
            }
            return _winnerName;
        }

    }

}
