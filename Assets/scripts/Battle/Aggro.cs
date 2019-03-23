using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace WOC
{
    public class Aggro : MonoBehaviour
    {
        [SerializeField] private float max;
        [SerializeField] private float incrementRatio;
        public StatusBar aggroBar;

        /// <summary>
        /// number of cards played this turn
        /// </summary>
        private int turnCardPlayedCount;
        /// <summary>
        /// value incremented and added to the total aggro for each card played
        /// </summary>
        private float current;
        /// <summary>
        /// total player aggro
        /// </summary>
        private float total;

        [HideInInspector]
        public float Value => total;

        public void Reset()
        {
            turnCardPlayedCount = 0;
            current = 0;
            total = 0;
            aggroBar.Set(0);
        }

        public void StartTurn()
        {
            turnCardPlayedCount = 0;
            current = 1;
        }


        public void Increment()
        {
            total = Math.Min(total + current, max);
            aggroBar.Set(total / max);

            turnCardPlayedCount++;
            current += incrementRatio * turnCardPlayedCount;
        }

        public void ChangeTotal(float value)
        {
        }

        //public void EndTurn()
        //{
        //    turnCardPlayedCount = 0;
        //    current = 0;
        //}
    }
}