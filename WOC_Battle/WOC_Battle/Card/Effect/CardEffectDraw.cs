using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json;
using System.Xml.Linq;
using System;

namespace WOC_Battle
{
    public class CardEffectDraw : CardEffect
    {
        public int value = 0;

        public override bool Play(PlayInfo info)
        {
            (Actor owner, _) = info;

            if (owner != null)
            {
                owner.DrawCards(value);
                return true;
            }
            return false;
        }
    }
}