using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using System.Security.Cryptography;
using System.Xml.Linq;


namespace WOC
{
    public class Deck : MonoBehaviour
    {
        public List<Card> cards = new List<Card>();
        public Player player;
        public float cardPlacementTime;

        public int Count => cards.Count;

        public void Init(string path = "")
        {
            if (path == "")
            {
                Init(Application.dataPath + "/data/default_deck.xml");
                return;
            }

            var cardlist = FindObjectOfType<CardList>();
            XDocument xdoc = XDocument.Load(path);
            foreach (var xcard in xdoc.Element("cards").Elements("card"))
            {
                Card card = cardlist.cards[xcard.Attribute("title").Value];
                Card newCard = Instantiate(card, transform);
                newCard.gameObject.SetActive(true);
                AddCard(newCard);
            }
            ReplaceCards();
        }

        public Card[] GetNewCards(int count)
        {
            Card[] newCards = null;

            int toRemove = Math.Min(count, cards.Count);
            if (cards.Count > 0)
            {
                newCards = cards.GetRange(0, toRemove).ToArray();
                cards.RemoveRange(0, toRemove);
            }
            ReplaceCards();
            return newCards;
        }

        public void Shuffle()
        {
            RNGCryptoServiceProvider provider = new RNGCryptoServiceProvider();
            int n = cards.Count;
            while (n > 1)
            {
                byte[] box = new byte[1];
                do provider.GetBytes(box);
                while (!(box[0] < n * (Byte.MaxValue / n)));
                int k = (box[0] % n);
                n--;
                Card value = cards[k];
                cards[k] = cards[n];
                cards[n] = value;
            }
            ReplaceCards();
        }

        public void AddCards(Card[] cards)
        {
            for (int i = 0; i < cards.Length; ++i)
            {
                AddCard(cards[i], false);
            }
            ReplaceCards();
        }

        public float cardOffsetPosition;
        public void AddCard(Card card, bool replace = true)
        {
            card.owner = player;
            cards.Add(card);
            if (replace)
            {
                ReplaceCards();
            }
        }

        public void ReplaceCards()
        {
            for (int i = 0; i < cards.Count; ++i)
            {
                Vector3 newPosition = transform.position + (player.cameraTransform.forward * (cardOffsetPosition * i));
                newPosition.y = transform.position.y;
                cards[i].Move(newPosition, transform.rotation, cardPlacementTime);
            }
        }

        void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawSphere(transform.position, 0.3f);
        }

        public Card[] Flush()
        {
            Card[] result = cards.ToArray();
            cards.Clear();
            return result;
        }
    }
}