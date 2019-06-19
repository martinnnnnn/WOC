using System;
using System.Collections;
using System.Collections.Generic;

namespace WOC_Battle
{
    public class Initiative
    {
        public int Value;
        int max;

        /// <summary>
        /// Initiative's value is set in regards to the number of cards a players has
        /// and an arbitrary maximum possible value of initiative
        /// </summary>
        /// <param name="cardCount"></param>
        public void Set(int cardCount, int maxInitiative)
        {
            max = maxInitiative;
            Value = (int)(1.0f / (float)cardCount * max);
        }
    }
}