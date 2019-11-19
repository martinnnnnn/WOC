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
    public class Hand
    {
        List<Card> cards = new List<Card>();
        public List<Card> Cards
        {
            get { return cards; }
        }

        BattlePlayer owner;

        public Hand(BattlePlayer owner)
        {
            this.owner = owner;
        }

        public int Count => cards.Count;

        public void Add(Card newCard)
        {
            cards.Add(newCard);
        }

        public void Add(Card[] newCards)
        {
            System.Diagnostics.Debug.Assert(newCards != null);
            cards.AddRange(newCards);
        }

        public bool Contains(Card card)
        {
            return cards.Find(c => c.name == card.name) != null;
        }

        public void Remove(int index)
        {
            if (index >= 0 && index < Count)
            {
                cards.RemoveAt(index);
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

        public Card[] Flush()
        {
            Card[] result = cards.ToArray();
            cards.Clear();
            return result;
        }

        //public Card DiscardRandom(Card ignore = null)
        //{
        //    Card result = null;
        //    if (cards.Count == 1)
        //    {
        //        return result;
        //    }

        //    do
        //    {
        //        int index = owner.Battle.random.Next(0, cards.Count);
        //        result = cards[index];
        //        cards.RemoveAt(index);
        //    }
        //    while ((ignore != null && result == ignore) || ignore != null);

        //    return result;
        //}
    }
}
