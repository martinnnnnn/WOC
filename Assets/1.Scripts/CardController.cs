using TMPro;
using UnityEngine;

using WOC_Core;



namespace WOC_Client
{
    public class CardController : MonoBehaviour
    {
        public float moveSpeed = 0.5f;
        public bool isSelected = false;
        public Vector3 restPosition;
        public bool useRestPos = false;

        public MainPlayerController owner;
        public int index = -1;

        public TMP_Text nameText;

        [HideInInspector] public int timeCost;
        public TMP_Text timeCostText;

        public Sprite attackSprite;
        public Sprite healSprite;
        public Sprite drawSprite;
        [HideInInspector] public string type;

        public void Init(MainPlayerController owner, PD_BattleCardDrawn data, Vector3 initPosition)
        {
            this.owner = owner;
            transform.position = initPosition;
            nameText.text = "";
            type = data.cardName;

            switch (data.cardName)
            {
                case "attack":
                    GetComponentInChildren<SpriteRenderer>().sprite = attackSprite;
                    break;
                case "heal":
                    GetComponentInChildren<SpriteRenderer>().sprite = healSprite;
                    break;
                case "draw":
                    GetComponentInChildren<SpriteRenderer>().sprite = drawSprite;
                    break;
            }
            nameText.text = data.cardName;
            timeCost = data.timeCost;
            timeCostText.text = "" + timeCost;
        }

        private void Update()
        {
            // position
            //Vector2 targetPosition = transform.position;
            //if (isSelected)
            //{
            //    transform.position = Vector2.Lerp(transform.position, Camera.main.ScreenToWorldPoint(Input.mousePosition), moveSpeed);
            //}
            //else if (useRestPos)
            //{
            //    transform.position = Vector2.Lerp(transform.position, restPosition, moveSpeed);
            //}

            // color
            timeCostText.color = Color.black;
            if (Time.time + timeCost >= owner.battle.turnEndTime)
            {
                timeCostText.color = Color.red;
            }
        }
    }
}

