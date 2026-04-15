using System;
using System.Collections.Generic;
using Pong.Framework.BehaviourTree;
using Pong.Gameplay.Actors;
using Pong.Systems.Graph;
using UnityEngine;

namespace Pong.Gameplay.Enemy
{
    public abstract class EnemyPathStrategyBase : INodeStrategy
    {
        protected Actor _actor;
        protected readonly EnemyPathFinder _pathFinder;
        private readonly Func<float> _movementSpeedProvider;

        protected List<GraphNode> _currentPath;
        protected int _currentPathIndex;
        protected GraphNode _currentTargetNode;

        protected EnemyPathStrategyBase(Actor actor, EnemyPathFinder pathFinder, Func<float> movementSpeedProvider)
        {
           _actor = actor;
            _pathFinder = pathFinder;
            _movementSpeedProvider = movementSpeedProvider;
            _currentPath = new List<GraphNode>();
            ResetPath();
        }

        protected bool IsReady =>_actor != null && _pathFinder != null;

        protected GraphNode CurrentTargetNode => _currentTargetNode;

        public abstract Node.Status Process();

        protected GraphNode GetClosestNode(Vector3 position)
        {
            return _pathFinder?.GetClosestNode(position);
        }

        protected List<GraphNode> GetAllNodes()
        {
            return _pathFinder?.GetAllNodes();
        }

        protected GraphNode GetRandomNode()
        {
            var graphNodes = GetAllNodes();
            if (graphNodes == null || graphNodes.Count == 0)
            {
                return null;
            }

            int randomIndex = UnityEngine.Random.Range(0, graphNodes.Count);
            return graphNodes[randomIndex];
        }

        protected bool TrySetPath(List<GraphNode> path, int startIndex = 0)
        {
            if (path == null || path.Count == 0)
            {
                ResetPath();
                return false;
            }

            _currentPath = path;
            _currentPathIndex = Mathf.Clamp(startIndex, 0, _currentPath.Count - 1);
            _currentTargetNode = _currentPath[_currentPathIndex];
            return true;
        }

        protected bool TryBuildPath(GraphNode startNode, GraphNode targetNode, bool preferHighWeight = false, int startIndex = 0)
        {
            if (startNode == null || targetNode == null || _pathFinder == null)
            {
                ResetPath();
                return false;
            }

            var path = _pathFinder.FindPath(startNode, targetNode, preferHighWeight);
            return TrySetPath(path, startIndex);
        }

        protected bool HasReachedTarget(GraphNode targetNode)
        {
            if (_actor == null || targetNode == null)
            {
                return false;
            }

            return (targetNode.transform.position -_actor.transform.position).sqrMagnitude < 0.25f;
        }

        protected void MoveTowardNode(GraphNode targetNode)
        {
            if (_actor == null || targetNode == null)
            {
                return;
            }

            float speed = _movementSpeedProvider != null ? Mathf.Max(0f, _movementSpeedProvider()) : 0f;
           _actor.transform.position = Vector3.MoveTowards(
               _actor.transform.position,
                targetNode.transform.position,
                speed * Time.deltaTime
            );
        }

        protected void AdvancePath()
        {
            if (_currentPath == null)
            {
                ResetPath();
                return;
            }

            _currentPathIndex++;
            if (_currentPathIndex >= _currentPath.Count)
            {
                ResetPath();
                return;
            }

            _currentTargetNode = _currentPath[_currentPathIndex];
        }

        protected void ResetPath()
        {
            if (_currentPath == null)
            {
                _currentPath = new List<GraphNode>();
            }
            else
            {
                _currentPath.Clear();
            }

            _currentPathIndex = 0;
            _currentTargetNode = null;
        }

        public virtual void Reset()
        {
            ResetPath();
        }
    }
}