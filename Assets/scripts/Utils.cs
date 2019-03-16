using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace WOC
{
    public static class Utils
    {
        public static Transform FindDeepChild(this Transform aParent, string aName)
        {
            Queue<Transform> queue = new Queue<Transform>();
            queue.Enqueue(aParent);
            while (queue.Count > 0)
            {
                var c = queue.Dequeue();
                if (c.name == aName)
                    return c;
                foreach (Transform t in c)
                    queue.Enqueue(t);
            }
            return null;
        }

        public static float GetClipDuration(Animator animator, string clipname)
        {
            foreach (AnimationClip ac in animator.runtimeAnimatorController.animationClips)
            {
                if (ac.name == clipname)
                {
                    return ac.length;
                }
            }
            return 0.0f;
        }
    };
}