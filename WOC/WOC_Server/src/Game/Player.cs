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
    public class Player : Combatant
    {
        // objets
        public Deck deck;
        public Hand hand;
        public CardPile drawPile;
        public CardPile discardPile;
        public Battle battle;
        // battle utils
        public double timeRemaining;

        public Player(string name, int life, int location, Deck deck)
        {
            this.name = name;
            this.life = life;
            this.location = location;
            this.deck = deck;
            hand = new Hand(this);
            drawPile = new CardPile(this);
            discardPile = new CardPile(this);

            drawPile.Push(deck.cards.ToArray());
        }

        public void Init(Battle battle)
        {
            this.battle = battle;
            drawPile.Shuffle();
        }

        public void Update(float dt)
        {
            timeRemaining -= dt;
            if (timeRemaining <= 0)
            {
                battle.PlayerTurnEnd(this);
            }
        }

        public void InitTurn(double turnDuration)
        {
            timeRemaining = turnDuration;
            battle.server.sessions.First(s => s.account.name == name)?.Send(
                new PD_BattlePlayerTurnStart
                {
                    startTime = DateTime.UtcNow.AddSeconds(5),
                    turnDuration = turnDuration
                });

            Random rand = new Random();
            DrawCards(5);
            //DrawCards(rand.Next(6, 10));
        }

        public bool PlayCard(int index, string targetName, bool force = false)
        {
            Card card = hand.Get(index);

            Combatant comb = battle.monsters.Find(m => m.name == targetName) as Combatant ??
                battle.players.Find(m => m.name == targetName) as Combatant;

            if (card.timeCost <= timeRemaining && card.Play(comb))
            {
                timeRemaining -= card.timeCost;
                hand.Remove(index);
                discardPile.Push(card);
                return true;
            }

            return false;
        }

        public bool PlayCard(string cardName, string targetName, bool force = false)
        {
            Card card = hand.Cards.Find(c => c.name == cardName);
            if (card != null)
            {
                int index = hand.Cards.IndexOf(card);
                return PlayCard(index, targetName, force);
            }
            return false;
        }

        public void MoveDiscardToDraw()
        {
            drawPile.Push(discardPile.Flush());
            drawPile.Shuffle();

            battle.server.BroadcastTo(new PD_BattleDiscardToDraw
            {
                newDrawPileCount = drawPile.Count
            }, battle.server.sessions.Where(s => s.account.name == name));
        }

        public void DrawCards(int count)
        {
            for (int i = 0; i < count; ++i)
            {
                if (drawPile.Count == 0)
                {
                    MoveDiscardToDraw();
                }

                Card newCard = drawPile.Pop();
                hand.Add(newCard);
                battle.server.Broadcast(new PD_BattleCardDrawn
                {
                    playerName = name,
                    cardName = newCard.name,
                    timeCost = newCard.timeCost
                }, null);
            }
        }

        public void EndTurn()
        {
            discardPile.Push(hand.Flush());
        }
    }
}
