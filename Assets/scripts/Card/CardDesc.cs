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

    private string title = "default title";
    public string Title
    {
        get { return title; }
        set { title = value; nameMesh.text = title; }
    }

    private int manaCost;
    public int ManaCost
    {
        get { return manaCost; }
        set { manaCost = value; manaMesh.text = manaCost.ToString(); }
    }

    private bool exhaust;
    public bool Exhaust
    {
        get { return exhaust; }
        set { exhaust = value; exhaustMesh.text = exhaust ? "Exhaust" : ""; }
    }

    private List<CardEffect> effects = new List<CardEffect>();
    public List<CardEffect> Effects
    {
        get { return effects; }
        set { effects = value; }
    }

    private Color color;
    public Color Color
    {
        get { return color; }
        set
        {
            color = value;
            GetComponent<MeshRenderer>().material.color = color;
        }
    }

    public void UpdateEffectDisplay()
    {
        string effectsStr = "";
        foreach (CardEffect e in effects)
        {
            effectsStr += e.ToString();
        }
        effectsMesh.text = effectsStr;
    }

    public void Apply(Character chara)
    {
        foreach(var effect in effects)
        {
            effect.Apply(chara);
        }
    }

    public void ReadXML(XElement xcard)
    {
        Title = xcard.Element("title").Value.ToString();
        ManaCost = int.Parse(xcard.Element("manaCost").Value);
        Exhaust = bool.Parse(xcard.Element("exhaust").Value);
        ColorUtility.TryParseHtmlString(xcard.Element("color").Value, out Color color);
        this.Color = color;

        foreach (var xeffect in xcard.Elements("effect"))
        {
            CardEffect effect = gameObject.AddComponent<CardEffect>();
            effect.value = int.Parse(xeffect.Attribute("value").Value);
            effect.type = (CardEffect.Type)Enum.Parse(typeof(CardEffect.Type), xeffect.Attribute("type").Value, true);
            effect.display = xeffect.Attribute("display").Value;
            Effects.Add(effect);
        }
        UpdateEffectDisplay();
    }
}