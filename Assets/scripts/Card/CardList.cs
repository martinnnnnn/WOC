using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;
using DG.Tweening;
using System.Xml.Linq;
using System;
using System.IO;
using Newtonsoft.Json.Linq;



public class CardList : MonoBehaviour
{
    public GameObject cardPrefab;
    public Dictionary<string, Card> cards;

    //public void ReadFile(string path)
    //{
    //    cards = new Dictionary<string, Card>();

    //    XDocument xdoc = XDocument.Load(path);
    //    var xroot = xdoc.Element("cards");
    //    foreach (var xcard in xroot.Elements("card"))
    //    {
    //        GameObject newCard = Instantiate(cardPrefab, transform);
    //        CardDesc newDesc = newCard.GetComponent<CardDesc>();
    //        newDesc.ReadXML(xcard);
    //        cards.Add(newDesc.Title, newCard.GetComponent<Card>());
    //        newCard.SetActive(false);
    //    }
    //}

    public void ReadJson(string path)
    {
        cards = new Dictionary<string, Card>();

        using (StreamReader reader = File.OpenText(path))
        {
            JObject o = (JObject)JToken.ReadFrom(new JsonTextReader(reader));
            foreach (var jcard in (JArray)o["cards"])
            {
                Card newCard = Instantiate(cardPrefab, transform).GetComponent<Card>();
                newCard.GetComponent<Card>().ReadJson(jcard);
                newCard.gameObject.SetActive(false);
                cards.Add(newCard.descc.title, newCard);
            }
        }
    }


}
