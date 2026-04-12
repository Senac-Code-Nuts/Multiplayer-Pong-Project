using Pong.Framework.BehaviourTree;
using System.Collections.Generic;
using UnityEngine;
using Pong.Systems.Graph;

namespace Pong.Gameplay.Enemy.Cerberus
{
    public class CerberusPatrolStrategy : INodeStrategy
    {
        private readonly CerberusEnemy _enemy;
        private readonly EnemyPathFinder _pathFinder;

        private List<GraphNode> _currentPath;
        private int _currentPathIndex;
        private GraphNode _currentTargetNode;
        private GraphNode _patrolNode;
        
        public int NodesBeforeReset { get; set; } = 2;

        public CerberusPatrolStrategy(CerberusEnemy enemy, EnemyPathFinder pathFinder)
        {
            _enemy = enemy;
            _pathFinder = pathFinder;
            _currentPath = new List<GraphNode>();
            _currentPathIndex = 0;
        }

        public Node.Status Process()
        {
            if (_currentPath.Count == 0 || _currentPathIndex >= _currentPath.Count)
            {
                SelectNewPatrolPoint();
                if (_currentPath.Count == 0)
                {
                    return Node.Status.Failure;
                }
            }

            _currentTargetNode = _currentPath[_currentPathIndex];

            if ((_currentTargetNode.transform.position - _enemy.transform.position).sqrMagnitude < 0.25f)
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
            var currentNode = _pathFinder.GetClosestNode(_enemy.transform.position);
            if (currentNode == null)
            {
                _currentPath.Clear();
                return;
            }

            _patrolNode = GetRandomPatrolNode();
            if (_patrolNode == null)
            {
                _currentPath.Clear();
                return;
            }

            _currentPath = _pathFinder.FindPath(currentNode, _patrolNode);
            _currentPathIndex = 0;
        }

        private GraphNode GetRandomPatrolNode()
        {
            var graphNodes = _pathFinder.GetAllNodes();
            if (graphNodes == null || graphNodes.Count == 0)
                return null;

            int randomIndex = Random.Range(0, graphNodes.Count);
            return graphNodes[randomIndex];
        }

        private void MoveTowardNode(GraphNode targetNode)
        {
            Vector3 targetPos = targetNode.transform.position;
            Vector3 currentPos = _enemy.transform.position;

            float step = _enemy.PatrolSpeed * Time.deltaTime;
            _enemy.transform.position = Vector3.MoveTowards(currentPos, targetPos, step);
        }

        public void Reset()
        {
            _currentPath.Clear();
            _currentPathIndex = 0;
            _currentTargetNode = null;
            _patrolNode = null;
        }
    }
}