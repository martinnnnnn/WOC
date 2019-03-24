﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace WOC
{
    public class Character : MonoBehaviour
    {

        public float life;
        public Text lifeDisplay;
        public string title;

        private void Start()
        {
            MeshRenderer renderer = GetComponent<MeshRenderer>();
            if (renderer) renderer.material.color = new Color(Random.Range(0.0f, 1.0f), Random.Range(0.0f, 1.0f), Random.Range(0.0f, 1.0f));
            ChangeLife(0);
        }

        public void ChangeLife(float value)
        {
            life += value;
            lifeDisplay.text = title + " : " + life;
            if (life <= 0)
            {
                Destroy(gameObject);
            }
        }
    }
}