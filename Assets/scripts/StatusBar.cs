using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace WOC
{
    public class StatusBar : MonoBehaviour
    {
        [Range(0, 20)]
        public float sizeX;
        [Range(0, 20)]
        public float sizeY;

        private Transform background;
        private Transform bar;
        private Transform barSprite;

        private void Start()
        {
            background = transform.Find("Background");
            bar = transform.Find("Bar");
            barSprite = transform.Find("BarSprite");

            background.localScale = new Vector3(sizeX, sizeY, 1);
            barSprite.localScale = new Vector3(sizeX, sizeY, 1);
            barSprite.position = new Vector3(sizeX / 2, 0, 0);
            bar.position = new Vector3(-sizeX / 2, 0, 0);
        }

        public void Set(float value)
        {
            bar.localScale = new Vector3(value, 1, 1);
        }

        void LateUpdate()
        {
            transform.LookAt(transform.position + Camera.main.transform.rotation * Vector3.forward,
                Camera.main.transform.rotation * Vector3.up);
        }
    }
}