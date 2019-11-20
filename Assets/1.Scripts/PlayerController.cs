using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using WOC_Core;




namespace WOC_Client
{
    public class PlayerController : MonoBehaviour
    {
        NetworkInterface network;
        BattleManager battle;
        [HideInInspector] public string playerName;

        public TMP_Text nameText;
        public TMP_Text lifeText;
        [HideInInspector] int handCount;
        public TMP_Text handCountText;
        
        public void Init(BattleManager battle, PD_BattleStatePlayer data)
        {
            this.battle = battle;
            network = FindObjectOfType<NetworkInterface>();
            network.Callback_BattleStatePlayer += HandleAPICall;
            network.Callback_BattleCardDrawn += HandleAPICall;
            network.Callback_BattleCardPlayed += HandleAPICall;
            network.Callback_BattlePlayerTurnEnd += HandleAPICall;

            HandleAPICall(data);
        }

        private void HandleAPICall(PD_BattleStatePlayer data)
        {
            transform.position = this.battle.playersLocations[data.location].position;
            playerName = data.name;
            nameText.text = playerName;
            lifeText.text = data.life.ToString();
            handCount = data.handCount;
            handCountText.text = handCount.ToString();
        }

        private void HandleAPICall(PD_BattleCardDrawn data)
        {
            if (data.playerName == playerName)
            {
                handCount++;
                handCountText.text = handCount.ToString();
            }
        }

        private void HandleAPICall(PD_BattleCardPlayed data)
        {
            if (data.ownerName == playerName)
            {
                int newHandCount = handCount - 1;
                handCountText.text = newHandCount.ToString();

                // TODO apply card effet
                battle.monstersControllers.Find(m => m.monsterName == data.targetName);
            }
        }

        private void HandleAPICall(PD_BattlePlayerTurnEnd data)
        {
            if (data.playerName == playerName)
            {
                handCount = 0;
                handCountText.text = handCount + "";
            }
        }


    }
}

