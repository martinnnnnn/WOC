﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WOC_Battle
{
    public class PNJActor : Actor
    {

        public PNJActor(
            Battle battle,
            Character character,
            string name,
            int initialInitiative,
            int maxInitiative) : base(battle, character, name)
        {
            initiative.Set(initialInitiative, maxInitiative);
        }

        public override void BattleInit()
        {
            base.BattleInit();
        }

        public override void BattleEnd()
        {
            base.BattleEnd();
        }

        public override void StartTurn()
        {
            base.StartTurn();
        }

        public override void EndTurn()
        {
            base.EndTurn();
        }
    }
}
