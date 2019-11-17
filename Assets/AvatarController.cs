using System.Collections.Generic;
using TMPro;
using UnityEngine;





namespace WOC_Client
{
    public class AvatarController : MonoBehaviour
    {
        public WOC_Core.RTTS.BattlePlayer player;

        public List<CardController> cardControllersDrawPile = new List<CardController>();
        public List<CardController> cardControllersHand = new List<CardController>();
        public GameObject cardPrefab;
        public GameObject drawPile;
        public GameObject discardPile;

        public Transform handStartPosition;
        public Transform handEndPosition;

        public void SetPlayer(WOC_Core.RTTS.BattlePlayer battlePlayer)
        {
            player = battlePlayer;
            GetComponentInChildren<TMP_Text>().text = this.player.name;

            foreach (var card in player.drawPile.cards)
            {
                GameObject newCardObj = Instantiate(cardPrefab, drawPile.transform);
                newCardObj.transform.position = drawPile.transform.position;
                CardController newCardController = newCardObj.GetComponent<CardController>();
                newCardController.owner = this;
                newCardController.card = card;
                newCardController.index = -1;
                cardControllersDrawPile.Add(newCardController);
            }

            player.CardDrawn += HandleCardDrawn;
        }

        public void HandleCardDrawn(WOC_Core.RTTS.Card card)
        {
            CardController drawnCard = cardControllersDrawPile.Find(controller => controller.card.name == card.name);
            if (drawnCard != null)
            {
                cardControllersDrawPile.Remove(drawnCard);
                cardControllersHand.Add(drawnCard);
            }

            GameObject newCardObj = Instantiate(cardPrefab, drawPile.transform);
            newCardObj.transform.position = drawPile.transform.position;
            CardController newCardController = newCardObj.GetComponent<CardController>();
            newCardController.owner = this;
            newCardController.card = card;
            newCardController.index = -1;
            cardControllersDrawPile.Add(newCardController);

            RepositionCards();
        }

        public void RepositionCards()
        {
            foreach (CardController c in cardControllersDrawPile)
            {
                c.transform.position = Vector2.Lerp(handStartPosition.position, handEndPosition.position, (float)c.index / (float)cardControllersDrawPile.Count);
            }
        }
    }
}

