using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Hand : MonoBehaviour
{
    public List<Card> cards = new List<Card>();
    public int maxCount;
    public int startingCount;

    public Transform startTransform;
    public Transform endTransform;
    public float cardPlacementTime;


    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.M))
        {
            ReplaceCards();
        }
    }

    public void Add(Card newCard)
    {
        if (cards.Count < maxCount)
        {
            cards.Add(newCard);
        }

        ReplaceCards();
    }

    public void Add(Card[] newCards)
    {
        if (newCards == null)
        {
            return;
        }
        if (cards.Count + newCards.Length < maxCount)
        {
            cards.AddRange(newCards);
        }

        ReplaceCards();
    }

    public void RemoveCard(Card toRemove)
    {
        foreach (Card card in cards)
        {
            if (card == toRemove)
            {
                cards.Remove(card);
                break;
            }
        }

        ReplaceCards();
    }


    public void ReplaceCards()
    {
        for (int i = 0; i < cards.Count; ++i)
        {
            float delta = (float)i / (float)cards.Count;
            Vector3 newPosition = Vector3.Lerp(startTransform.position, endTransform.position, delta);
            Quaternion newRotation = Quaternion.Lerp(startTransform.rotation, endTransform.rotation, delta);
            cards[i].Move(newPosition, newRotation, cardPlacementTime);
        }
    }
    
    public void Discard()
    {
        cards.Clear();
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawSphere(startTransform.position, 0.3f);
        Gizmos.DrawSphere(endTransform.position, 0.3f);
    }
}
