using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;
using DG.Tweening;
using System.Xml.Linq;
using System;

public class CardEffect : MonoBehaviour
{
    public enum Type
    {
        Damage,
        Block,
        Heal,
        DrawCards,
        DiscardRandomCards,
        CopyToDeck,
        CopyToDiscard,
        UpgradeCard,
        ApplyWeak,
        ApplyStrenght,
        ApplyVulnarable,
        ShuffleDeck,
        ShuffleDiscard
    }

    public class PlayInfo
    {
        public PlayInfo(Player owner, Character target)
        {
            this.owner = owner;
            this.target = target;
        }
        public Player owner;
        public Character target;
    }

    private Dictionary<Type, Func<PlayInfo, bool>> applyEffects;
    public int value;
    public Type type;
    public string display = "";
    bool effectFunctionsInitialized = false;

    public void Start()
    {
        InitEffectFunctions();
    }

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

    void InitEffectFunctions()
    {
        applyEffects = new Dictionary<Type, Func<PlayInfo, bool>>();
        applyEffects.Add(Type.Damage, DamageEffect);
        applyEffects.Add(Type.Block, BlockEffect);
        applyEffects.Add(Type.Heal, HealEffect);
        applyEffects.Add(Type.DrawCards, DrawCardsEffect);
        applyEffects.Add(Type.DiscardRandomCards, DiscardRandomCardsEffect);
        applyEffects.Add(Type.CopyToDeck, CopyToDeckEffect);
        applyEffects.Add(Type.CopyToDiscard, CopyToDiscardEffect);
        applyEffects.Add(Type.UpgradeCard, UpgradeCardEffect);
        applyEffects.Add(Type.ApplyWeak, ApplyWeakEffect);
        applyEffects.Add(Type.ApplyStrenght, ApplyStrenghtEffect);
        applyEffects.Add(Type.ApplyVulnarable, ApplyVulnerableEffect);
        applyEffects.Add(Type.ShuffleDeck, ShuffleDeckEffect);
        applyEffects.Add(Type.ShuffleDiscard, ShuffleDiscardEffect);
    }

    private bool DamageEffect(PlayInfo parameters)
    {
        if (parameters.target && parameters.owner != parameters.target)
        {
            parameters.target.ChangeLife(-value);
            return true;
        }
        return false;
    }

    private bool BlockEffect(PlayInfo parameters)
    {
        return false;
    }

    private bool HealEffect(PlayInfo parameters)
    {
        if (parameters.target)
        {
            parameters.target.ChangeLife(value);
            return true;
        }
        return false;
    }

    private bool DrawCardsEffect(PlayInfo parameters)
    {
        if (parameters.owner)
        {
            parameters.owner.DrawCards(value);
            return true;
        }
        return false;
    }

    private bool DiscardRandomCardsEffect(PlayInfo parameters)
    {
        if (parameters.owner)
        {
            parameters.owner.DiscardRandomCards(value, GetComponent<Card>());
            return true;
        }
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
