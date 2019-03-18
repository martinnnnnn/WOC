using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace WOC
{
    [RequireComponent(typeof(Aggro), typeof(Mana))]
    public class MainPlayer : BasePlayer
    {

        Aggro aggro;
        Battle battle;
        Mana mana;

        Deck drawPile;
        Deck discardPile;
        Hand hand;
        [HideInInspector] public Character character;

        private void Start()
        {
            battle = FindObjectOfType<Battle>();
            aggro = GetComponent<Aggro>();
            mana = GetComponent<Mana>();
        }

        public override void BattleInit()
        {
            character = GetComponentInChildren<Character>();
            drawPile.Init();
            drawPile.Shuffle();
            aggro.Reset();
            mana.Reset();
        }

        public override void StartTurn()
        {
            mana.Reset();
            aggro.StartTurn();
        }

        public override void PlayTurn()
        {

        }

        public override void EndTurn()
        {
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
    }
}