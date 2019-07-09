using System;
using System.Collections;
using System.Collections.Generic;

namespace WOC_Core
{
    public class Hand
    {
        List<Card> cards = new List<Card>();
        int maxCount;
        int startingCount;

        Random random = new Random(Battle.RandomSeed);

        public Hand(int startingCount, int maxCount)
        {
            this.startingCount = startingCount;
            this.maxCount = maxCount;
        }

        public int Count => cards.Count;
        public int StartingCount => startingCount;
        public bool IsFull => cards.Count == maxCount;

        public void Add(Card newCard)
        {
            if (cards.Count < maxCount)
            {
                cards.Add(newCard);
            }
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
        }

        public bool Contains(Card card)
        {
            return cards.Find(c => c.name == card.name) != null;
        }

        public void Remove(Card toRemove)
        {
            foreach (Card card in cards)
            {
                if (card == toRemove)
                {
                    cards.Remove(card);
                    break;
                }
            }
        }

        public void Remove(int index)
        {
            if (index >= 0 && index < Count)
            {
                Remove(cards[index]);
            }
        }

        public Card Get(int index)
        {
            if (index >= 0 && index < Count)
            {
                return cards[index];
            }
            return null;
        }

        public Card DiscardRandom(Card ignore = null)
        {
            Card result = null;
            if (cards.Count == 1)
            {
                return result;
            }

            do
            {
                int index = random.Next(0, cards.Count);
                result = cards[index];
                cards.RemoveAt(index);
            }
            while ((ignore != null && result == ignore) || ignore != null);

            return result;
        }

        public Card[] Flush()
        {
            Card[] result = cards.ToArray();
            cards.Clear();
            return result;
        }

        public Card[] AsArray()
        {
            Card[] result = cards.ToArray();
            return result;
        }
    }
}