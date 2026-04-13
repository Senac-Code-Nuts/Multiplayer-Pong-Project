using System.Collections.Generic;
using UnityEngine;
using Pong.Framework.BehaviourTree;
using Pong.Systems.Graph;

namespace Pong.Gameplay.Enemy
{
    public class MinotaurMoveStrategy : EnemyPathStrategyBase
    {
        private readonly MinotaurEnemy _minotaurEnemy;
        private float _repathTimer;

        public MinotaurMoveStrategy(MinotaurEnemy enemy, EnemyPathFinder pathFinder)
            : base(enemy, pathFinder, () => enemy != null ? enemy.CurrentMovementSpeed : 0f)
        {
            _minotaurEnemy = enemy;
            Reset();
        }

        public override Node.Status Process()
        {
            if (_minotaurEnemy == null || _pathFinder == null)
            {
                return Node.Status.Failure;
            }

            if (_minotaurEnemy.TargetRelic == null)
            {
                return Node.Status.Failure;
            }

            TickMovement();

            return Node.Status.Running;
        }

        public void TickMovement()
        {
            if (_minotaurEnemy == null || _pathFinder == null || _minotaurEnemy.TargetRelic == null)
            {
                return;
            }

            _repathTimer += Time.deltaTime;

            if (_currentPath.Count == 0 || _currentPathIndex >= _currentPath.Count || _repathTimer >= _minotaurEnemy.PathDecisionDelay)
            {
                if (!TryBuildPath())
                {
                    return;
                }

                _repathTimer = 0f;
            }

            if (_currentPath.Count == 0 || _currentPathIndex >= _currentPath.Count)
            {
                return;
            }

            _currentTargetNode = _currentPath[_currentPathIndex];
            MoveTowardNode(_currentTargetNode);

            if (HasReachedTarget(_currentTargetNode))
            {
                _currentPathIndex++;

                if (_currentPathIndex >= _currentPath.Count)
                {
                    _currentPath.Clear();
                    _currentPathIndex = 0;
                }
            }
        }

        private bool TryBuildPath()
        {
            var currentNode = GetClosestNode(_minotaurEnemy.transform.position);
            var targetNode = GetRelicTargetNode();

            if (currentNode == null || targetNode == null)
            {
                return false;
            }

            // Parry Mode: define comportamento de movimento
            if (_minotaurEnemy.IsInParryMode)
            {
                // EM PARRY MODE: Procura a relic em áreas de ALTO peso (onde o player está)
                var highWeightTarget = _pathFinder.GetClosestHighWeightNode(_minotaurEnemy.TargetRelic.transform.position);
                targetNode = highWeightTarget ?? targetNode;
                
                // Usa preferHighWeight: true para priorizar caminhos pesados
                var path = _pathFinder.FindPath(currentNode, targetNode, preferHighWeight: true);
                if (path == null || path.Count == 0)
                {
                    return false;
                }

                if (!TrySetPath(path, path.Count > 1 ? 1 : 0))
                {
                    return false;
                }
                return true;
            }
            else
            {
                // FORA DE PARRY MODE: Evita a relic, procura em áreas de BAIXO peso (longe do player)
                var lowWeightTarget = _pathFinder.GetClosestLowWeightNode(_minotaurEnemy.TargetRelic.transform.position);
                if (lowWeightTarget != null)
                {
                    targetNode = lowWeightTarget;
                }
                
                // Usa preferHighWeight: false para evitar pesos altos
                var path = _pathFinder.FindPath(currentNode, targetNode, preferHighWeight: false);
                if (path == null || path.Count == 0)
                {
                    return false;
                }

                if (!TrySetPath(path, path.Count > 1 ? 1 : 0))
                {
                    return false;
                }
                return true;
            }
        }

        private GraphNode GetRelicTargetNode()
        {
            if (_minotaurEnemy.TargetRelic == null)
            {
                return null;
            }

            List<GraphNode> nodes = GetAllNodes();

            if (nodes == null || nodes.Count == 0)
            {
                return null;
            }

            GraphNode closestNode = null;
            float closestDistance = float.MaxValue;

            foreach (var node in nodes)
            {
                if (node == null)
                {
                    continue;
                }

                float distance = Vector3.Distance(node.transform.position, _minotaurEnemy.TargetRelic.transform.position);
                if (distance < closestDistance)
                {
                    closestDistance = distance;
                    closestNode = node;
                }
            }

            return closestNode;
        }

        private void ResetPathState()
        {
            ResetPath();
            _repathTimer = 0f;
        }

        public override void Reset()
        {
            ResetPathState();
        }
    }
}