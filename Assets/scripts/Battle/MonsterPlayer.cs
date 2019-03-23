using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace WOC
{
    public enum MonsterPattern
    {
        ATTACK,
        HEAL,
    }

    public class MonsterPlayer : BasePlayer
    {
        public MonsterPattern[] patterns;
        public string title;
        public int healAmount;
        public int health;
        public int attack;
        int healthCurrent = 0;

        Battle battle;
        int current = 0;
        Animator animator;

        public override void BattleInit()
        {
            base.BattleInit();
            battle = FindObjectOfType<Battle>();
            animator = GetComponentInChildren<Animator>();
        }

        public override void StartTurn()
        {
            base.BattleInit();

        }

        bool hasPlayedTurn = false;
        public override void PlayTurn()
        {
            base.BattleInit();
            if (!hasPlayedTurn)
            {
                hasPlayedTurn = true;
                StartCoroutine(PlayTurnRoutine());
            }

        }

        public override void EndTurn()
        {
            base.BattleInit();
        }

        public override void OnAggroChange(Character newBiggestAggro)
        {
            base.BattleInit();
            transform.LookAt(newBiggestAggro.transform);
        }

        IEnumerator PlayTurnRoutine()
        {
            switch (patterns[current])
            {
                case MonsterPattern.ATTACK:
                    Character target = battle.GetBiggestAggro().character;
                    animator.SetTrigger("Attack");
                    target.ChangeLife(-attack);
                    break;
                case MonsterPattern.HEAL:
                    animator.SetTrigger("Heal");
                    healthCurrent = Mathf.Min(healthCurrent + healAmount, health);
                    break;
            }
            current = (current + 1) % patterns.Length;
            yield return new WaitForSeconds(Utils.GetClipDuration(animator, "Armature|TRex_Attack"));
            hasPlayedTurn = false;
            battle.OnEndButton();
            yield return null;
        }
    }
}