using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using WOC_Core;
using DG.Tweening;



namespace WOC_Client
{
    public class LifeController : MonoBehaviour
    {
        int life;
        public TMP_Text lifeText;

        
        public int Life
        {
            get
            {
                return life;
            }
            set
            {
                life = value;
                lifeText.text = life.ToString();
            }
        }

    }
}

