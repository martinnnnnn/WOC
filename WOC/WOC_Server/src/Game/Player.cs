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
        int mana = 0;

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

        public void InitTurn(int highestMana)
        {
            mana = highestMana;

            var player = battle.server.sessions.First(s => s.account.name == name);

            if (player != null)
            {
                player.Send(new PD_BattlePlayerTurnStart
                {
                    startTime = DateTime.UtcNow.AddSeconds(5),
                    manas = battle.monsters.Select(m => m.roundValues[battle.turnCount % m.roundValues.Length].mana).ToArray(),
                    highestMana = highestMana
                });

                if (battle.turnCount == 1)
                {
                    DrawCards(5);
                }
                else
                {
                    DrawCards(3);
                }
            }
        }

        public bool PlayCard(int index, string targetName, bool force = false)
        {
            Card card = hand.Get(index);

            var monster = battle.monsters.Find(m => m.name == targetName);
            if (mana >= card.timeCost && monster != null && monster.currentMana[this] >= card.timeCost && card.Play(monster as Combatant))
            {
                battle.monsters.ForEach(m => m.currentMana[this] -= card.timeCost);
                mana -= card.timeCost;
                hand.Remove(index);
                discardPile.Push(card);
                return true;
            }

            var player = battle.players.Find(p => p.name == targetName);
            if (card.timeCost <= mana && card.Play(player))
            {
                battle.monsters.ForEach(m => m.currentMana[this] -= card.timeCost);
                mana -= card.timeCost;
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
