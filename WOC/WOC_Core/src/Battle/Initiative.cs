using System;
using System.Collections;
using System.Collections.Generic;

namespace WOC_Core
{
    public class Initiative
    {
        public int Value;
        public static int Max = 50;

        /// <summary>
        /// Initiative's value is set in regards to the number of cards a players has
        /// and an arbitrary maximum possible value of initiative
        /// </summary>
        /// <param name="cardCount"></param>
        public void Set(int value)
        {
            Value = (int)((1.0f / (float)value) * Max);
        }
    }
}