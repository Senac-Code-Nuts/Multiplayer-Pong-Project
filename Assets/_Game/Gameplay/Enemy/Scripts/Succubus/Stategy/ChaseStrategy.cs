using Pong.Framework.Strategy;
using Pong.Framework.BehaviourTree;
using System.Collections.Generic;
using UnityEngine;
using Pong.Systems.Graph;
using Pong.Gameplay.Player;

namespace Pong.Gameplay.Enemy.Succubus
{
    public class ChaseStrategy : INodeStrategy
    {
        private readonly SuccubusEnemy _enemy;
        private readonly EnemyPathFinder _pathFinder;
        private Transform _target;

        private List<GraphNode> _currentPath;
        private int _currentPathIndex;
        private GraphNode _currentTargetNode;

        private PlayerController[] _cachedPlayers; // Cache dos players

        public ChaseStrategy(SuccubusEnemy enemy, EnemyPathFinder pathFinder)
        {
            _enemy = enemy;
            _pathFinder = pathFinder;
            _currentPath = new List<GraphNode>();
            _currentPathIndex = 0;
            _cachedPlayers = null; 
        }

        public void SetTarget()
        {
            CachePlayersIfNeeded();
            _target = FindFarthestPlayer()?.transform;
        }

        private void CachePlayersIfNeeded()
        {
            if (_cachedPlayers != null) return;

            GameObject[] playerObjects = GameObject.FindGameObjectsWithTag("Player");
            
            _cachedPlayers = new PlayerController[playerObjects.Length];
            for (int i = 0; i < playerObjects.Length; i++)
            {
                _cachedPlayers[i] = playerObjects[i].GetComponent<PlayerController>();
            }
        }

        private PlayerController FindFarthestPlayer()
        {
            if (_cachedPlayers.Length == 0)
            {
                Debug.LogWarning("[Chase] Nenhum Player encontrado com a tag 'Player'!");
                return null;
            }

            PlayerController farthestPlayer = null;
            float maxDistance = float.MinValue;

            foreach (var player in _cachedPlayers)
            {
                if (player != null)
                {
                    float distance = Vector3.Distance(_enemy.transform.position, player.transform.position);
                    if (distance > maxDistance)
                    {
                        maxDistance = distance;
                        farthestPlayer = player;
                    }
                }
            }

            if (farthestPlayer != null)
                Debug.Log($"[Chase] Target setado: {farthestPlayer.name} (farthest)");

            return farthestPlayer;
        }

        public Node.Status Process()
        {
            if (_target == null)
            {
                SetTarget();
            }

            if (_target == null)
            {
                return Node.Status.Failure;
            }

            var currentNode = _pathFinder.GetClosestNode(_enemy.transform.position);
            var targetNode = _pathFinder.GetClosestNode(_target.position);

            if (currentNode == null || targetNode == null)
            {
                Debug.LogWarning("[Chase] Nó não encontrado!");
                return Node.Status.Failure;
            }

            if (_currentPath.Count == 0 || _currentPathIndex >= _currentPath.Count)
            {
                _currentPath = _pathFinder.FindPath(currentNode, targetNode);
                _currentPathIndex = 0;

                if (_currentPath.Count == 0)
                {
                    Debug.LogWarning("[Chase] Caminho vazio!");
                    return Node.Status.Failure;
                }

                Debug.Log($"[Chase] Novo caminho calculado: {_currentPath.Count} nós");
            }

            _currentTargetNode = _currentPath[_currentPathIndex];

            if ((_currentTargetNode.transform.position - _enemy.transform.position).sqrMagnitude < 0.25f)
            {
                _currentPathIndex++;

                if (_currentPathIndex >= _currentPath.Count)
                {
                    Debug.Log("[Chase] ✓ Chegou no alvo!");
                    return Node.Status.Success; 
                }
            }

            MoveTowardNode(_currentTargetNode);

            return Node.Status.Running;
        }

        private void MoveTowardNode(GraphNode targetNode)
        {
            Vector3 targetPos = targetNode.transform.position;
            Vector3 currentPos = _enemy.transform.position;

            float step = _enemy.ChaseSpeed * Time.deltaTime;
            _enemy.transform.position = Vector3.MoveTowards(currentPos, targetPos, step);
        }

        public void Reset()
        {
            _currentPath.Clear();
            _currentPathIndex = 0;
            _currentTargetNode = null;
            _target = null;
        }
    }
}
