using System;
using UnityEngine;


using WOC_Core;


namespace WOC_Client
{
    public class CardManager : MonoBehaviour
    {
        CardController current = null;
        public LayerMask mask;
        NetworkInterface network;
        public BattleManager battleManager;
        LineRenderer line;
        GameObject target = null;

        private void Start()
        {
            network = FindObjectOfType<NetworkInterface>();
            //battleManager.GetComponent<BattleManager>();
        }

        private void Update()
        {
            //if (battleManager == null) battleManager.GetComponent<BattleManager>();

            if (Input.GetMouseButtonDown(0))
            {
                RaycastHit hitInfo = new RaycastHit();
                bool hit = Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hitInfo);
                if (hit)
                {
                    var card = hitInfo.transform.GetComponentInParent<CardController>();
                    if (card != null)
                    {
                        current = card;
                        current.isSelected = true;
                    }
                }
            }

            if (battleManager.mainPlayerController?.life.Life <= 0)
            {
                current = null;
            }

            if (Input.GetMouseButtonUp(0))
            {
                if (current != null)
                {
                    RaycastHit hitInfo = new RaycastHit();
                    bool hit = Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hitInfo, Mathf.Infinity, mask);

                    if (battleManager.turnStarted
                        && Time.time + current.timeCost < battleManager.turnEndTime)
                    {
                        string targetName = "";
                        if (hit)
                        {
                            MonsterController monsterController = hitInfo.transform.gameObject.GetComponentInParent<MonsterController>();
                            if (monsterController != null)
                            {
                                targetName = monsterController.monsterName;
                            }

                            PlayerController playerController = hitInfo.transform.gameObject.GetComponentInParent<PlayerController>();
                            if (playerController != null)
                            {
                                targetName = playerController.playerName;
                            }

                            MainPlayerController mainPlayerController = hitInfo.transform.gameObject.GetComponentInParent<MainPlayerController>();
                            if (mainPlayerController != null)
                            {
                                targetName = mainPlayerController.playerName;
                            }
                        }
                        network.SendMessage(new PD_BattleCardPlayed
                        {
                            eventTime = DateTime.UtcNow,
                            ownerName = current.owner.playerName,
                            targetName = targetName,
                            cardIndex = current.index
                        }, validate: false);
                    }
                    current.isSelected = false;
                }
                current = null;
            }

            if (line == null)
            {
                line = Instantiate(battleManager.beamPrefab, battleManager.runtimeInstances.transform).GetComponent<LineRenderer>();
                line.transform.position = Vector3.zero;
            }

            if (current != null)
            {
                line.gameObject.SetActive(true);

                line.SetPositions(new Vector3[]
                {
                    current.transform.position,
                    Camera.main.ScreenToWorldPoint(Input.mousePosition) - new Vector3(0, 0, Camera.main.transform.position.z  + 3)
                });

                switch (current.type)
                {
                    case "attack":
                        line.startColor = Color.red;
                        line.endColor = Color.red;
                        break;
                    case "heal":
                        line.startColor = Color.green;
                        line.endColor = Color.green;
                        break;
                    case "draw":
                        line.startColor = Color.blue;
                        line.endColor = Color.blue;
                        break;
                }
            }
            else
            {
                line.gameObject.SetActive(false);
            }
        }
    }
}

