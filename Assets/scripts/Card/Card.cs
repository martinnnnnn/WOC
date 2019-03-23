using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;
using DG.Tweening;
using Newtonsoft.Json.Linq;
using System;

namespace WOC
{

    public struct PlayInfo
    {
        public MainPlayer owner;
        public Character target;
    }

    public class Card : MonoBehaviour
    {
        private bool isSelected = false;
        public MainPlayer owner;
        Sequence sequence;

        public CardDescc descc;
        public List<CardEffect> effects = new List<CardEffect>();


        private void Start()
        {
            sequence = DOTween.Sequence();
        }

        public bool Play(PlayInfo playInfo)
        {
            bool result = false;
            foreach (var effect in effects)
            {
                if (effect.Play(playInfo))
                {
                    result = true;
                }
            }
            return result;
        }

        public void Move(Transform endValue, float time)
        {
            sequence.Append(transform.DOMove(endValue.position, time));
            sequence.Join(transform.DORotateQuaternion(endValue.rotation, time));
        }

        public void Move(Vector3 endPosition, Quaternion endRotation, float time)
        {
            sequence.Append(transform.DOMove(endPosition, time));
            sequence.Join(transform.DORotateQuaternion(endRotation, time));
        }

        public bool IsSelected
        {
            get
            {
                return isSelected;
            }
            set
            {
                isSelected = value;
                if (isSelected)
                {
                    GetComponent<MeshRenderer>().material.color = Color.red;
                }
                else
                {
                    GetComponent<MeshRenderer>().material.color = descc.color;
                }
            }
        }


        Color FromHtmlString(string str = "white")
        {
            Color c;
            return ColorUtility.TryParseHtmlString(str, out c) ? c : Color.gray;
        }

        public void ReadJson(JToken jcard)
        {
            descc = gameObject.AddComponent<CardDescc>();
            descc.title = jcard["title"]?.ToString();
            descc.manaCost = jcard["manaCost"] != null ? (int)jcard["manaCost"] : 0;
            descc.exhaust = jcard["exhaust"] != null ? (bool)jcard["exhaust"] : false;
            descc.color = FromHtmlString(jcard["color"]?.ToString());

            foreach (var jeffect in (JArray)jcard["effects"])
            {
                effects.Add(CardEffect.FromJson(jeffect, this));
            }

            GetComponent<MeshRenderer>().material.color = descc.color;
            nameMesh.text = descc.title;
            manaMesh.text = descc.manaCost.ToString();
            exhaustMesh.text = descc.exhaust ? "Exhaust" : "";
            string effectsStr = "";
            foreach (CardEffect e in effects)
            {
                effectsStr += e.ToString();
            }
            effectsMesh.text = effectsStr;
        }

        public TextMesh nameMesh;
        public TextMesh manaMesh;
        public TextMesh effectsMesh;
        public TextMesh exhaustMesh;
    }
}