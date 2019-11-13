using System;
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
        Battle battle;

        public Battle Battle
        {
            get
            {
                return battle;
            }
            set
            {
                battle = value;
                this.character.OnDeath += battle.OnCharacterDeath;
            }
        }
        public Initiative initiative = new Initiative();

        private Character character;
        public Character Chara
        {
            get
            {
                return character;
            }
            set
            {
                character = value;
                character.Owner = this;
            }
        }


        public TurnState turnState = TurnState.NOT_MINE;

        public Actor(string name)
        {
            Name = name;
        }

        public Actor(Character character, string name)
        {
            Chara = character;
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
                Console.WriteLine("[ACTOR] {0}({1}) turn starts", Name, character.Name);
                turnState = TurnState.MINE;
                return true;
            }
            return false;
        }

        public virtual bool EndTurn()
        {
            if (turnState == TurnState.MINE)
            {
                Console.WriteLine("[ACTOR] {0} turn ends", Name);
                turnState = TurnState.NOT_MINE;
                return true;
            }
            return false;
        }
    }
}
