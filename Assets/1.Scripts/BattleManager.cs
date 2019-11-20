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

        MainPlayerController mainPlayerController;
        public Transform drawPile;
        public Transform discardPile;
        public Transform handStartPosition;
        public Transform handEndPosition;

        double timeRemaining;
        DateTime endTurnTime;
        public TMP_Text timeRemainingText;

        private void Start()
        {
            network = FindObjectOfType<NetworkInterface>();
            network.Callback_BattleStart += HandleAPICall;
            network.Callback_BattleState += HandleAPICall;
            network.Callback_BattleCardDrawn += HandleAPICall;
            network.Callback_BattlePlayerTurnStart += HandleAPICall;
            network.Callback_BattlePlayerTurnEnd += HandleAPICall;
        }

        private void Update()
        {
            //timeRemaining -= Time.deltaTime;
            //timeRemainingText.text = String.Format("{0:0.00}", timeRemaining);
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
            }
        }

        private void HandleAPICall(PD_BattleCardDrawn data)
        {
            //Debug.Log("Card drawn by " + data.playerName + " : " + data.cardName);
        }

        private void HandleAPICall(PD_BattlePlayerTurnStart data)
        {
            Debug.Log("Turn starts at " + data.startTime.ToString("HH:mm:ss") + ", time now : " + DateTime.UtcNow.ToString("HH:mm:ss"));
            //endTurnTime = data.startTime.AddSeconds(data.turnDuration);
        }

        private void HandleAPICall(PD_BattlePlayerTurnEnd data)
        {
            Debug.Log("Ending turn");
        }

        

    }
}


