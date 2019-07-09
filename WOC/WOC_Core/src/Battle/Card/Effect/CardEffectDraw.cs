using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json;
using System.Xml.Linq;
using System;

namespace WOC_Core
{
    public class CardEffectDraw : CardEffect
    {
        public int value = 0;

        public CardEffectDraw(int value)
        {
            this.value = value;
        }

        public override bool Play(PlayInfo info)
        {
            (Actor owner, _) = info;

            if (owner != null && owner is PlayerActor)
            {
                PlayerActor playerOwner = owner as PlayerActor;
                playerOwner.DrawCards(value);
                return true;
            }
            return false;
        }
    }
}