using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WOC_Battle
{
    public class PlayerActor : Actor
    {
        public Aggro aggro = new Aggro();
        public Mana mana = new Mana();
        public List<Card> deck = new List<Card>();
        public CardPile drawPile = new CardPile();
        public CardPile discardPile = new CardPile();
        public Hand hand = new Hand();

        [JsonConstructor]
        public PlayerActor(
            Battle battle,
            List<string> cardsNames,
            int aggroIncrement,
            int manaMax,
            int maxInitiative,
            int life) : base(battle, life)
        {
            foreach (string cardName in cardsNames)
            {
                deck.Add(battle.GetCard(cardName));
            }

            aggro.IncrementRatio = 0;
            mana.Max = manaMax;
            initiative.Set(deck.Count, maxInitiative);
            character.Life = life;
            character.MaxLife = life;
        }

        public override void BattleInit()
        {
            base.BattleInit();

            // init character
            character.Life = character.MaxLife;

            // init drawpile
            drawPile.Flush();
            drawPile.Push(deck.ToArray());
            drawPile.Shuffle();

            // empty discardpile
            discardPile.Flush();

            // empty hand
            hand.Flush();

            // reset mana
            mana.Reset();

            // reset aggro
            aggro.Reset();
        }

        public override void BattleEnd()
        {
            base.BattleEnd();

        }

        public override void StartTurn()
        {
            base.StartTurn();

            mana.Reset();
            aggro.StartTurn();
            DrawCards(hand.startingCount);
        }

        public bool PlayCard(Card card, Character target)
        {
            PlayInfo info = new PlayInfo()
            {
                Owner = this,
                Target = target
            };

            if (card.Play(info))
            {
                mana.Consume(card.manaCost);
                hand.Remove(card);
                discardPile.Push(card);
                aggro.Increment();
                return true;
            }
            return false;
        }

        public override void EndTurn()
        {
            base.EndTurn();

            discardPile.Push(hand.Flush());
        }

        public void DrawCards(int count, bool discardIfMaxReach = true)
        {
            for (int i = 0; i < count; ++i)
            {
                if (drawPile.Count == 0)
                {
                    MoveDiscardToDraw();
                }

                if (hand.maxCount == hand.Count)
                {
                    if (discardIfMaxReach)
                    {
                        discardPile.Push(drawPile.Pop(1));
                    }
                    else
                    {
                        break;
                    }
                }
                else
                {
                    hand.Add(drawPile.Pop(1));
                }
            }
        }

        public void MoveDiscardToDraw()
        {
            drawPile.Push(discardPile.Flush());
            drawPile.Shuffle();
        }

        public void DiscardRandomCards(int count, Card caller = null)
        {
            while (hand.cards.Count > 0 && count > 0)
            {
                Card card = hand.DiscardRandom(caller);
                if (card != null)
                {
                    discardPile.Push(card);
                    aggro.Increment();
                }
                --count;
            }
        }
    }
}
