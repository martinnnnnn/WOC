using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WOC_Core
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
            int manaMax) : base(battle, character, name)
        {
            foreach (string cardName in cardsNames)
            {
                deck.Add(battle.GetCard(cardName));
            }

            this.hand = hand;
            aggro.IncrementRatio = 0;
            mana.Max = manaMax;
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

        public override bool StartTurn()
        {
            if (base.StartTurn())
            {
                mana.Reset();
                aggro.StartTurn();
                DrawCards(hand.StartingCount);
                return true;
            }
            return false;
        }

        public bool PlayCard(Card card, Character target)
        {
            if (turnState == TurnState.NOT_MINE)
            {
                LOG.Print("[BATTLE] {0} cannot play : not his turn.", Name);
                return false;
            }

            if (hand.Contains(card))
            {
                if (card.Play(new PlayInfo{Owner = this, Target = target}))
                {
                    mana.Consume(card.manaCost);
                    hand.Remove(card);
                    discardPile.Push(card);
                    aggro.Increment();
                    LOG.Print("[BATTLE] {0} played {1} on {2}", Name, card.name, target.Name);
                    card.effects.ForEach(e =>
                    {
                        switch (e)
                        {
                            case CardEffectDamage dmg:
                                LOG.Print("[CARDEFFECT] [DMG] {0} dmg to {1} who has {2} hp", dmg.value, target.Name, target.Life);
                                break;
                            case CardEffectHeal heal:
                                LOG.Print("[CARDEFFECT] [HEAL] {0} heal to {1} who has {2} hp", heal.value, target.Name, target.Life);
                                break;
                            case CardEffectDraw draw:
                                LOG.Print("[CARDEFFECT] [DRAW] {0} cards drawn by {1} who has {2} cards", draw.value, this.Name, hand.Count);
                                break;
                            case CardEffectDiscard disc:
                                LOG.Print("[CARDEFFECT] [DISCARD] {0} cards discarded by {1} who has {2} cards", disc.value, this.Name, hand.Count);
                                break;
                        }
                    });
                    return true;
                }
            }
            return false;
        }

        public override bool EndTurn()
        {
            if (base.EndTurn())
            {
                discardPile.Push(hand.Flush());
                return true;
            }
            return false;
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

        public override void UpdateInitiative()
        {
            initiative.Set(deck.Count);
        }
    }
}
