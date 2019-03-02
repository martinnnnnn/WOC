using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;
using DG.Tweening;

public class Card : MonoBehaviour
{
    CardDesc desc;
    //public enum TargetType
    //{
    //    CHARACTER,
    //    BATTLEFIELD
    //}

    //public enum Effect
    //{
    //    SILENCE,
    //    ROOT,
    //}

    //public string name = "default name";
    //public List<Effect> effects;
    //public float damage;
    //public int manaCost;
    //TargetType targetType;

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
                GetComponent<MeshRenderer>().material.color = Color.red;
            }   
            else
            {
                //GetComponent<MeshRenderer>().material.color = color;
            }
        }
    }

    public Player owner;
    //Color color;

    Sequence sequence;
    private void Start()
    {
        desc = GetComponent<CardDesc>();
        //color = new Color(Random.Range(0.0f, 1.0f), Random.Range(0.0f, 1.0f), Random.Range(0.0f, 1.0f));
        //GetComponent<MeshRenderer>().material.color = color;
        //manaCost = Random.Range(0, 10);
        //GetComponentInChildren<TextMesh>().text = manaCost.ToString();
        sequence = DOTween.Sequence();
    }


    public bool DoEffect(GameObject target)
    {
        //bool effectDone = false;

        //if (owner.mana >= manaCost)
        //{
        //    switch (targetType)
        //    {
        //        case TargetType.BATTLEFIELD:
        //            effectDone = DoEffectBattlefield();
        //            break;
        //        case TargetType.CHARACTER:
        //            effectDone = DoEffectCharacter(target);
        //            break;
        //    }
        //}

        //return effectDone;
        return true;
    }

    private bool DoEffectCharacter(GameObject target)
    {
        //if (!target)
        //{
        //    return false;
        //}
        //Character targetChara = target.GetComponent<Character>();
        //if (!targetChara)
        //{
        //    return false;
        //}
        //targetChara.ChangeLife(-damage);
        return true;
    }

    private bool DoEffectBattlefield()
    {
        return true;
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
