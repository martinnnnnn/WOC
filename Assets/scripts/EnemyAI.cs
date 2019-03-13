using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace WOC
{
    public enum Pattern
    {
        ATTACK,
        HEAL,
    }

    public class EnemyAI : MonoBehaviour
    {
        public Pattern[] patterns;
        public string name;

        Battle battle;
        int current = 0;


        private void Start()
        {
            
        }

        public void PlayTurn()
        {


        }
    }
}