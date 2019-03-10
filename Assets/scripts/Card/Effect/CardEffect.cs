using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;
using DG.Tweening;
using System.Xml.Linq;
using System;
using Newtonsoft.Json.Linq;

public class CardEffect : MonoBehaviour
{
    public virtual bool Play(PlayInfo parameters) { return false; }
    public override string ToString() { return ""; }

    // temp function : should probably make something better in the future 
    public static CardEffect FromJson(JToken jeffect, Card owner)
    {
        CardEffect effect = null;

        switch (jeffect["type"].ToString())
        {
            case "Damage":
                effect = owner.gameObject.AddComponent<CardEffectDamage>();
                CardEffectDamage dmg = effect as CardEffectDamage;
                dmg.value = jeffect["value"] != null ? (int)jeffect["value"] : 0;
                dmg.display = jeffect["display"]?.ToString();
                break;
            case "Heal":
                effect = owner.gameObject.AddComponent<CardEffectHeal>();
                CardEffectHeal heal = effect as CardEffectHeal;
                heal.value = jeffect["value"] != null ? (int)jeffect["value"] : 0;
                heal.display = jeffect["display"]?.ToString();
                break;
            case "DrawCards":
                effect = owner.gameObject.AddComponent<CardEffectDraw>();
                CardEffectDraw draw = effect as CardEffectDraw;
                draw.value = jeffect["value"] != null ? (int)jeffect["value"] : 0;
                draw.display = jeffect["display"]?.ToString();
                break;
            case "DiscardRandomCards":
                effect = owner.gameObject.AddComponent<CardEffectDiscard>();
                CardEffectDiscard discard = effect as CardEffectDiscard;
                discard.value = jeffect["value"] != null ? (int)jeffect["value"] : 0;
                discard.display = jeffect["display"]?.ToString();
                break;
            default:
                effect = new CardEffect();
                break;
        }

        return effect;
    }
}
