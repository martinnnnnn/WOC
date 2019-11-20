﻿using TMPro;
using UnityEngine;

using WOC_Core;



namespace WOC_Client
{
    public class CardController : MonoBehaviour
    {
        public float moveSpeed = 0.1f;
        public bool isSelected = false;
        public Vector3 restPosition;
        public bool useRestPos = false;

        public MainPlayerController owner;
        public int index = -1;

        [HideInInspector] public int timeCost;
        public TMP_Text timeCostText;

        public void Init(MainPlayerController owner, PD_BattleCardDrawn data, Vector3 initPosition)
        {
            this.owner = owner;
            transform.position = initPosition;
            timeCost = data.timeCost;
            timeCostText.text = "" + timeCost;
        }

        private void Update()
        {
            Vector2 targetPosition = transform.position;

            if (isSelected)
            {
                transform.position = Vector2.Lerp(transform.position, Camera.main.ScreenToWorldPoint(Input.mousePosition), moveSpeed);
            }
            else if (useRestPos)
            {
                transform.position = Vector2.Lerp(transform.position, restPosition, moveSpeed);
            }

        }
    }
}
