using System.Collections.Generic;
using TMPro;
using UnityEngine;





namespace WOC_Client
{
    public class MonsterController : MonoBehaviour
    {
        public WOC_Core.RTTS.Monster monster;

        public void SetMonster(WOC_Core.RTTS.Monster monster)
        {
            this.monster = monster;
            GetComponentInChildren<TMP_Text>().text = this.monster.name;
        }
    }
}

