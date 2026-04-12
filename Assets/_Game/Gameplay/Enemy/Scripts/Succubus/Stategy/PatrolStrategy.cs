using Pong.Framework.BehaviourTree;

namespace Pong.Gameplay.Enemy.Succubus
{
    public class PatrolStrategy : EnemyPathStrategyBase
    {
        private readonly SuccubusEnemy _succubusEnemy;

        public int NodesBeforeReset { get; set; } = 2;

        public PatrolStrategy(SuccubusEnemy enemy, EnemyPathFinder pathFinder)
            : base(enemy, pathFinder, () => enemy != null ? enemy.PatrolSpeed : 0f)
        {
            _succubusEnemy = enemy;
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

            if ((_currentTargetNode.transform.position - _succubusEnemy.transform.position).sqrMagnitude < 0.25f)
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
            var currentNode = GetClosestNode(_succubusEnemy.transform.position);
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