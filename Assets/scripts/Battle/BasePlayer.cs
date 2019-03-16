using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace WOC
{
    [RequireComponent(typeof(CameraTransform))]
    public class BasePlayer : MonoBehaviour
    {
        public CameraTransform cameraTransform;

        private void Start()
        {
            cameraTransform = GetComponent<CameraTransform>();
        }


        public virtual void BattleInit()
        {

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

        public virtual void OnAggroChange()
        {
        }
        
    }
}