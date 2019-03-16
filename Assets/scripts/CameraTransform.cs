using UnityEngine;
using DG.Tweening;

namespace WOC
{
    public class CameraTransform : MonoBehaviour
    {
        public Transform cameraTransform;
        public float cameraSwitchTime;

        void SetCamera(System.Action callback)
        {
            Sequence sequence = DOTween.Sequence();
            sequence.Append(Camera.main.transform.DOMove(cameraTransform.position, cameraSwitchTime));
            sequence.Join(Camera.main.transform.DORotate(cameraTransform.rotation.eulerAngles, cameraSwitchTime));
            sequence.OnComplete(() => callback());
        }
    }
}

