using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace WOC
{
    [RequireComponent(typeof(Aggro), typeof(Mana))]
    public class MainPlayer : BasePlayer
    {
        public Deck drawPile;
        public Deck discardPile;
        public Hand hand;

        Aggro aggro;
        Battle battle;
        Mana mana;


        Card selectedCard;

        private void Start()
        {
            battle = FindObjectOfType<Battle>();
            aggro = GetComponent<Aggro>();
            mana = GetComponent<Mana>();
        }

        public override void BattleInit()
        {
            base.BattleInit();
            drawPile.Init();
            drawPile.Shuffle();
            aggro.Reset();
            mana.Reset();
        }

        public override void StartTurn()
        {
            base.BattleInit();
            mana.Reset();
            aggro.StartTurn();
            DrawCards(hand.startingCount);
        }

        public override void PlayTurn()
        {
            base.BattleInit();
            if (Input.GetMouseButtonDown(0))
            {
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                RaycastHit hit;
                if (Physics.Raycast(ray, out hit))
                {
                    if (selectedCard) selectedCard.IsSelected = false;
                    Card card = hit.transform.GetComponent<Card>();
                    if (card && hand.cards.Contains(card))
                    {
                        selectedCard = card;
                        selectedCard.IsSelected = true;
                    }
                    else
                    {
                        if (selectedCard && selectedCard.owner == this)
                        {
                            PlayInfo info = new PlayInfo()
                            {
                                owner = this,
                                target = hit.transform.GetComponent<Character>()
                            };
                            if (info.target) Debug.Log("clicked on " + info.target.name);
                            if (selectedCard.Play(info))
                            {
                                selectedCard.IsSelected = false;
                                mana.Consume(selectedCard.descc.manaCost);
                                hand.RemoveCard(selectedCard);
                                discardPile.AddCard(selectedCard);
                                selectedCard = null;

                                aggro.Increment();
                            }
                        }
                    }
                }
            }
        }

        public override void EndTurn()
        {
            base.BattleInit();
            discardPile.AddCards(hand.Flush());
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
                        discardPile.AddCards(drawPile.GetNewCards(1));
                    }
                    else
                    {
                        break;
                    }
                }
                else
                {
                    hand.Add(drawPile.GetNewCards(1));
                }
            }
        }

        public void MoveDiscardToDraw()
        {
            drawPile.AddCards(discardPile.Flush());
            drawPile.Shuffle();
        }

        public void DiscardRandomCards(int count, Card caller = null)
        {
            while (hand.cards.Count > 0 && count > 0)
            {
                Card card = hand.DiscardRandom(caller);
                if (card)
                {
                    discardPile.AddCard(card);
                    aggro.Increment();
                }
                --count;
            }
            discardPile.ReplaceCards();
        }
    }
}