using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WOC_Battle
{
    public class Actor
    {
        public Initiative initiative = new Initiative();

        public static Actor ActorFactory()
        {
            
        }

        public Actor()
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
        }

        public virtual void EndTurn()
        {
        }
    }
}
