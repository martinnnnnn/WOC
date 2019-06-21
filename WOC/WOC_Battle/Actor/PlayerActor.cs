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
        public Hand hand;

        [JsonConstructor]
        public PlayerActor(
            Battle battle,
            Character character,
            Hand hand,
            string name,
            List<string> cardsNames,
            int aggroIncrement,
            int manaMax,
            int maxInitiative) : base(battle, character, name)
        {
            foreach (string cardName in cardsNames)
            {
                deck.Add(battle.GetCard(cardName));
            }

            this.hand = hand;
            aggro.IncrementRatio = 0;
            mana.Max = manaMax;
            initiative.Set(deck.Count, maxInitiative);
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
            DrawCards(hand.StartingCount);
        }

        public bool PlayCard(Card card, Character target)
        {
            if (hand.Contains(card))
            {
                if (card.Play(new PlayInfo(){Owner = this, Target = target}))
                {
                    mana.Consume(card.manaCost);
                    hand.Remove(card);
                    discardPile.Push(card);
                    aggro.Increment();
                    return true;
                }
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

                if (hand.IsFull)
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
            while (hand.Count > 0 && count > 0)
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
