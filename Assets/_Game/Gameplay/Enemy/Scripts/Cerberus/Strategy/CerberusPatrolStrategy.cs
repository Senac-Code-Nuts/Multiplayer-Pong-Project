using Pong.Framework.BehaviourTree;
using UnityEngine;
using Pong.Systems.Graph;
using Pong.Gameplay.Enemy;

namespace Pong.Gameplay.Enemy.Cerberus
{
    public class CerberusPatrolStrategy : EnemyPathStrategyBase
    {
        private readonly CerberusEnemy _cerberusEnemy;

        public int NodesBeforeReset { get; set; } = 2;

        public CerberusPatrolStrategy(CerberusEnemy enemy, EnemyPathFinder pathFinder)
            : base(enemy, pathFinder, () => enemy != null ? enemy.PatrolSpeed : 0f)
        {
            _cerberusEnemy = enemy;
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

            if ((_currentTargetNode.transform.position - _cerberusEnemy.transform.position).sqrMagnitude < 0.25f)
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
            var currentNode = GetClosestNode(_cerberusEnemy.transform.position);
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