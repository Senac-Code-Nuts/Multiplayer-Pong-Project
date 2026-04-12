using Pong.Framework.BehaviourTree;

namespace Pong.Gameplay.Enemy
{
    public class AlmaPauseStrategy : EnemyTimerStrategyBase
    {
        public AlmaPauseStrategy(float pauseTime)
            : base(pauseTime)
        {
        }

        protected override Node.Status WaitingStatus => Node.Status.Running;

        protected override Node.Status CompletionStatus => Node.Status.Success;

        public override Node.Status Process()
        {
            return base.Process();
        }
    }
}