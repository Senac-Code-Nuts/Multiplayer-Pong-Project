using System.Collections.Generic;
using UnityEngine;
using Pong.Framework.BehaviourTree;
using Pong.Systems.Graph;

namespace Pong.Gameplay.Enemy
{
    public class MinotaurMoveStrategy : INodeStrategy
    {
        private readonly MinotaurEnemy _enemy;
        private readonly EnemyPathFinder _pathFinder;

        private List<GraphNode> _currentPath;
        private int _currentPathIndex;
        private GraphNode _currentTargetNode;
        private float _repathTimer;

        public MinotaurMoveStrategy(MinotaurEnemy enemy, EnemyPathFinder pathFinder)
        {
            _enemy = enemy;
            _pathFinder = pathFinder;
            _currentPath = new List<GraphNode>();
            Reset();
        }

        public Node.Status Process()
        {
            if (_enemy == null || _pathFinder == null)
            {
                return Node.Status.Failure;
            }

            if (_enemy.TargetRelic == null)
            {
                return Node.Status.Failure;
            }

            TickMovement();

            return Node.Status.Running;
        }

        public void TickMovement()
        {
            if (_enemy == null || _pathFinder == null || _enemy.TargetRelic == null)
            {
                return;
            }

            _repathTimer += Time.deltaTime;

            if (_currentPath.Count == 0 || _currentPathIndex >= _currentPath.Count || _repathTimer >= _enemy.PathDecisionDelay)
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
            var currentNode = _pathFinder.GetClosestNode(_enemy.transform.position);
            var targetNode = GetRelicTargetNode();

            if (currentNode == null || targetNode == null)
            {
                return false;
            }

            // Parry Mode: define comportamento de movimento
            if (_enemy.IsInParryMode)
            {
                // EM PARRY MODE: Procura a relic em áreas de ALTO peso (onde o player está)
                var highWeightTarget = _pathFinder.GetClosestHighWeightNode(_enemy.TargetRelic.transform.position);
                targetNode = highWeightTarget ?? targetNode;
                
                // Usa preferHighWeight: true para priorizar caminhos pesados
                var path = _pathFinder.FindPath(currentNode, targetNode, preferHighWeight: true);
                if (path == null || path.Count == 0)
                {
                    return false;
                }

                _currentPath = path;
                _currentPathIndex = _currentPath.Count > 1 ? 1 : 0;
                return true;
            }
            else
            {
                // FORA DE PARRY MODE: Evita a relic, procura em áreas de BAIXO peso (longe do player)
                var lowWeightTarget = _pathFinder.GetClosestLowWeightNode(_enemy.TargetRelic.transform.position);
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

                _currentPath = path;
                _currentPathIndex = _currentPath.Count > 1 ? 1 : 0;
                return true;
            }
        }

        private GraphNode GetRelicTargetNode()
        {
            if (_enemy.TargetRelic == null)
            {
                return null;
            }

            List<GraphNode> nodes = _pathFinder.GetAllNodes();

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

                float distance = Vector3.Distance(node.transform.position, _enemy.TargetRelic.transform.position);
                if (distance < closestDistance)
                {
                    closestDistance = distance;
                    closestNode = node;
                }
            }

            return closestNode;
        }

        private void MoveTowardNode(GraphNode targetNode)
        {
            Vector3 currentPosition = _enemy.transform.position;
            Vector3 targetPosition = targetNode.transform.position;
            float step = _enemy.CurrentMovementSpeed * Time.deltaTime;

            _enemy.transform.position = Vector3.MoveTowards(currentPosition, targetPosition, step);
        }

        private bool HasReachedTarget(GraphNode targetNode)
        {
            return (targetNode.transform.position - _enemy.transform.position).sqrMagnitude < 0.25f;
        }

        private void ResetPathState()
        {
            _currentPath.Clear();
            _currentPathIndex = 0;
            _currentTargetNode = null;
            _repathTimer = 0f;
        }

        public void Reset()
        {
            ResetPathState();
        }
    }
}