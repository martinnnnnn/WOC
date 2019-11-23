using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using WOC_Core;
using DG.Tweening;
using System.Collections;

namespace WOC_Client
{
    public class PlayerController : MonoBehaviour
    {
        NetworkInterface network;
        BattleManager battle;
        [HideInInspector] public string playerName;
        public TMP_Text nameText;

        [HideInInspector] public LifeController life;

        [HideInInspector] public int handCount;
        public TMP_Text handCountText;
        public GameObject glow;

        public void Init(BattleManager battle, PD_BattleStatePlayer data)
        {
            life = GetComponent<LifeController>();
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
            life.Life = data.life;
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

        LineRenderer line;
        private void HandleAPICall(PD_BattleCardPlayed data)
        {
            if (data.ownerName == playerName)
            {
                //glow.SetActive(true);
                //glow.transform.DOPunchScale(new Vector3(5.0f, 5.0f, 5.0f), 1.0f, vibrato: 2, elasticity: 0).OnComplete(() =>
                //{
                //    glow.transform.localScale = Vector3.one;
                //    glow.SetActive(false);
                //});
                if (line == null)
                {
                    line = Instantiate(battle.beamPrefab, battle.runtimeInstances.transform).GetComponent<LineRenderer>();
                }

                StartCoroutine(LineCoroutine(data.targetName));

                handCount--;
                handCountText.text = handCount.ToString();
            }
        }

        IEnumerator LineCoroutine(string targetName)
        {
            line.gameObject.SetActive(true);
            line.SetPositions(new Vector3[]
            {
                    gameObject.transform.position,
                    battle.GetCombatant(targetName).transform.position
            });
            yield return new WaitForSeconds(3);
            line.gameObject.SetActive(false);
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

