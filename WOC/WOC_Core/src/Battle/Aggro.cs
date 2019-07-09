using System;
using System.Collections;
using System.Collections.Generic;


namespace WOC_Core
{
    public class Aggro
    {
        private float max;

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

        public float Value => total;

        public float IncrementRatio;

        public void Reset()
        {
            turnCardPlayedCount = 0;
            current = 0;
            total = 0;
        }

        public void StartTurn()
        {
            turnCardPlayedCount = 0;
            current = 1;
        }


        public void Increment()
        {
            total = Math.Min(total + current, max);
            turnCardPlayedCount++;
            current += IncrementRatio * turnCardPlayedCount;
        }
    }
}