using System.Collections.Generic;
using TMPro;
using UnityEngine;
using WOC_Core;
using DG.Tweening;
using System;

namespace WOC_Client
{
    public class MainPlayerController : MonoBehaviour
    {
        NetworkInterface network;
        BattleManager battle;
        string playerName;

        public GameObject cardPrefab;
        List<CardController> hand = new List<CardController>();

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
            network.Callback_BattlePlayerTurnEnd += HandleAPICall;
            network.Callback_BattleCardDrawn += HandleAPICall;
            network.Callback_BattleCardPlayed += HandleAPICall;

            playerName = network.session.account.name;
            nameText.text = playerName;
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
        private void HandleAPICall(PD_BattlePlayerTurnEnd data)
        {
            for (int i = 0; i < hand.Count; ++i)
            {
                CardController current = hand[i];
                current.transform.DOMove(battle.discardPile.position, 1).OnComplete(() =>
                {
                    hand.Remove(current);
                    Destroy(current.gameObject);
                });
            }
        }

        private void HandleAPICall(PD_BattleCardDrawn data)
        {
            if (data.playerName == playerName)
            {
                //CardController
                GameObject newCard = Instantiate(cardPrefab, this.transform);
                CardController newCardController = newCard.GetComponent<CardController>();
                newCardController.Init(this, battle.drawPile.position);
                newCardController.index = hand.Count;
                hand.Add(newCardController);
                UpdateHandPositions();

                int newDrawnPileCount = int.Parse(drawPileCountText.text);
                newDrawnPileCount--;
                drawPileCountText.text = newDrawnPileCount.ToString();
            }
        }

        private void HandleAPICall(PD_BattleCardPlayed data)
        {
            CardController played = hand[data.cardIndex];
            played.transform.DOMove(battle.discardPile.position, 1).OnComplete(() =>
            {
                hand.Remove(played);
                UpdateHandIndexes();
                UpdateHandPositions();
                Destroy(played.gameObject);
            });
        }

        private void UpdateHandIndexes()
        {
            for (int i = 0; i < hand.Count; ++i)
            {
                hand[i].index = i;
            }
        }

        
        private void UpdateHandPositions()
        {
            for (int i = 0; i < hand.Count; ++i)
            {
                Vector3 endPosition = 
                    battle.handStartPosition.position + (((float) i / hand.Count) * (battle.handEndPosition.position - battle.handStartPosition.position));

                // this seems useless but it needed for the callback to work :
                // i keeps incrementing and is going to be == handCount when OnComplete is called.
                CardController current = hand[i];
                current.transform.DOMove(endPosition, 1).OnComplete(() =>
                {
                    current.restPosition = endPosition;
                    current.useRestPos = true;
                });
                
            }
        }
    }
}

