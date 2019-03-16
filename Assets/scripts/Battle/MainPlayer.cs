using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace WOC
{
    [RequireComponent(typeof(Aggro))]
    public class MainPlayer : BasePlayer
    {
        Aggro aggro;
        Battle battle;

        private void Start()
        {
            battle = FindObjectOfType<Battle>();
            aggro = GetComponent<Aggro>();
        }

        public override void BattleInit()
        {
            aggro.Reset();
        }

        public override void StartTurn()
        {

        }

        public override void PlayTurn()
        {

        }

        public override void EndTurn()
        {

        }
    }
}