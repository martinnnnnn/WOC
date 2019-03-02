using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;
using DG.Tweening;
using System.Xml.Linq;
using System;

public class CardList : MonoBehaviour
{
    public GameObject cardPrefab;
    public Dictionary<string, Card> cards;

    public void ReadFile(string path)
    {
        cards = new Dictionary<string, Card>();

        XDocument xdoc = XDocument.Load(path);
        var xroot = xdoc.Element("cards");
        foreach (var xcard in xroot.Elements("card"))
        {
            GameObject newCard = Instantiate(cardPrefab, transform);
            CardDesc newDesc = newCard.GetComponent<CardDesc>();
            newDesc.ReadXML(xcard);
            cards.Add(newDesc.Title, newCard.GetComponent<Card>());
            newCard.SetActive(false);
        }
    }
}
