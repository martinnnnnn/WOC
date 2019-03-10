using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;
using DG.Tweening;
using System.Xml.Linq;
using System;

namespace WOC
{
    public class CardEffectDiscard : CardEffect
    {
        public int value = 0;
        public string display = "";

        public override bool Play(PlayInfo parameters)
        {
            if (parameters.owner)
            {
                parameters.owner.DiscardRandomCards(value, GetComponent<Card>());
                return true;
            }
            return false;
        }

        public override string ToString()
        {
            return string.Format(display, value) + "\n";
        }
    }

}