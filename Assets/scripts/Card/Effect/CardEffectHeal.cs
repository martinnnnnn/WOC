using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;
using DG.Tweening;
using System.Xml.Linq;
using System;


public class CardEffectHeal : CardEffect
{
    public int value = 0;
    public string display = "";

    public override bool Play(PlayInfoo parameters)
    {
        if (parameters.target)
        {
            parameters.target.ChangeLife(value);
            return true;
        }
        return false;
    }

    public override string ToString()
    {
        return string.Format(display, value) + "\n";
    }
}

