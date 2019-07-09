using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json;
using System.Xml.Linq;
using System;

namespace WOC_Core
{
    public class CardEffectDiscard : CardEffect
    {
        public int value = 0;
                
        public CardEffectDiscard(int value)
        {
            this.value = value;
        }
        public override bool Play(PlayInfo info)
        {
            (Actor owner, _) = info;

            if (owner != null && owner is PlayerActor)
            {
                PlayerActor playerOwner = owner as PlayerActor;
                playerOwner.DiscardRandomCards(value, this.owner);
                return true;
            }
            return false;
        }
    }

}