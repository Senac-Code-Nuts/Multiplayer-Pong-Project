using UnityEngine;
using Pong.Framework.BehaviourTree;

namespace Pong.Gameplay.Enemy
{
    public class MinotaurPreparationStrategy : EnemyTimerStrategyBase
    {
        private readonly MinotaurEnemy _enemy;

        public MinotaurPreparationStrategy(MinotaurEnemy enemy)
            : base(enemy != null ? enemy.PreAttackTime : 0f)
        {
            _enemy = enemy;
            Reset();
        }

        public override Node.Status Process()
        {
            if (_enemy == null)
            {
                return Node.Status.Failure;
            }

            return base.Process();
        }

        protected override Node.Status WaitingStatus => Node.Status.Running;

        protected override void OnTimerStarted()
        {
            if (_enemy == null)
            {
                return;
            }

            _enemy.BeginPreparation();
        }

        protected override void OnTimerTick()
        {
            _enemy.FaceRelic();
        }

        protected override void OnTimerCompleted()
        {
            if (_enemy == null)
            {
                return;
            }

            _enemy.BeginCounterWindow();
        }

        public override void Reset()
        {
            base.Reset();
        }
    }
}