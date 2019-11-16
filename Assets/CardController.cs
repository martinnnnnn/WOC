using UnityEngine;





namespace WOC_Client
{
    public class CardController : MonoBehaviour
    {
        public float moveSpeed = 0.1f;
        public bool isSelected = false;
        Vector3 restPos;

        private void Start()
        {
            restPos = transform.position;
        }

        private void Update()
        {
            Vector2 targetPosition = transform.position;

            if (isSelected)
            {
                targetPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            }
            else
            {
                targetPosition = restPos;
            }
            transform.position = Vector2.Lerp(transform.position, targetPosition, moveSpeed);
        }
    }
}

