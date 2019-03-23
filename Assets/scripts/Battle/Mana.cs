using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace WOC
{
    public class Mana : MonoBehaviour
    {
        public int max;
        public StatusBar statusBar;

        int value;

        public void Reset()
        {
            value = max;
            statusBar.Set((float)value / (float)max);
        }


        public void Consume(int amount)
        {
            value = Math.Max(value - amount, 0);
            statusBar.Set((float)value / (float)max);
        }

        public void Fill(int amount)
        {
            value = Math.Min(value + amount, max);
            statusBar.Set((float)value / (float)max);
        }


    }
}