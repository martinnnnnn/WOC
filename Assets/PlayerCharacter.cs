using UnityEngine;





namespace WOC_Client
{
    public class PlayerCharacter : MonoBehaviour
    {
        


        private void Update()
        {
            Vector3 position = transform.position;

            if (Input.GetKey(KeyCode.Z))
            {
                position.y += 1;
            }
            if (Input.GetKey(KeyCode.S))
            {
                position.y -= 1;
            }
            if (Input.GetKey(KeyCode.D))
            {
                position.x += 1;
            }
            if (Input.GetKey(KeyCode.Q))
            {
                position.x -= 1;
            }

            transform.position = position;
        }
    }
}

