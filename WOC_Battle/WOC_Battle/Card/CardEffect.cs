using System.Collections;
using System.Collections.Generic;
using System;
using System.Security.Cryptography;
using System.Xml.Linq;


namespace WOC_Battle
{
    public class CardEffect
    {
        public Card owner;
        public virtual bool Play(PlayInfo info) { return false; }
        public override string ToString() { return ""; }
    }
}