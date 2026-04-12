using UnityEngine;
using Pong.Framework.BehaviourTree;

namespace Pong.Gameplay.Enemy
{
    public class MinotaurCounterWindowStrategy : INodeStrategy
    {
        private readonly MinotaurEnemy _enemy;
        private float _timer;
        private bool _isArmed;
        private bool _isCharging;

        public bool IsArmed => _isArmed;        public MinotaurCounterWindowStrategy(MinotaurEnemy enemy)
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

            if (!_isCharging && !_isArmed)
            {
                _isCharging = true;
                _timer = 0f;
                _enemy.BeginPreparation();
                Debug.Log("<color=yellow>[Minotaur] Charging counter while chasing the relic.</color>");
            }

            if (_isCharging)
            {
                _timer += Time.deltaTime;

                if (_timer < _enemy.PreAttackTime)
                {
                    return Node.Status.Failure;
                }

                _isCharging = false;
                _isArmed = true;
                _timer = 0f;
                _enemy.BeginCounterWindow();
                Debug.Log("<color=cyan>[Minotaur] Counter window armed. Still chasing the relic.</color>");
                return Node.Status.Failure;
            }

            _enemy.ContinueRelicMovement();

            if (_enemy.ConsumeCounterAttackExecuted())
            {
                Debug.Log("<color=lime>[Minotaur] Parry confirmed. Counter attack executed.</color>");
                Reset();
                return Node.Status.Success;
            }

            _timer += Time.deltaTime;

            if (_timer >= _enemy.CounterWindowTimeout)
            {
                Debug.Log("<color=orange>[Minotaur] Counter window expired without parry.</color>");
                Reset();
                return Node.Status.Failure;
            }

            return Node.Status.Running;
        }

        public void Reset()
        {
            _timer = 0f;
            _isArmed = false;
            _isCharging = false;

            if (_enemy == null)
            {
                return;
            }

            _enemy.SetCounterAttackReady(false);
            _enemy.SetVulnerable(true);
            _enemy.SetCycleColor(Color.gray);
        }
    }
}