using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WOC_Battle
{
    public class Actor
    {
        public string Name;
        protected Battle battle;
        public Initiative initiative = new Initiative();
        public Character character = new Character();

        public Actor(Battle battle, Character character, int life)
        {
            this.battle = battle;
            this.character = character;
            this.character.Life = life;
            this.character.OnDeath += battle.OnCharacterDeath;
        }

        public virtual void BattleInit()
        {
        }

        public virtual void BattleEnd()
        {

        }

        public virtual void StartTurn()
        {
        }

        public virtual void EndTurn()
        {
        }
    }
}
