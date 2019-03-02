using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Character : MonoBehaviour
{

    public float life;
    public Text lifeDisplay;
    public string title;

    private void Start()
    {
        GetComponent<MeshRenderer>().material.color = new Color(Random.Range(0.0f, 1.0f), Random.Range(0.0f, 1.0f), Random.Range(0.0f, 1.0f));
        ChangeLife(0);
    }

    public void ChangeLife(float value)
    {
        life += value;
        lifeDisplay.text = "Player 1 : " + life;
        if (life <= 0)
        {
            Destroy(gameObject);
        }
    }
}
