using System.Collections.Generic;
using TMPro;
using UnityEngine;
using WOC_Core;




namespace WOC_Client
{
    public class MonsterController : MonoBehaviour
    {
        NetworkInterface network;
        BattleManager battle;

        [HideInInspector] public string monsterName;
        public TMP_Text nameText;
        [HideInInspector] public int life;
        public TMP_Text lifeText;

        public void Init(BattleManager battle, PD_BattleStateMonster data)
        {
            this.battle = battle;
            network = FindObjectOfType<NetworkInterface>();
            network.Callback_BattleStateMonster += HandleAPICall;
            network.Callback_BattleMonsterTurnStart += HandleAPICall;
            network.Callback_BattleMonsterTurnEnd += HandleAPICall;
            HandleAPICall(data);
        }

        private void HandleAPICall(PD_BattleStateMonster data)
        {
            transform.position = this.battle.monstersLocations[data.location].position;
            monsterName = data.name;
            nameText.text = monsterName;
            life = data.life;
            lifeText.text = life.ToString();
        }
        private void HandleAPICall(PD_BattleMonsterTurnStart data)
        {
            Debug.Log("monster turn start");
        }
        private void HandleAPICall(PD_BattleMonsterTurnEnd data)
        {
            Debug.Log("monster turn end");
        }

        public void OnDestroy()
        {
            network.Callback_BattleStateMonster -= HandleAPICall;
            network.Callback_BattleMonsterTurnStart -= HandleAPICall;
            network.Callback_BattleMonsterTurnEnd -= HandleAPICall;
        }
    }
}

