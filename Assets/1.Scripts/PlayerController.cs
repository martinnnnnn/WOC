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

        public TMP_Text nameText;
        public TMP_Text lifeText;

        public void Init(BattleManager battle, PD_BattleStatePlayer data)
        {
            this.battle = battle;
            network = FindObjectOfType<NetworkInterface>();
            network.Callback_BattleStatePlayer += HandleAPICall;
            HandleAPICall(data);
        }

        private void HandleAPICall(PD_BattleStatePlayer data)
        {
            transform.position = this.battle.playersLocations[data.location].position;
            nameText.text = data.name;
            lifeText.text = data.life.ToString();
        }
    }
}

