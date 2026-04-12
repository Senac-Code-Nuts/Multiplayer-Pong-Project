using UnityEngine;
using Pong.Framework.BehaviourTree;

namespace Pong.Gameplay.Enemy
{
    public class AlmaPauseStrategy : INodeStrategy
    {
        private readonly float _pauseTime;
        private float _timer;

        public AlmaPauseStrategy(float pauseTime)
        {
            _pauseTime = Mathf.Max(0f, pauseTime);
            Reset();
        }

        public Node.Status Process()
        {
            _timer += Time.deltaTime;

            if (_timer >= _pauseTime)
            {
                Reset();
                return Node.Status.Success;
            }

            return Node.Status.Running;
        }

        public void Reset()
        {
            _timer = 0f;
        }
    }
}