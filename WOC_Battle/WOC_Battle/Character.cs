using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WOC_Battle
{
    public class Character
    {
        public float Life;
        public float MaxLife;
        public Actor Owner;

        public Action OnDeath;

        public void ChangeLife(float value)
        {
            Life += value;
            if (Life <= 0)
            {
                OnDeath();
            }
        }
    }
}
