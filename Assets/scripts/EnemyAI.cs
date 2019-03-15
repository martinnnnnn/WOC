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

    public class EnemyAI : Fighter
    {
        public EnemyPattern[] patterns;
        public string name;
        public int healAmount;
        public int health;
        public int attack;
        int healthCurrent = 0;

        Battle battle;
        int current = 0;
        Animator animator;

        private void Start()
        {
            battle = FindObjectOfType<Battle>();
            animator = GetComponentInChildren<Animator>();
        }

        public override void PlayTurn()
        {
            switch (patterns[current])
            {
                case EnemyPattern.ATTACK:
                    Character target = battle.GetBiggestAggro().character;
                    transform.LookAt(target.transform);
                    animator.SetTrigger("Attack");
                    target.ChangeLife(-attack);
                    break;
                case EnemyPattern.HEAL:
                    animator.SetTrigger("Heal");
                    healthCurrent = Mathf.Min(healthCurrent + healAmount, health);
                    break;
            }
            current = (current + 1) % patterns.Length;
            battle.OnEndButton();
        }

        //IEnumerator Attack()
        //{
        //    Character target = battle.GetBiggestAggro().character;
        //    transform.LookAt(target.transform);
        //    animator.SetTrigger("Attack");
        //    target.ChangeLife(-attack);
        //    yield return WaitForAnimation()
        //}
    }
}