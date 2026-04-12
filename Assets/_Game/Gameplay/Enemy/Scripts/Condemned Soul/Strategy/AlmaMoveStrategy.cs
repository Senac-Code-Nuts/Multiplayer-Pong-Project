using System.Collections.Generic;
using UnityEngine;
using Pong.Framework.BehaviourTree;
using Pong.Systems.Graph;

namespace Pong.Gameplay.Enemy
{
    public class AlmaMoveStrategy : EnemyPathStrategyBase
    {
        private readonly CondemnedSoulEnemy _condemnedSoul;
        private float _decisionTimer;
        private bool _isWaitingForDecision;

        public AlmaMoveStrategy(CondemnedSoulEnemy enemy, EnemyPathFinder pathFinder)
            : base(enemy, pathFinder, () => enemy != null ? enemy.MovementSpeed : 0f)
        {
            _condemnedSoul = enemy;
            Reset();
        }

        public override Node.Status Process()
        {
            if (_condemnedSoul == null || _pathFinder == null)
            {
                return Node.Status.Failure;
            }

            if (_isWaitingForDecision)
            {
                _decisionTimer += Time.deltaTime;

                if (_decisionTimer < _condemnedSoul.PathDecisionDelay)
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
            var startNode = GetClosestNode(_condemnedSoul.transform.position);

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

            if (!TrySetPath(path, path.Count > 1 ? 1 : 0))
            {
                return false;
            }

            if (_currentPathIndex >= _currentPath.Count)
            {
                BeginDecisionWait();
            }

            return true;
        }

        private GraphNode SelectTargetNode(GraphNode startNode)
        {
            List<GraphNode> nodes = GetAllNodes();

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

        private void BeginDecisionWait()
        {
            _isWaitingForDecision = true;
            _decisionTimer = 0f;
        }

        private void ResetPathState()
        {
            ResetPath();
            _isWaitingForDecision = false;
            _decisionTimer = 0f;
        }

        public override void Reset()
        {
            ResetPathState();
        }
    }
}