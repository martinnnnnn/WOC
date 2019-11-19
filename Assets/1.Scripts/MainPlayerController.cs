using System.Collections.Generic;
using TMPro;
using UnityEngine;
using WOC_Core;


namespace WOC_Client
{
    public class MainPlayerController : MonoBehaviour
    {
        NetworkInterface network;
        BattleManager battle;

        public GameObject cardPrefab;

        public Transform drawPile;
        public Transform discardPile;
        public Transform handStartPosition;
        public Transform handEndPosition;

        public TMP_Text nameText;
        public TMP_Text lifeText;
        public TMP_Text drawPileCountText;
        public TMP_Text discardPileCountText;

        public void Init(BattleManager battle, PD_BattleStateMainPlayer data)
        {
            this.battle = battle;
            network = FindObjectOfType<NetworkInterface>();
            network.Callback_BattleStateMainPlayer += HandleAPICall;
            network.Callback_BattlePlayerTurnStart += HandleAPICall;
            network.Callback_BattleCardDrawn += HandleAPICall;
            network.Callback_BattleCardPlayed += HandleAPICall;

            nameText.text = this.network.session.account.name;
            HandleAPICall(data);
        }

        private void HandleAPICall(PD_BattleStateMainPlayer data)
        {
            transform.position = this.battle.playersLocations[data.location].position;
            lifeText.text = data.life.ToString();
            //public List<string> hand;
            drawPileCountText.text = data.drawPileCount.ToString();
            discardPileCountText.text = data.discardPileCount.ToString();
        }

        private void HandleAPICall(PD_BattlePlayerTurnStart data)
        {

        }

        private void HandleAPICall(PD_BattleCardDrawn data)
        {

        }

        private void HandleAPICall(PD_BattleCardPlayed data)
        {

        }

        //public void SetPlayer(WOC_Core.RTTS.BattlePlayer battlePlayer)
        //{
        //    player = battlePlayer;
        //    GetComponentInChildren<TMP_Text>().text = this.player.name;

        //    foreach (var card in player.drawPile.cards)
        //    {
        //        GameObject newCardObj = Instantiate(cardPrefab, drawPile.transform);
        //        newCardObj.transform.position = drawPile.transform.position;
        //        CardController newCardController = newCardObj.GetComponent<CardController>();
        //        newCardController.owner = this;
        //        newCardController.card = card;
        //        newCardController.index = -1;
        //        cardControllersDrawPile.Add(newCardController);
        //    }

        //    player.CardDrawn += HandleCardDrawn;
        //}

        //public void HandleCardDrawn(WOC_Core.RTTS.Card card)
        //{
        //    CardController drawnCard = cardControllersDrawPile.Find(controller => controller.card.name == card.name);
        //    if (drawnCard != null)
        //    {
        //        cardControllersDrawPile.Remove(drawnCard);
        //        cardControllersHand.Add(drawnCard);
        //    }

        //    GameObject newCardObj = Instantiate(cardPrefab, drawPile.transform);
        //    newCardObj.transform.position = drawPile.transform.position;
        //    CardController newCardController = newCardObj.GetComponent<CardController>();
        //    newCardController.owner = this;
        //    newCardController.card = card;
        //    newCardController.index = -1;
        //    cardControllersDrawPile.Add(newCardController);

        //    RepositionCards();
        //}

        //public void RepositionCards()
        //{
        //    foreach (CardController c in cardControllersDrawPile)
        //    {
        //        c.transform.position = Vector2.Lerp(handStartPosition.position, handEndPosition.position, (float)c.index / (float)cardControllersDrawPile.Count);
        //    }
        //}
    }
}

