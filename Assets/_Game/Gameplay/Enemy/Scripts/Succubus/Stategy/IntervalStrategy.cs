using Pong.Framework.BehaviourTree;

namespace Pong.Gameplay.Enemy.Succubus
{
    public class IntervalStrategy : EnemyTimerStrategyBase
    {
        public IntervalStrategy(float interval): base(interval){}

        protected override Node.Status WaitingStatus => Node.Status.Failure;

        protected override Node.Status CompletionStatus => Node.Status.Success;

        public override Node.Status Process()
        {
            return base.Process();
        }

        public override void Reset()
        {
        }
    }
}