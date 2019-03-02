using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

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

    public void PlayTurn()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit))
            {
                Card card = hit.transform.GetComponent<Card>();
                if (card)
                {
                    if (selectedCard)
                    {
                        selectedCard.IsSelected = false;
                    }
                    selectedCard = card;
                    selectedCard.IsSelected = true;
                }
                else
                {
                    if (selectedCard && selectedCard.owner == this)
                    {
                        bool result = selectedCard.DoEffect(hit.transform.gameObject);
                        if (result)
                        {
                            selectedCard.IsSelected = false;
                            //ChangeMana(-selectedCard.manaCost);
                            hand.RemoveCard(selectedCard);
                            discard.AddCard(selectedCard);
                            selectedCard = null;
                        }
                    }
                }
            }
        }
    }

    public void EndTurn()
    {
        mana = manaStart;
        if (deck.cards.Count == 0)
        {
            deck.AddCards(discard.cards.ToArray());
            discard.cards.Clear();
            deck.Shuffle();
        }
    }


    public void StartTurn()
    {
        SetCamera(() => SetupStartTurnCard());
    }

    void SetupStartTurnCard()
    {
        discard.AddCards(hand.cards.ToArray());
        hand.Discard();
        hand.Add(deck.GetNewCards(hand.startingCount));
    }

    void SetCamera(System.Action callback)
    {
        Sequence sequence = DOTween.Sequence();
        sequence.Append(Camera.main.transform.DOMove(cameraTransform.position, cameraSwitchTime));
        sequence.Join(Camera.main.transform.DORotateQuaternion(cameraTransform.rotation, cameraSwitchTime));
        sequence.OnComplete(() => callback());
    }

    public void ChangeMana(int value)
    {
        mana += value;
        manaText.text = string.Format("{0} : {1}", pseudo, mana);
    }
}
