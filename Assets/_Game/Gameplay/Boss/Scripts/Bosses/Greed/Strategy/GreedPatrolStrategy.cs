using Pong.Gameplay.Boss.Greed;
using Pong.Framework.BehaviourTree;
using Pong.Gameplay.Enemy;

namespace Pong.Gameplay.Boss.Greed
{
    public class GreedPatrolStrategy : EnemyPathStrategyBase
    {
        private readonly GreedBoss _greedBoss;
        public int NodesBeforeReset { get; set; } = 2;

        public GreedPatrolStrategy(GreedBoss boss, EnemyPathFinder pathFinder): base(boss,pathFinder, () => boss != null ? boss.PatrolSpeed : 0f)
        {
            _greedBoss = boss;
        }

        public override Node.Status Process()
        {
            if (!IsReady)
            {
                return Node.Status.Failure;
            }

            if (_currentPath.Count == 0 || _currentPathIndex >= _currentPath.Count)
            {
                SelectNewPatrolPoint();
                if (_currentPath.Count == 0)
                {
                    return Node.Status.Failure;
                }
            }

            _currentTargetNode = _currentPath[_currentPathIndex];

            if ((_currentTargetNode.transform.position - _greedBoss.transform.position).sqrMagnitude < 0.25f)
            {
                _currentPathIndex++;

                if (_currentPathIndex >= NodesBeforeReset || _currentPathIndex >= _currentPath.Count)
                {
                    _currentPath.Clear();
                }

                return Node.Status.Running;
            }

            MoveTowardNode(_currentTargetNode);

            return Node.Status.Running;
        }

        private void SelectNewPatrolPoint()
        {
            var currentNode = GetClosestNode(_greedBoss.transform.position);
            if (currentNode == null)
            {
                ResetPath();
                return;
            }

            var patrolNode = GetRandomNode();
            if (patrolNode == null)
            {
                ResetPath();
                return;
            }

            TrySetPath(_pathFinder.FindPath(currentNode, patrolNode), 0);
        }
    }
}
