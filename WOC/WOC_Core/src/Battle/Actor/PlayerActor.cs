//using Newtonsoft.Json;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;

//namespace WOC_Core
//{
//    public class PlayerActor : Actor
//    {
//        public Aggro aggro = new Aggro();
//        public Mana mana = new Mana();
//        public List<Card> deck = new List<Card>();
//        public CardPile drawPile;
//        public CardPile discardPile;
//        public Hand hand;

//        [JsonConstructor]
//        public PlayerActor(
//            string name,
//            int aggroIncrement,
//            int manaMax) : base(name)
//        {
//            hand = new Hand(this);
//            drawPile = new CardPile(this);
//            discardPile = new CardPile(this);
//            aggro.IncrementRatio = 0;
//            mana.Max = manaMax;
//            Console.WriteLine("[PLAYER] mana max : {0}", mana.Max);
//        }

//        public void AddCards(List<string> cardsNames)
//        {
//            foreach (string cardName in cardsNames)
//            {
//                deck.Add(Card.Get(cardName));
//            }
//        }

//        public override void BattleInit()
//        {
//            base.BattleInit();

//            // init character
//            Chara.SetLife(Chara.MaxLife);

//            // init drawpile
//            drawPile.Flush();
//            drawPile.Push(deck.ToArray());
//            drawPile.Shuffle();

//            // empty discardpile
//            discardPile.Flush();

//            // empty hand
//            hand.Flush();

//            // reset mana
//            mana.Reset();

//            // reset aggro
//            aggro.Reset();
//        }

//        public override void BattleEnd()
//        {
//            base.BattleEnd();

//        }

//        public override bool StartTurn()
//        {
//            if (base.StartTurn())
//            {
//                mana.Reset();
//                aggro.StartTurn();
//                DrawCards(Hand.StartingCount);
//                Console.WriteLine("{0}/{1} mana, {2} life", mana.Value, mana.Max, Chara.Life);
//                return true;
//            }
//            return false;
//        }

//        public bool PlayCard(Card card, Character target)
//        {
//            if (turnState == TurnState.NOT_MINE)
//            {
//                Console.WriteLine("[BATTLE] {0} cannot play : not his turn.", Name);
//                return false;
//            }

//            if (!hand.Contains(card))
//            {
//                Console.WriteLine("[BATTLE] {0} cannot play : he doesn't have this card.", Name);
//                return false;
//            }

//            if (mana.Value < card.manaCost)
//            {
//                Console.WriteLine("[BATTLE] {0} cannot play : not enough mana ({1} < {2}).", Name, mana.Value, card.manaCost);
//                return false;
//            }

//            if (card.Play(new PlayInfo { Owner = this, Target = target }))
//            {
//                mana.Consume(card.manaCost);
//                hand.Remove(card);
//                discardPile.Push(card);
//                aggro.Increment();
//                Console.WriteLine("[BATTLE] {0} played {1} on {2}", Name, card.name, target.Name);
//                card.effects.ForEach(e =>
//                {
//                    switch (e)
//                    {
//                        case CardEffectDamage dmg:
//                            Console.WriteLine("[CARDEFFECT] [DMG] {0} dmg to {1} who has {2} hp, {3} mana left", dmg.value, target.Name, target.Life, mana.Value);
//                            break;
//                        case CardEffectHeal heal:
//                            Console.WriteLine("[CARDEFFECT] [HEAL] {0} heal to {1} who has {2} hp, {3} mana left", heal.value, target.Name, target.Life, mana.Value);
//                            break;
//                        case CardEffectDraw draw:
//                            Console.WriteLine("[CARDEFFECT] [DRAW] {0} cards drawn by {1} who has {2} cards, {3} mana left", draw.value, this.Name, hand.Count, mana.Value);
//                            break;
//                        case CardEffectDiscard disc:
//                            Console.WriteLine("[CARDEFFECT] [DISCARD] {0} cards discarded by {1} who has {2} cards, {3} mana left", disc.value, this.Name, hand.Count, mana.Value);
//                            break;
//                    }
//                });
//                return true;
//            }
//            return false;
//        }

//        public override bool EndTurn()
//        {
//            if (base.EndTurn())
//            {
//                discardPile.Push(hand.Flush());
//                return true;
//            }
//            return false;
//        }

//        public void DrawCards(int count, bool discardIfMaxReach = true)
//        {
//            for (int i = 0; i < count; ++i)
//            {
//                if (drawPile.Count == 0)
//                {
//                    MoveDiscardToDraw();
//                }

//                if (hand.IsFull)
//                {
//                    if (discardIfMaxReach)
//                    {
//                        discardPile.Push(drawPile.Pop(1));
//                    }
//                    else
//                    {
//                        break;
//                    }
//                }
//                else
//                {
//                    hand.Add(drawPile.Pop(1));
//                }
//            }
//        }

//        public void MoveDiscardToDraw()
//        {
//            drawPile.Push(discardPile.Flush());
//            drawPile.Shuffle();
//        }

//        public void DiscardRandomCards(int count, Card caller = null)
//        {
//            while (hand.Count > 0 && count > 0)
//            {
//                Card card = hand.DiscardRandom(caller);
//                if (card != null)
//                {
//                    discardPile.Push(card);
//                    aggro.Increment();
//                }
//                --count;
//            }
//        }

//        public override void UpdateInitiative()
//        {
//            initiative.Set(deck.Count);
//        }
//    }
//}
