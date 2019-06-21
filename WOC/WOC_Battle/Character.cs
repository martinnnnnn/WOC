using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WOC_Battle
{
    public class Character
    {
        public string Name;
        public float Life;
        public float MaxLife;
        public Actor Owner;

        public Action<Character> OnDeath;

        public Character(string name, float life, float maxLife)
        {
            Name = name;
            Life = life;
            MaxLife = life;
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
