using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;
using DG.Tweening;
using System.Xml.Linq;
using System;

public class CardDesc : MonoBehaviour
{
    public TextMesh nameMesh;
    public TextMesh manaMesh;
    public TextMesh effectsMesh;
    public TextMesh exhaustMesh;

    public string Title = "default title";
    public int ManaCost;
    public bool Exhaust = false;
    public Color CardColor;

    public List<CardEffect> Effects = new List<CardEffect>();

    public void UpdateEffectDisplay()
    {
        string effectsStr = "";
        foreach (CardEffect e in Effects)
        {
            effectsStr += e.ToString();
        }
        effectsMesh.text = effectsStr;
    }

    public bool Play(CardEffect.PlayInfo playInfo)
    {
        bool result = false;
        foreach(var effect in Effects)
        {
            if (effect.Play(playInfo))
            {
                result = true;
            }
        }
        return result;
    }

    public void ReadXML(XElement xcard)
    {
        ChangeTitle(xcard.Element("title").Value.ToString());
        ChangeManaCost(int.Parse(xcard.Element("manaCost").Value));
        ChangeExhaust(bool.Parse(xcard.Element("exhaust").Value));
        ColorUtility.TryParseHtmlString(xcard.Element("color").Value, out Color color);
        this.CardColor = color;
        GetComponent<MeshRenderer>().material.color = CardColor;

        foreach (var xeffect in xcard.Elements("effect"))
        {
            CardEffect effect = gameObject.AddComponent<CardEffect>();
            effect.ReadXML(xeffect);
            Effects.Add(effect);
        }
        UpdateEffectDisplay();
    }

    public void ChangeTitle(string title)
    {
        Title = title;
        nameMesh.text = Title;
    }

    public void ChangeManaCost(int cost)
    {
        ManaCost = cost;
        manaMesh.text = ManaCost.ToString();
    }

    public void ChangeExhaust(bool ex)
    {
        Exhaust = ex;
        exhaustMesh.text = Exhaust ? "Exhaust" : "";
    }
}