using Pong.Gameplay.Boss;
using Pong.Gameplay.Enemy;
using Pong.Framework.BehaviourTree;
using UnityEngine;

namespace Pong.Gameplay.Boss
{
    public class WrathSpinAttackStrategy : EnemyTimerStrategyBase
    {
        private WrathBoss _boss;

        public WrathSpinAttackStrategy(WrathBoss boss) : base(2f)
        {
            _boss = boss;
        }

        protected override Node.Status WaitingStatus => Node.Status.Running;

        protected override void OnTimerStarted()
        {
            _boss.SetVulnerable(false);
            _boss.StopMovement();
            _boss.ShowAttackArea();
        }

        protected override void OnTimerCompleted()
        {
            _boss.ExecuteSpinAttack();
            _boss.SetVulnerable(true);
        }
    }
}
