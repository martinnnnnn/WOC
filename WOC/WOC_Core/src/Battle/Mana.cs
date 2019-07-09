using System;
using System.Collections;
using System.Collections.Generic;

namespace WOC_Core
{
    public class Mana
    {
        private int max;

        int value;

        public int Max { get => max; set => max = value; }
        
        public void Reset()
        {
            value = max;
        }

        public bool Consume(int amount)
        {
            if (value >= amount)
            {
                value -= amount;
            }
            return false;
        }

        public void Fill(int amount)
        {
            value = Math.Min(value + amount, max);
        }


    }
}