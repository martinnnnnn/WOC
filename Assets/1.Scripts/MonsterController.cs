using System.Collections.Generic;
using TMPro;
using UnityEngine;
using WOC_Core;
using DG.Tweening;



namespace WOC_Client
{
    public class MonsterController : MonoBehaviour
    {
        NetworkInterface network;
        BattleManager battle;
        int location;

        [HideInInspector] public string monsterName;
        public TMP_Text nameText;
        [HideInInspector] public LifeController life;
        [HideInInspector] public int mana;
        public TMP_Text manaText;


        public void Init(BattleManager battle, PD_BattleStateMonster data)
        {
            this.battle = battle;
            location = data.location;
            life = GetComponent<LifeController>();
            network = FindObjectOfType<NetworkInterface>();
            network.Callback_BattleStateMonster += HandleAPICall;
            network.Callback_BattleMonsterTurnStart += HandleAPICall;
            network.Callback_BattleMonsterTurnEnd += HandleAPICall;
            network.Callback_BattleMonsterDead += HandleAPICall;
            network.Callback_BattlePlayerTurnStart += HandleAPICall;
            HandleAPICall(data);
        }

        private void HandleAPICall(PD_BattleStateMonster data)
        {
            transform.position = this.battle.monstersLocations[data.location].position;
            if (data.location > 0)
            {
                transform.localScale = new Vector3(0.6f, 0.6f, 0.6f);
            }
            monsterName = data.name;
            nameText.text = monsterName;
            life.Life = data.life;
        }

        private void HandleAPICall(PD_BattleMonsterDead data)
        {
            if (data.monsterName == monsterName)
            {
                life.Life = 0;
                nameText.color = Color.gray;
                GetComponentInChildren<SpriteRenderer>().color = Color.gray;
                transform.DORotate(new Vector3(-30, 0, 0), 1.0f);
            }
        }

        private void HandleAPICall(PD_BattleMonsterTurnStart data)
        {
            Debug.Log("monster turn start");
        }
        private void HandleAPICall(PD_BattleMonsterTurnEnd data)
        {
            Debug.Log("monster turn end");
        }

        private void HandleAPICall(PD_BattlePlayerTurnStart data)
        {
            mana = data.manas[location];
            manaText.text = mana.ToString();
        }

        public void OnDestroy()
        {
            network.Callback_BattleStateMonster -= HandleAPICall;
            network.Callback_BattleMonsterTurnStart -= HandleAPICall;
            network.Callback_BattleMonsterTurnEnd -= HandleAPICall;
        }
    }
}

