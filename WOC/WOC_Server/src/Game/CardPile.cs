using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Concurrent;
using WOC_Core;

namespace WOC_Server
{
    public class CardPile
    {
        public Queue<Card> cards = new Queue<Card>();
        public int Count => cards.Count;
        Player owner;

        public CardPile(Player owner)
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

        public Card Pop()
        {
            return cards.Dequeue();
        }

        public void Shuffle()
        {
            Card[] tmp = cards.ToArray();
            cards.Clear();

            int n = tmp.Length;
            while (n > 1)
            {
                int k = owner.battle.random.Next(0, n);
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
