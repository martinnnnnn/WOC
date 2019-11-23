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
using DG.Tweening;

namespace WOC_Client
{
    public class BattleManager : MonoBehaviour
    {
        NetworkInterface network;
        public Transform runtimeInstances;

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

        [HideInInspector] public bool isOngoing = false;
        public GameObject beamPrefab;

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
            network.Callback_BattleMonsterAttack += HandleAPICall;
            network.Callback_BattleMonsterTurnEnd += HandleAPICall;
            network.Callback_BattleEnd += HandleAPICall;
            
            timeRemainingText.gameObject.SetActive(false);
        }

        private void Update()
        {
            if (turnBootStarted)
            {
                if (Time.time >= turnStartTime)
                {
                    turnStarted = true;
                    float timeRemaining = turnEndTime - Time.time;

                    timeRemainingText.color = Color.white;
                    if (timeRemaining < 10.0f)
                    {
                        if ((int)(timeRemaining * 10) % 4 == 0)
                        {
                            timeRemainingText.color = Color.red;
                        }
                    }

                    timeRemainingText.gameObject.SetActive(true);
                    timeRemainingText.text = String.Format("{0:0}", turnEndTime - Time.time);
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
            isOngoing = true;
            network.SendMessage(new PD_BattleState(), validate: false);
        }

        public void HandleAPICall(PD_BattleEnd data)
        {
            isOngoing = false;
            turnEndTime = Time.time;
            foreach (Transform t in runtimeInstances)
            {
                Destroy(t.gameObject);
            }
        }

        public void HandleAPICall(PD_BattleState data)
        {
            GameObject newMainPlayer = Instantiate(mainPlayerPrefab, runtimeInstances);
            mainPlayerController = newMainPlayer.GetComponent<MainPlayerController>();
            mainPlayerController.Init(this, data.mainPlayer);

            foreach (var player in data.players)
            {
                GameObject newPlayer = Instantiate(playerPrefab, runtimeInstances);
                PlayerController newPlayerController = newPlayer.GetComponent<PlayerController>();
                newPlayerController.Init(this, player);
                playersControllers.Add(newPlayerController);
            }

            foreach (var monster in data.monsters)
            {
                GameObject newMonster = Instantiate(monsterPrefab, runtimeInstances);
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
            // TODO : add these effects to network, call network and handle behaviour inside controller classes.
            data.effects.ForEach(e =>
            {
                switch (e)
                {
                    case PD_BattleCardEffectDamage damage:
                    {
                        MonsterController monster = monstersControllers.Find(m => m.monsterName == data.targetName);
                        if (monster)
                        {
                            monster.life.Life -= damage.value;
                        }
                        PlayerController player = playersControllers.Find(p => p.playerName == data.targetName);
                        if (player)
                        {
                            player.life.Life -= damage.value;
                        }
                        if (mainPlayerController.playerName == data.targetName)
                        {
                            mainPlayerController.life.Life -= damage.value;
                        }
                        break;
                    }
                    case PD_BattleCardEffectHeal heal:
                    {
                        MonsterController monster = monstersControllers.Find(m => m.monsterName == data.targetName);
                        if (monster)
                        {
                            monster.life.Life += heal.value;
                        }
                        PlayerController player = playersControllers.Find(p => p.playerName == data.targetName);
                        if (player)
                        {
                            player.life.Life += heal.value;
                        }
                        if (mainPlayerController.playerName == data.targetName)
                        {
                            mainPlayerController.life.Life += heal.value;
                        }
                        break;
                    }
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

        private void HandleAPICall(PD_BattleMonsterAttack data)
        {
            var monster = monstersControllers.Find(m => m.monsterName == data.monster);
            var player = playersControllers.Find(m => m.playerName == data.target);
            if (player != null)
            {
                player.life.Life -= data.damage;
                StartCoroutine(DamageFX(monster.transform.position, player.transform.position));
            }
            else if (mainPlayerController.playerName == data.target)
            {
                mainPlayerController.life.Life -= data.damage;
                StartCoroutine(DamageFX(monster.transform.position, mainPlayerController.transform.position));
            }

        }

        private void HandleAPICall(PD_BattleMonsterTurnEnd data)
        {

        }

        private void HandleAPICall(PD_BattlePlayerTurnEnd data)
        {
        }

        public void OnDestroy()
        {
            network.Callback_BattleStart -= HandleAPICall;
            network.Callback_BattleState -= HandleAPICall;
            network.Callback_BattleCardDrawn -= HandleAPICall;
            network.Callback_BattleCardPlayed -= HandleAPICall;
            network.Callback_BattlePlayerTurnStart -= HandleAPICall;
            network.Callback_BattlePlayerTurnEnd -= HandleAPICall;
            network.Callback_BattleMonsterTurnStart -= HandleAPICall;
            network.Callback_BattleMonsterTurnEnd -= HandleAPICall;
            network.Callback_BattleEnd -= HandleAPICall;
        }

        IEnumerator DamageFX(Vector3 position1, Vector3 position2)
        {
            LineRenderer beam = Instantiate(beamPrefab, runtimeInstances.transform).GetComponent<LineRenderer>();
            beam.transform.position = -runtimeInstances.position;
            beam.SetPositions(new Vector3[]
            {
                position1,
                position2
            });

            yield return new WaitForSeconds(4);
            Destroy(beam.gameObject);
        }

        public GameObject GetCombatant(string name)
        {
            GameObject combatant = null;
            if (mainPlayerController.playerName == name)
            {
                combatant = mainPlayerController.gameObject;
            }
            var player = playersControllers.Find(p => p.playerName == name);
            if (player != null)
            {
                combatant = player.gameObject;
            }
            var monster = monstersControllers.Find(m => m.monsterName == name);
            if (monster != null)
            {
                combatant = monster.gameObject;
            }
            return combatant;
        }
    }
}


