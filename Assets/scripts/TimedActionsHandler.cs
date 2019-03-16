using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace WOC
{
    public enum ActionQueue
    {
        BattleFlow,
    }

    class TimedActionsHandler : Singleton<TimedActionsHandler>
    {
        Dictionary<ActionQueue, CoroutineQueue> actionQueues;

        protected TimedActionsHandler()
        {
            actionQueues = new Dictionary<ActionQueue, CoroutineQueue>()
            {
                { ActionQueue.BattleFlow, new CoroutineQueue(this) }
            }; 

            foreach (var queue in actionQueues.Values)
            {
                queue.StartLoop();
            }
        }

        public void Add(ActionQueue type, IEnumerator action)
        {
            actionQueues[type].EnqueueAction(action);
        }

        public void AddWait(ActionQueue type, float duration)
        {
            actionQueues[type].EnqueueWait(duration);
        }
    }
}