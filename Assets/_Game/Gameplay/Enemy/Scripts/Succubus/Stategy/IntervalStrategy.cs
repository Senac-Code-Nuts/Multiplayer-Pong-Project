using Pong.Framework.BehaviourTree;
using UnityEngine;

namespace Pong.Gameplay.Enemy.Succubus
{
    public class IntervalStrategy : INodeStrategy
    {
        private float _interval;
        private float _lastTriggerTime;
        private bool _isWaiting = false; 

        public IntervalStrategy(float interval)
        {
            _interval = interval;
        }

        public Node.Status Process()
        {
            if (!_isWaiting)
            {
                _lastTriggerTime = Time.time;
                _isWaiting = true;
            }

            float elapsedTime = Time.time - _lastTriggerTime;
            
            if (elapsedTime >= _interval)
            {
                _isWaiting = false; 
                return Node.Status.Success;
            }

            return Node.Status.Failure;
        }
    }
}