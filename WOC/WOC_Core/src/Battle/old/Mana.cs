//using System;
//using System.Collections;
//using System.Collections.Generic;
//using System.Diagnostics;

//namespace WOC_Core
//{
//    public class Mana
//    {
//        public int Value;

//        public int Max;
        
//        public void Reset()
//        {
//            Value = Max;
//        }

//        public void Consume(int amount)
//        {
//            Debug.Assert(Value >= amount, "You don't have the mana for this !");
//            if (Value >= amount)
//            {
//                Value -= amount;
//            }
//        }

//        public void Fill(int amount)
//        {
//            Value = Math.Min(Value + amount, Max);
//        }


//    }
//}