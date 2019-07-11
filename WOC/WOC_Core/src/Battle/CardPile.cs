using System.Collections;
using System.Collections.Generic;
using System;
using System.Security.Cryptography;
using System.Xml.Linq;


namespace WOC_Core
{
    public class CardPile
    {
        public Queue<Card> cards = new Queue<Card>();
        public int Count => cards.Count;
        Actor owner;

        public CardPile(Actor owner)
        {
            this.owner = owner;
        }

        public Card[] Pop(int count)
        {
            count = Math.Min(count, cards.Count);

            Card[] newCards = new Card[count];

            for (int i = 0; i < count; ++i)
            {
                newCards[i] = cards.Dequeue();
            }
            return newCards;
        }

        public void Shuffle()
        {
            Card[] tmp = cards.ToArray();
            cards.Clear();

            int n = tmp.Length;
            while (n > 1)
            {
                int k = owner.battle.random.Next(0, n);
                LOG.Print("[CARDPILE] random {0}", k);
                n--;
                Card value = tmp[k];
                tmp[k] = tmp[n];
                tmp[n] = value;
            }

            Array.ForEach(tmp, card => cards.Enqueue(card));
        }

        public void Push(Card newCard)
        {
            cards.Enqueue(newCard);
        }

        public void Push(Card[] newCards)
        {
            Array.ForEach(newCards, card => cards.Enqueue(card));
        }

        public Card[] Flush()
        {
            Card[] result = cards.ToArray();
            cards.Clear();
            return result;
        }
    }
}