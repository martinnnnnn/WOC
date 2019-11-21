using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using WOC_Core;
using DG.Tweening;



namespace WOC_Client
{
    public class PlayerController : MonoBehaviour
    {
        NetworkInterface network;
        BattleManager battle;
        [HideInInspector] public string playerName;

        public TMP_Text nameText;
        [HideInInspector] public int life;
        public TMP_Text lifeText;
        [HideInInspector] public int handCount;
        public TMP_Text handCountText;
        public GameObject glow;

        public void Init(BattleManager battle, PD_BattleStatePlayer data)
        {
            this.battle = battle;
            network = FindObjectOfType<NetworkInterface>();
            network.Callback_BattleStatePlayer += HandleAPICall;
            network.Callback_BattleCardDrawn += HandleAPICall;
            network.Callback_BattleCardPlayed += HandleAPICall;
            network.Callback_BattlePlayerTurnEnd += HandleAPICall;
            HandleAPICall(data);

            glow.SetActive(false);
        }

        private void HandleAPICall(PD_BattleStatePlayer data)
        {
            transform.position = this.battle.playersLocations[data.location].position;
            playerName = data.name;
            nameText.text = playerName;
            life = data.life;
            lifeText.text = life.ToString();
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
                glow.SetActive(true);
                glow.transform.DOPunchScale(new Vector3(5.0f, 5.0f, 5.0f), 1.0f, vibrato: 2, elasticity: 0).OnComplete(() =>
                {
                    glow.transform.localScale = Vector3.one;
                    glow.SetActive(false);
                });

                handCount--;
                handCountText.text = handCount.ToString();
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

        public void OnDestroy()
        {
            network.Callback_BattleStatePlayer -= HandleAPICall;
            network.Callback_BattleCardDrawn -= HandleAPICall;
            network.Callback_BattleCardPlayed -= HandleAPICall;
            network.Callback_BattlePlayerTurnEnd -= HandleAPICall;
        }
    }
}

