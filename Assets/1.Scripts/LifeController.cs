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
        public TMP_Text changeText;

        public int Life
        {
            get
            {
                return life;
            }
            set
            {
                int diff = value - life;
                life = value;
                lifeText.text = life.ToString();

                changeText.gameObject.SetActive(true);
                changeText.text = diff.ToString();
                if (diff < 0)
                {
                    transform.DOShakePosition(0.5f, new Vector3(0, 1, 0));
                    changeText.color = Color.red;
                }
                else
                {
                    changeText.color = Color.green;
                }
                changeText.transform.DOMove(changeText.transform.position + new Vector3(0, 5, 0), 1.5f).OnComplete(() =>
                {
                    changeText.gameObject.SetActive(false);
                });
            }
        }

    }
}

