using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace WOC
{
    public enum EnemyPattern
    {
        ATTACK,
        HEAL,
    }

    public class EnemyAI : MonoBehaviour
    {
        public EnemyPattern[] patterns;
        public string name;
        public int healAmount;
        public int health;
        public int attack;
        int healthCurrent = 0;

        Battle battle;
        int current = 0;


        private void Start()
        {
            battle = FindObjectOfType<Battle>();
        }

        public void PlayTurn()
        {
            switch (patterns[current])
            {
                case EnemyPattern.ATTACK:
                    battle.GetBiggestAggro().character.ChangeLife(-attack);
                    break;
                case EnemyPattern.HEAL:
                    healthCurrent = Mathf.Min(healthCurrent + healAmount, health);
                    break;
            }
            current = (current + 1) % patterns.Length;
        }
    }
}