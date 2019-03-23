using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace WOC
{
    public class Hand : MonoBehaviour
    {
        public List<Card> cards = new List<Card>();
        public int maxCount;
        public int startingCount;

        public int Count => cards.Count;

        public Transform startTransform;
        public Transform endTransform;
        public float cardPlacementTime;

        public void Add(Card newCard)
        {
            if (cards.Count < maxCount)
            {
                cards.Add(newCard);
            }

            ReplaceCards();
        }

        public void Add(Card[] newCards)
        {
            if (newCards == null)
            {
                return;
            }
            if (cards.Count + newCards.Length < maxCount)
            {
                cards.AddRange(newCards);
            }

            ReplaceCards();
        }

        public void RemoveCard(Card toRemove)
        {
            foreach (Card card in cards)
            {
                if (card == toRemove)
                {
                    cards.Remove(card);
                    break;
                }
            }

            ReplaceCards();
        }

        public void ReplaceCards()
        {
            for (int i = 0; i < cards.Count; ++i)
            {
                float delta = (float)i / (float)cards.Count;
                Vector3 newPosition = Vector3.Lerp(startTransform.position, endTransform.position, delta);
                Quaternion newRotation = Quaternion.Lerp(startTransform.rotation, endTransform.rotation, delta);
                cards[i].Move(newPosition, newRotation, cardPlacementTime);
            }
        }

        public Card DiscardRandom(Card ignore = null)
        {
            Card result = null;
            if (cards.Count == 1)
            {
                return result;
            }
            if (ignore)
            {
                while (!result || result == ignore)
                {
                    int index = Random.Range(0, cards.Count);
                    result = cards[index];
                    cards.RemoveAt(index);
                }
            }
            else
            {
                int index = Random.Range(0, cards.Count);
                result = cards[index];
                cards.RemoveAt(index);
            }
            return result;
        }

        public Card[] Flush()
        {
            Card[] result = cards.ToArray();
            cards.Clear();
            return result;
        }

        void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawSphere(startTransform.position, 0.3f);
            Gizmos.DrawSphere(endTransform.position, 0.3f);
        }
    }
}