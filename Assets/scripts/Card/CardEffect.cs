using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;
using DG.Tweening;
using System.Xml.Linq;
using System;

public class CardEffect : MonoBehaviour
{
    public class PlayInfo
    {
        Player owner;
        Character target;
    }

    private static Dictionary<Type, Func<PlayInfo, bool>> applyEffects;

    public void Start()
    {
        InitEffectFunctions();
    }

    public enum Type
    {
        Damage,
        Block,
        Heal,
        DrawCard,
        DiscardCard,
        CopyToDeck,
        CopyToDiscard,
        UpgradeCard,
        ApplyWeak,
        ApplyStrenght,
        ApplyVulnarable,
        ShuffleDeck,
        ShuffleDiscard
    }
    public int value;
    public Type type;
    public string display = "";

    public void ReadXML(XElement xeffect)
    {
        value = int.Parse(xeffect.Attribute("value").Value);
        type = (Type)Enum.Parse(typeof(Type), xeffect.Attribute("type").Value, true);
        display = xeffect.Attribute("display").Value;
    }

    public override string ToString()
    {
        return string.Format(display, value) + "\n";
    }

    public bool Play(PlayInfo info)
    {
        return applyEffects[type](info);
    }

    bool effectFunctionsInitialized = false;
    void InitEffectFunctions()
    {
        if (!effectFunctionsInitialized)
        {
            applyEffects = new Dictionary<Type, Func<PlayInfo, bool>>();
            applyEffects.Add(Type.Damage, DamageEffect);
            applyEffects.Add(Type.Block, BlockEffect);
            applyEffects.Add(Type.Heal, DamageEffect);
            applyEffects.Add(Type.DrawCard, DrawCardEffect);
            applyEffects.Add(Type.DiscardCard, DiscardCardEffect);
            applyEffects.Add(Type.CopyToDeck, CopyToDeckEffect);
            applyEffects.Add(Type.CopyToDiscard, CopyToDiscardEffect);
            applyEffects.Add(Type.UpgradeCard, UpgradeCardEffect);
            applyEffects.Add(Type.ApplyWeak, ApplyWeakEffect);
            applyEffects.Add(Type.ApplyStrenght, ApplyStrenghtEffect);
            applyEffects.Add(Type.ApplyVulnarable, ApplyVulnerableEffect);
            applyEffects.Add(Type.ShuffleDeck, ShuffleDeckEffect);
            applyEffects.Add(Type.ShuffleDiscard, ShuffleDiscardEffect);

            effectFunctionsInitialized = true;
        }
    }

    private bool DamageEffect(PlayInfo parameters)
    {
        return false;
    }

    private bool BlockEffect(PlayInfo parameters)
    {
        return false;
    }

    private bool HealEffect(PlayInfo parameters)
    {
        return false;
    }

    private bool DrawCardEffect(PlayInfo parameters)
    {
        return false;
    }

    private bool DiscardCardEffect(PlayInfo parameters)
    {
        return false;
    }

    private bool CopyToDeckEffect(PlayInfo parameters)
    {
        return false;
    }

    private bool CopyToDiscardEffect(PlayInfo parameters)
    {
        return false;
    }

    private bool UpgradeCardEffect(PlayInfo parameters)
    {
        return false;
    }

    private bool ApplyWeakEffect(PlayInfo parameters)
    {
        return false;
    }

    private bool ApplyStrenghtEffect(PlayInfo parameters)
    {
        return false;
    }

    private bool ApplyVulnerableEffect(PlayInfo parameters)
    {
        return false;
    }

    private bool ShuffleDeckEffect(PlayInfo parameters)
    {
        return false;
    }

    private bool ShuffleDiscardEffect(PlayInfo parameters)
    {
        return false;
    }
}
