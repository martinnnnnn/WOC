using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json;
using System.Xml.Linq;
using System;

namespace WOC_Battle
{
    public class CardEffectDamage : CardEffect
    {
        public int value = 0;

        public override bool Play(PlayInfo info)
        {
            (Actor owner, Character target) = info;

            if (target != null && owner != target.Owner)
            {
                target.ChangeLife(-value);
                return true;
            }
            return false;
        }
    }
}