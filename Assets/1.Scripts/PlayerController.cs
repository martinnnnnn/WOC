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
        string playerName;

        public TMP_Text nameText;
        public TMP_Text lifeText;
        public TMP_Text handCount;

        public void Init(BattleManager battle, PD_BattleStatePlayer data)
        {
            this.battle = battle;
            network = FindObjectOfType<NetworkInterface>();
            network.Callback_BattleStatePlayer += HandleAPICall;
            network.Callback_BattleCardDrawn += HandleAPICall;
            network.Callback_BattleCardPlayed += HandleAPICall;

            playerName = network.session.account.name;
            HandleAPICall(data);
        }

        private void HandleAPICall(PD_BattleStatePlayer data)
        {
            transform.position = this.battle.playersLocations[data.location].position;
            nameText.text = data.name;
            lifeText.text = data.life.ToString();
            handCount.text = data.handCount.ToString();
        }

        private void HandleAPICall(PD_BattleCardDrawn data)
        {
            if (data.playerName == playerName)
            {
                int drawCount = Int32.Parse(handCount.text);
                int newHandCount = drawCount + 1;
                handCount.text = newHandCount.ToString();

            }
        }

        private void HandleAPICall(PD_BattleCardPlayed data)
        {

        }

    }
}

