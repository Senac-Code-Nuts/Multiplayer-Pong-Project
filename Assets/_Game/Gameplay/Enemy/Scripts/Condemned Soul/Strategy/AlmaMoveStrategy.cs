using System.Collections.Generic;
using UnityEngine;
using Pong.Framework.BehaviourTree;
using Pong.Systems.Graph;

namespace Pong.Gameplay.Enemy
{
    public class AlmaMoveStrategy : INodeStrategy
    {
        private readonly CondemnedSoulEnemy _enemy;
        private readonly EnemyPathFinder _pathFinder;

        private List<GraphNode> _currentPath;
        private int _currentPathIndex;
        private GraphNode _currentTargetNode;
        private float _decisionTimer;
        private bool _isWaitingForDecision;

        public AlmaMoveStrategy(CondemnedSoulEnemy enemy, EnemyPathFinder pathFinder)
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

            if (_isWaitingForDecision)
            {
                _decisionTimer += Time.deltaTime;

                if (_decisionTimer < _enemy.PathDecisionDelay)
                {
                    return Node.Status.Running;
                }

                ResetPathState();
                return Node.Status.Success;
            }

            if (_currentPath.Count == 0)
            {
                if (!TryBuildPath())
                {
                    return Node.Status.Failure;
                }
            }

            if (_currentPathIndex >= _currentPath.Count)
            {
                BeginDecisionWait();
                return Node.Status.Running;
            }

            _currentTargetNode = _currentPath[_currentPathIndex];
            MoveTowardNode(_currentTargetNode);

            if (HasReachedTarget(_currentTargetNode))
            {
                _currentPathIndex++;

                if (_currentPathIndex >= _currentPath.Count)
                {
                    BeginDecisionWait();
                }
            }

            return Node.Status.Running;
        }

        private bool TryBuildPath()
        {
            var startNode = _pathFinder.GetClosestNode(_enemy.transform.position);

            if (startNode == null)
            {
                return false;
            }

            var targetNode = SelectTargetNode(startNode);
            if (targetNode == null)
            {
                return false;
            }

            var path = _pathFinder.FindPath(startNode, targetNode);
            if (path == null || path.Count == 0)
            {
                return false;
            }

            _currentPath = path;
            _currentPathIndex = _currentPath.Count > 1 ? 1 : 0;
            _currentTargetNode = _currentPath[_currentPathIndex];

            if (_currentPathIndex >= _currentPath.Count)
            {
                BeginDecisionWait();
            }

            return true;
        }

        private GraphNode SelectTargetNode(GraphNode startNode)
        {
            List<GraphNode> nodes = _pathFinder.GetAllNodes();

            if (nodes == null || nodes.Count == 0)
            {
                return null;
            }

            List<GraphNode> candidates = new List<GraphNode>();

            foreach (var node in nodes)
            {
                if (node != null && node != startNode)
                {
                    candidates.Add(node);
                }
            }

            if (candidates.Count == 0)
            {
                return startNode;
            }

            int randomIndex = Random.Range(0, candidates.Count);
            return candidates[randomIndex];
        }

        private void MoveTowardNode(GraphNode targetNode)
        {
            Vector3 currentPosition = _enemy.transform.position;
            Vector3 targetPosition = targetNode.transform.position;
            float step = _enemy.MovementSpeed * Time.deltaTime;

            _enemy.transform.position = Vector3.MoveTowards(currentPosition, targetPosition, step);
        }

        private bool HasReachedTarget(GraphNode targetNode)
        {
            return (targetNode.transform.position - _enemy.transform.position).sqrMagnitude < 0.25f;
        }

        private void BeginDecisionWait()
        {
            _isWaitingForDecision = true;
            _decisionTimer = 0f;
        }

        private void ResetPathState()
        {
            _currentPath.Clear();
            _currentPathIndex = 0;
            _currentTargetNode = null;
            _isWaitingForDecision = false;
            _decisionTimer = 0f;
        }

        public void Reset()
        {
            ResetPathState();
        }
    }
}