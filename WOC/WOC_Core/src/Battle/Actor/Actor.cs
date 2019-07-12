﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WOC_Core
{
    public class Actor
    {
        public enum TurnState
        {
            NOT_MINE,
            MINE
        }

        public string Name;
        public Battle battle;
        public Initiative initiative = new Initiative();
        public Character character;


        public TurnState turnState = TurnState.NOT_MINE;

        public Actor(Character character, string name)
        {
            this.character = character;
            character.Owner = this;
            this.character.OnDeath += battle.OnCharacterDeath;
            Name = name;
        }

        public virtual void UpdateInitiative()
        {
        }

        public virtual void BattleInit()
        {
        }

        public virtual void BattleEnd()
        {
        }

        public virtual bool StartTurn()
        {
            if (turnState == TurnState.NOT_MINE)
            {
                LOG.Print("[ACTOR] {0}({1}) turn starts", Name, character.Name);
                turnState = TurnState.MINE;
                return true;
            }
            return false;
        }

        public virtual bool EndTurn()
        {
            if (turnState == TurnState.MINE)
            {
                LOG.Print("[ACTOR] {0} turn ends", Name);
                turnState = TurnState.NOT_MINE;
                return true;
            }
            return false;
        }
    }
}
