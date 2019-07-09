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
        protected Battle battle;
        public Initiative initiative = new Initiative();
        public Character character;


        public TurnState turnState = TurnState.NOT_MINE;

        public Actor(Battle battle, Character character, string name)
        {
            this.battle = battle;
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

        public virtual void StartTurn()
        {
            turnState = TurnState.MINE;
        }

        public virtual void EndTurn()
        {
            turnState = TurnState.NOT_MINE;
        }
    }
}
