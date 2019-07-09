using System.Collections;
using System.Collections.Generic;
using System;
using System.Security.Cryptography;
using System.Xml.Linq;


namespace WOC_Core
{
    public class CardEffect
    {
        public Card owner = null;

        public CardEffect()
        {
        }
        public virtual bool Play(PlayInfo info) { return false; }
    }
}