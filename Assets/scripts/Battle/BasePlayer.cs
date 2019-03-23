using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace WOC
{
    [RequireComponent(typeof(CameraTransform))]
    public class BasePlayer : MonoBehaviour
    {
        [HideInInspector] public CameraTransform cameraTransform;
        [HideInInspector] public Character character;


        public virtual void BattleInit()
        {
            cameraTransform = GetComponent<CameraTransform>();
            character = GetComponentInChildren<Character>();
        }

        public virtual void BattleEnd()
        {

        }

        public virtual void StartTurn()
        {

        }

        public virtual void PlayTurn()
        {

        }

        public virtual void EndTurn()
        {

        }

        public virtual void OnAggroChange(Character newBiggestAggro)
        {

        }

    }
}