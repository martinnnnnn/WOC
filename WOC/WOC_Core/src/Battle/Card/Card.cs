using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WOC_Core
{

    //public struct PlayInfo
    //{
    //    public BattlePlayer Owner;
    //    public ICombatant Target;

    //    public void Deconstruct(out BattlePlayer owner, out ICombatant target) => (owner, target) = (Owner, Target);
    //}

    public class Card
    {
        public string name = "";
        public int timeCost = 0;
        public List<CardEffect> effects = new List<CardEffect>();

        public Card()
        {
        }

        public Card(Card template)
        {
            name = template.name;
            effects = new List<CardEffect>(template.effects);
            effects.ForEach(e => e.owner = this);
        }

        //public bool Play(PlayInfo info)
        //{
        //    bool result = false;
        //    foreach (var effect in effects)
        //    {
        //        if (effect.Play(info))
        //        {
        //            result = true;
        //        }
        //    }
        //    return result;
        //}
    }
}