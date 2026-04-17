using Pong.Framework.BehaviourTree;
using Pong.Gameplay.Enemy;
using Pong.Systems.Graph;

namespace Pong.Gameplay.Boss
{
    public class GluttonyPatrolStrategy : EnemyPathStrategyBase
    {
        private readonly GluttonyBoss _boss;

        public GluttonyPatrolStrategy(GluttonyBoss boss, EnemyPathFinder pathFinder)
            : base(boss, pathFinder, () => boss != null ? boss.PatrolSpeed : 0f)
        {
            _boss = boss;
        }

        public override Node.Status Process()
        {
            if (!IsReady || !_boss.CanMove || _boss.IsAttacking)
                return Node.Status.Failure;

            if (_currentPath.Count == 0 || _currentTargetNode == null)
            {
                SelectNewDestination();

                if (_currentPath.Count == 0)
                    return Node.Status.Failure;
            }

            _boss.RotateTowardsPosition(_currentTargetNode.transform.position);
            MoveTowardNode(_currentTargetNode);

            if (HasReachedTarget(_currentTargetNode))
            {
                AdvancePath();
            }

            return Node.Status.Running;
        }

        private void SelectNewDestination()
        {
            var currentNode = GetClosestNode(_boss.transform.position);
            var targetNode = GetRandomNode();

            if (currentNode == null || targetNode == null)
            {
                ResetPath();
                return;
            }

            TryBuildPath(currentNode, targetNode);
        }
    }
}