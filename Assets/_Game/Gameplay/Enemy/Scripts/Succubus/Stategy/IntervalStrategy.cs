using Pong.Framework.BehaviourTree;
using UnityEngine;

namespace Pong.Gameplay.Enemy.Succubus
{
    public class IntervalStrategy : INodeStrategy
    {
        private float _interval;
        private float _lastTriggerTime;

        public IntervalStrategy(float interval)
        {
            _interval = interval;
            _lastTriggerTime = Time.time; 
        }

        public Node.Status Process()
        {
            float elapsedTime = Time.time - _lastTriggerTime;
            
            if (elapsedTime >= _interval)
            {
                _lastTriggerTime = Time.time;
                Debug.Log($"Interval triggered! ({_interval}s)");
                return Node.Status.Success;
            }

            return Node.Status.Failure;
        }
    }
}
