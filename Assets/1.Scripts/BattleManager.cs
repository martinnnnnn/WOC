using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using WOC_Core;
using WOC_Client;
using System.Threading.Tasks;
using System;
using System.Threading;
using System.Net.Sockets;
using System.Text;
using System.Net;
using UnityEditor;
using UnityEngine.UI;
using TMPro;

namespace WOC_Client
{
    public class BattleManager : MonoBehaviour
    {
        NetworkInterface network;
        public Transform[] playersLocations;
        public GameObject playerPrefab;
        public GameObject mainPlayerPrefab;
        public Transform[] monstersLocations;
        public GameObject monsterPrefab;

        MainPlayerController mainPlayerController;
        public Transform drawPile;
        public Transform discardPile;
        public Transform handStartPosition;
        public Transform handEndPosition;

        [HideInInspector] public List<PlayerController> playersControllers = new List<PlayerController>();
        [HideInInspector] public List<MonsterController> monstersControllers = new List<MonsterController>();


        [HideInInspector] public bool turnStarted = false;
        [HideInInspector] public bool turnBootStarted = false;
        float turnStartTime;
        [HideInInspector] public float turnEndTime;
        public TMP_Text timeRemainingText;

        private void Start()
        {
            network = FindObjectOfType<NetworkInterface>();
            network.Callback_BattleStart += HandleAPICall;
            network.Callback_BattleState += HandleAPICall;
            network.Callback_BattleCardDrawn += HandleAPICall;
            network.Callback_BattleCardPlayed += HandleAPICall;
            network.Callback_BattlePlayerTurnStart += HandleAPICall;
            network.Callback_BattlePlayerTurnEnd += HandleAPICall;
            network.Callback_BattleMonsterTurnStart += HandleAPICall;
            network.Callback_BattleMonsterTurnEnd += HandleAPICall;

            timeRemainingText.gameObject.SetActive(false);
        }

        private void Update()
        {
            if (turnBootStarted)
            {
                if (Time.time >= turnStartTime)
                {
                    turnStarted = true;
                    timeRemainingText.gameObject.SetActive(true);
                    timeRemainingText.text = String.Format("{0:0.00}", turnEndTime - Time.time);
                }
                if (Time.time >= turnEndTime)
                {
                    turnStarted = false;
                    turnBootStarted = false;
                    timeRemainingText.gameObject.SetActive(false);
                    network.SendMessage(new PD_BattlePlayerTurnEnd
                    {
                        playerName = mainPlayerController.playerName,
                        eventTime = DateTime.UtcNow
                    });
                }
            }
        }

        public void HandleAPICall(PD_BattleStart data)
        {
            network.SendMessage(new PD_BattleState(), validate: false);
        }

        public void HandleAPICall(PD_BattleState data)
        {
            GameObject newMainPlayer = Instantiate(mainPlayerPrefab, this.transform);
            mainPlayerController = newMainPlayer.GetComponent<MainPlayerController>();
            mainPlayerController.Init(this, data.mainPlayer);

            foreach (var player in data.players)
            {
                GameObject newPlayer = Instantiate(playerPrefab, this.transform);
                PlayerController newPlayerController = newPlayer.GetComponent<PlayerController>();
                newPlayerController.Init(this, player);
                playersControllers.Add(newPlayerController);
            }

            foreach (var monster in data.monsters)
            {
                GameObject newMonster = Instantiate(monsterPrefab, this.transform);
                MonsterController newMonsterController = newMonster.GetComponent<MonsterController>();
                newMonsterController.Init(this, monster);
                monstersControllers.Add(newMonsterController);
            }
        }

        private void HandleAPICall(PD_BattleCardDrawn data)
        {
            //Debug.Log("Card drawn by " + data.playerName + " : " + data.cardName);
        }

        private void HandleAPICall(PD_BattleCardPlayed data)
        {
            data.effects.ForEach(e =>
            {
                switch (e)
                {
                    case PD_BattleCardEffectDamage damage:
                        MonsterController monster = monstersControllers.Find(m => m.monsterName == data.targetName);
                        monster.life -= damage.value;
                        monster.lifeText.text = monster.life.ToString();
                        break;
                    case PD_BattleCardEffectHeal heal:
                        PlayerController player = playersControllers.Find(p => p.playerName == data.targetName);
                        player.life += heal.value;
                        player.lifeText.text = player.life.ToString();
                        break;
                    case PD_BattleCardEffectDraw draw:
                        break;
                }

            });
            
        }


        private void HandleAPICall(PD_BattlePlayerTurnStart data)
        {
            Debug.Log("Turn starts in " + data.startTime.Subtract(DateTime.UtcNow).TotalSeconds + " seconds");
            turnStartTime = Time.time + (float)data.startTime.Subtract(DateTime.UtcNow).TotalSeconds;
            turnEndTime = turnStartTime + (float)data.turnDuration;
            turnBootStarted = true;
        }

        private void HandleAPICall(PD_BattleMonsterTurnStart data)
        {
            
        }
        private void HandleAPICall(PD_BattleMonsterTurnEnd data)
        {

        }

        private void HandleAPICall(PD_BattlePlayerTurnEnd data)
        {
        }


    }
}


