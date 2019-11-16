using UnityEngine;





namespace WOC_Client
{
    public class CardManager : MonoBehaviour
    {
        CardController current = null;

        private void Update()
        {
            if (Input.GetMouseButtonUp(0))
            {
                if (current != null)
                {
                    current.isSelected = false;
                }
                current = null;
            }

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
        }
    }
}

