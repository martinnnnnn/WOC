using System.Collections.Generic;
using TMPro;
using UnityEngine;
using WOC_Core;




namespace WOC_Client
{
    public class PlayerController : MonoBehaviour
    {
        NetworkInterface network;
        public BattleManager battle;

        public TMP_Text lifeText;

        private void Start()
        {
            network = FindObjectOfType<NetworkInterface>();
            network.Callback_BattleStatePlayer += HandleAPICall;
        }

        public void Init(BattleManager battle, PD_BattleStatePlayer data)
        {
            this.battle = battle;
            HandleAPICall(data);
        }

        private void HandleAPICall(PD_BattleStatePlayer data)
        {
            transform.position = this.battle.playersLocations[data.location].position;
            lifeText.text = data.life.ToString();
        }
    }
}

