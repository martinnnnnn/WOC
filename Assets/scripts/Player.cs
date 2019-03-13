﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using System;

namespace WOC
{
    public class Player : MonoBehaviour
    {
        public string pseudo;
        public int mana;
        public int manaStart;
        public Text manaText;
        public Deck deck;
        public Deck discard;
        public Hand hand;
        public Transform cameraTransform;
        public float cameraSwitchTime;
        Card selectedCard;
        [HideInInspector] public float aggro;
        public float aggroX;
        public float aggroMax;
        int turnCardPlayed;
        float currentAggroValue;
        StatusBar aggroBar;

        public void BattleInit()
        {
            deck.Init();
            deck.Shuffle();
            aggro = 0;
            aggroBar = GetComponentInChildren<StatusBar>();
            aggroBar.Set(aggro / aggroMax);
        }

        public void DrawCards(int count, bool discardIfMaxReach = true)
        {
            for(int i = 0; i < count; ++i)
            {
                if (deck.cards.Count == 0)
                {
                    deck.AddCards(discard.cards.ToArray());
                    discard.cards.Clear();
                    deck.Shuffle();
                }
                if (hand.maxCount == hand.cards.Count)
                {
                    if (discardIfMaxReach)
                    {
                        discard.AddCards(deck.GetNewCards(1));
                    }
                    else
                    {
                        break;
                    }
                }
                else
                {
                    hand.Add(deck.GetNewCards(1));
                }
            }
        }

        public void DiscardRandomCards(int count, Card caller = null)
        {
            while (hand.cards.Count > 0 && count > 0)
            {
                Card card = hand.DiscardRandom(caller);
                if (card)
                {
                    discard.AddCard(card);
                }
                --count;
            }
            discard.ReplaceCards();
        }

        public void PlayTurn()
        {
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
                            if (selectedCard.Play(info))
                            {
                                selectedCard.IsSelected = false;
                                ChangeMana(-selectedCard.descc.manaCost);
                                hand.RemoveCard(selectedCard);
                                discard.AddCard(selectedCard);
                                selectedCard = null;

                                IncrementAggro();
                            }
                        }
                    }
                }
            }
        }

        private void IncrementAggro()
        {
            aggro = Math.Min(aggro + currentAggroValue, aggroMax);
            
            turnCardPlayed++;
            currentAggroValue += aggroX * turnCardPlayed;
            aggroBar.Set(aggro / aggroMax);
        }

        public void EndTurn()
        {
            discard.AddCards(hand.cards.ToArray());
            hand.Discard();
        }

        public void StartTurn()
        {
            mana = manaStart;
            turnCardPlayed = 0;
            currentAggroValue = 1;
            SetCamera(() =>
            {
                DrawCards(hand.startingCount);
            });
        }

        void SetCamera(System.Action callback)
        {
            Sequence sequence = DOTween.Sequence();
            sequence.Append(Camera.main.transform.DOMove(cameraTransform.position, cameraSwitchTime));
            sequence.Join(Camera.main.transform.DORotate(cameraTransform.rotation.eulerAngles, cameraSwitchTime));
            sequence.OnComplete(() => callback());
        }

        public void ChangeMana(int value)
        {
            mana += value;
            manaText.text = string.Format("{0} : {1}", pseudo, mana);
        }
    }
}

