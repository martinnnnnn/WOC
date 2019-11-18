using UnityEngine;


using WOC_Core;


namespace WOC_Client
{
    //public class CardManager : MonoBehaviour
    //{
    //    CardController current = null;
    //    public LayerMask mask;
    //    NetworkInterface network;

    //    private void Start()
    //    {
    //        network = FindObjectOfType<NetworkInterface>();
    //    }

    //    private void Update()
    //    {
    //        if (Input.GetMouseButtonDown(0))
    //        {
    //            RaycastHit hitInfo = new RaycastHit();
    //            bool hit = Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hitInfo);
    //            if (hit)
    //            {
    //                var card = hitInfo.transform.GetComponent<CardController>();
    //                if (card != null)
    //                {
    //                    current = card;
    //                    current.isSelected = true;
    //                }
    //            }
    //        }

    //        if (Input.GetMouseButtonUp(0))
    //        {
    //            if (current != null)
    //            {
    //                RaycastHit hitInfo = new RaycastHit();
    //                bool hit = Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hitInfo, Mathf.Infinity, mask);
    //                if (hit)
    //                {
    //                    //Debug.Log("just hit : " + hitInfo.transform.gameObject.name);
    //                    //MonsterController monsterController = hitInfo.transform.gameObject.GetComponent<MonsterController>();
    //                    //if (monsterController != null)
    //                    //{
    //                    //    network.SendMessage(new PD_BattlePlayCard
    //                    //    {
    //                    //        ownerName = current.owner.name,
    //                    //        targetName = monsterController.monster.name,
    //                    //        cardIndex = current.index
    //                    //    });
    //                    //}
    //                }
    //                current.isSelected = false;
    //            }
    //            current = null;
    //        }
    //    }
    //}
}

