using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WOC_Core
{
    public class Character
    {
        public enum Race
        {
            HUMAN,
            OGRE,
            ELFE,
        }

        public enum Category
        {
            BARBARIAN,
            DRUID,
            CHAMAN,
            PALADIN,
            SORCERER
        }

        public string Name;
        public Race race;
        public Category category;
        public Actor Owner;
        public float Life;
        public float MaxLife;

        public Action<Character> OnDeath;

        public Character(Race race, Category category, float life, string name = "")
        {
            Life = life;
            MaxLife = life;
            this.race = race;
            this.category = category;
            Name = (name == "") ? category.ToString() + " the " + race.ToString() : name;
        }

        public void ChangeLife(float value)
        {
            Life += value;
            if (Life <= 0)
            {
                OnDeath(this);
            }
        }
    }
}
