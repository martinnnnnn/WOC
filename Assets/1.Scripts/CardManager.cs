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
                    var card = hitInfo.transform.GetComponent<CardController>();
                    if (card != null)
                    {
                        current = card;
                        current.isSelected = true;
                    }
                }
            }

            if (Input.GetMouseButtonUp(0))
            {
                if (current != null)
                {
                    RaycastHit hitInfo = new RaycastHit();
                    bool hit = Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hitInfo, Mathf.Infinity, mask);

                    if (battleManager.turnStarted
                        && Time.time + current.timeCost < battleManager.turnEndTime
                        && hit)
                    {
                        MonsterController monsterController = hitInfo.transform.gameObject.GetComponent<MonsterController>();
                        if (monsterController != null)
                        {
                            network.SendMessage(new PD_BattleCardPlayed
                            {
                                eventTime = DateTime.UtcNow,
                                ownerName = current.owner.playerName,
                                targetName = monsterController.monsterName,
                                cardIndex = current.index
                            }, validate: false);
                        }

                        PlayerController playerController = hitInfo.transform.gameObject.GetComponent<PlayerController>();
                        if (playerController != null)
                        {
                            network.SendMessage(new PD_BattleCardPlayed
                            {
                                eventTime = DateTime.UtcNow,
                                ownerName = current.owner.playerName,
                                targetName = playerController.playerName,
                                cardIndex = current.index
                            }, validate: false);
                        }
                    }
                    current.isSelected = false;
                }
                current = null;
                //if (current != null)
                //{
                //    RaycastHit hitInfo = new RaycastHit();
                //    bool hit = Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hitInfo, Mathf.Infinity, mask);
                //    if (hit)
                //    {
                //        Debug.Log("just hit : " + hitInfo.transform.gameObject.name);
                //        MonsterController monsterController = hitInfo.transform.gameObject.GetComponent<MonsterController>();
                //        if (monsterController != null)
                //        {
                //            network.SendMessage(new PD_BattleCardPlayed
                //            {
                //                eventTime = DateTime.UtcNow,
                //                ownerName = current.owner.name,
                //                targetName = "",
                //                cardIndex = current.index
                //            });
                //            current.isSelected = false;
                //            current.useRestPos = false;
                //        }
                //    }
                //}
                //current = null;
            }
        }
    }
}

