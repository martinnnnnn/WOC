using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json;
using System.Xml.Linq;
using System;

namespace WOC_Battle
{
    public class CardEffectDiscard : CardEffect
    {
        public int value = 0;

        public override bool Play(PlayInfo info)
        {
            (Actor playOwner, _) = info;

            if (playOwner != null)
            {
                playOwner.DiscardRandomCards(value, owner);
                return true;
            }
            return false;
        }
    }

}