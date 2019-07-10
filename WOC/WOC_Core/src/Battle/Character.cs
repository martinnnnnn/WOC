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

        private float life;

        public string Name;
        public Race race;
        public Category category;
        public Actor Owner;
        public float Life { get => life; }
        public float MaxLife;

        public Action<Character> OnDeath;

        public Character(Race race, Category category, float life, string name = "")
        {
            this.life = life;
            MaxLife = life;
            this.race = race;
            this.category = category;
            Name = (name == "") ? category.ToString() + " the " + race.ToString() : name;
        }

        public void ChangeLife(float value)
        {
            life += value;
            if (Life <= 0)
            {
                OnDeath(this);
            }
        }

        public void SetLife(float value)
        {
            life = value;
            if (Life <= 0)
            {
                OnDeath(this);
            }
        }
    }
}
