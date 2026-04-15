using System.Collections.Generic;
using Pong.Framework.BehaviourTree;
using Pong.Gameplay.Player;
using UnityEngine;

namespace Pong.Gameplay.Enemy.Succubus
{
    public class ChaseStrategy : EnemyPathStrategyBase
    {
        private readonly SuccubusEnemy _succubusEnemy;
        private Transform _target;
        private List<PlayerController> _activePlayers;
        private List<Transform> _cachedPlayers;

        public ChaseStrategy(SuccubusEnemy enemy, EnemyPathFinder pathFinder)
            : base(enemy, pathFinder, () => enemy != null ? enemy.ChaseSpeed : 0f)
        {
            _succubusEnemy = enemy;
            _activePlayers = new List<PlayerController>();
            _cachedPlayers = null;
        }

        public void SetActivePlayers(List<PlayerController> activePlayers)
        {
            _activePlayers = activePlayers ?? new List<PlayerController>();
            _cachedPlayers = null;
            _target = null;
        }

        public void SetTarget()
        {
            CachePlayersIfNeeded();
            _target = FindFarthestPlayer();
        }

        private void CachePlayersIfNeeded()
        {
            if (_cachedPlayers != null) return;

            _cachedPlayers = new List<Transform>();

            for (int i = 0; i < _activePlayers.Count; i++)
            {
                PlayerController player = _activePlayers[i];

                if (player != null)
                {
                    _cachedPlayers.Add(player.transform);
                }
            }
        }

        private Transform FindFarthestPlayer()
        {
            if (_cachedPlayers == null || _cachedPlayers.Count == 0)
            {
                Debug.LogWarning("[Chase] Nenhum player ativo encontrado para perseguir.");
                return null;
            }

            Transform farthestPlayer = null;
            float maxDistance = float.MinValue;

            foreach (var player in _cachedPlayers)
            {
                if (player != null)
                {
                    float distance = Vector3.Distance(_succubusEnemy.transform.position, player.position);
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

        public override Node.Status Process()
        {
            if (!IsReady)
            {
                return Node.Status.Failure;
            }

            if (_target == null)
            {
                SetTarget();
            }

            if (_target == null)
            {
                return Node.Status.Failure;
            }

            var currentNode = GetClosestNode(_succubusEnemy.transform.position);
            var targetNode = GetClosestNode(_target.position);

            if (currentNode == null || targetNode == null)
            {
                Debug.LogWarning("[Chase] Nó não encontrado!");
                return Node.Status.Failure;
            }

            if (_currentPath.Count == 0 || _currentPathIndex >= _currentPath.Count)
            {
                if (!TryBuildPath(currentNode, targetNode))
                {
                    Debug.LogWarning("[Chase] Caminho vazio!");
                    return Node.Status.Failure;
                }

                Debug.Log($"[Chase] Novo caminho calculado: {_currentPath.Count} nós");
            }

            _currentTargetNode = _currentPath[_currentPathIndex];

            if ((_currentTargetNode.transform.position - _succubusEnemy.transform.position).sqrMagnitude < 0.25f)
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

        public override void Reset()
        {
            base.Reset();
            _target = null;
            _cachedPlayers = null;
        }
    }
}
