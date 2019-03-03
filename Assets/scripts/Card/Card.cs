using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;
using DG.Tweening;

public class Card : MonoBehaviour
{
    CardDesc desc;

    private bool isSelected = false;
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
               desc.GetComponent<MeshRenderer>().material.color = Color.red;
            }   
            else
            {
                desc.GetComponent<MeshRenderer>().material.color = desc.Color;
            }
        }
    }

    public Player owner;

    Sequence sequence;
    private void Start()
    {
        desc = GetComponent<CardDesc>();
        sequence = DOTween.Sequence();
    }

    public bool Play(CardEffect.PlayInfo playInfo)
    {
        return desc.Play(playInfo);
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
}
