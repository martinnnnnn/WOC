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

        private void Start()
        {
            network = FindObjectOfType<NetworkInterface>();
            network.Callback_BattleStart += HandleAPICall;
            network.Callback_BattleState += HandleAPICall;
        }

        public void HandleAPICall(PD_BattleStart data)
        {

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
    }
}


