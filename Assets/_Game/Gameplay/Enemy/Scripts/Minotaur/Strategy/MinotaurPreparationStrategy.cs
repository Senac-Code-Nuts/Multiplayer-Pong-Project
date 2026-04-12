using UnityEngine;
using Pong.Framework.BehaviourTree;

namespace Pong.Gameplay.Enemy
{
    public class MinotaurPreparationStrategy : INodeStrategy
    {
        private readonly MinotaurEnemy _enemy;
        private float _timer;
        private bool _isPreparing;

        public MinotaurPreparationStrategy(MinotaurEnemy enemy)
        {
            _enemy = enemy;
            Reset();
        }

        public Node.Status Process()
        {
            if (_enemy == null)
            {
                return Node.Status.Failure;
            }

            if (!_isPreparing)
            {
                _isPreparing = true;
                _timer = 0f;
                _enemy.BeginPreparation();
            }

            _enemy.FaceRelic();
            _timer += Time.deltaTime;

            if (_timer >= _enemy.PreAttackTime)
            {
                _enemy.BeginCounterWindow();
                Reset();
                return Node.Status.Success;
            }

            return Node.Status.Running;
        }

        public void Reset()
        {
            _timer = 0f;
            _isPreparing = false;
        }
    }
}