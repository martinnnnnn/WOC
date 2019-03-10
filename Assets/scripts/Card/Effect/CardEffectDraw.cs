using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;
using DG.Tweening;
using System.Xml.Linq;
using System;


public class CardEffectDraw : CardEffect
{
    public int value = 0;
    public string display = "";

    public override bool Play(PlayInfoo parameters)
    {
        if (parameters.owner)
        {
            parameters.owner.DrawCards(value);
            return true;
        }
        return false;
    }

    public override string ToString()
    {
        return string.Format(display, value) + "\n";
    }
}

