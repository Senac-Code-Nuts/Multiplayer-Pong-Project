using Pong.Framework.BehaviourTree;
using Pong.Gameplay.Boss;
using Pong.Gameplay.Enemy;
using UnityEngine;

namespace Pong.Gameplay
{
    public class EnvyVulnerableTimerStrategy : EnemyTimerStrategyBase
    {
        private EnvyBoss _envyBoss;
        public EnvyVulnerableTimerStrategy(EnvyBoss boss, float duration) : base(duration)
        {
            _envyBoss = boss;
        }
        protected override Node.Status WaitingStatus => Node.Status.Running;

        protected override void OnTimerStarted()
        {
            _envyBoss.EnterVulnerableState();
        }

        protected override void OnTimerCompleted()
        {
            _envyBoss.ExitVulnerableState();
        }
    }
}
