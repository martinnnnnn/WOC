using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using System.Security.Cryptography;

public class Deck : MonoBehaviour
{
    public List<Card> cards = new List<Card>();
    public Player player;
    public float cardPlacementTime;
    public bool isDiscard = false;


    private void Start()
    {
        if (!isDiscard)
        {
            AddCards(player.GetComponentsInChildren<Card>());
            Shuffle();
        }
    }

    public Card[] GetNewCards(int count)
    {
        Card[] newCards = null;

        int toRemove = Math.Min(count, cards.Count);
        if (cards.Count > 0)
        {
            newCards = cards.GetRange(0, toRemove).ToArray();
            cards.RemoveRange(0, toRemove);
        }
        ReplaceCards();
        return newCards;
    }

    public void Shuffle()
    {
        RNGCryptoServiceProvider provider = new RNGCryptoServiceProvider();
        int n = cards.Count;
        while (n > 1)
        {
            byte[] box = new byte[1];
            do provider.GetBytes(box);
            while (!(box[0] < n * (Byte.MaxValue / n)));
            int k = (box[0] % n);
            n--;
            Card value = cards[k];
            cards[k] = cards[n];
            cards[n] = value;
        }
        ReplaceCards();
    }

    public void AddCards(Card[] cards)
    {
        for (int i = 0; i < cards.Length; ++i)
        {
            AddCard(cards[i], false);
        }
        ReplaceCards();
    }

    public float cardOffsetPosition;
    public void AddCard(Card card, bool replace = true)
    {
        card.owner = player;
        cards.Add(card);
        if (replace)
        {
            ReplaceCards();
        }
    }

    void ReplaceCards()
    {
        for (int i = 0; i < cards.Count; ++i)
        {
            Vector3 newPosition = transform.position + (player.cameraTransform.forward * (cardOffsetPosition * i));
            newPosition.y = transform.position.y;
            cards[i].Move(newPosition, transform.rotation, cardPlacementTime);
        }
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawSphere(transform.position, 0.3f);
    }
}
