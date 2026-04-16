using Pong.Gameplay.Enemy;
using UnityEngine;
using Pong.Framework.BehaviourTree;
using Pong.Gameplay.Player;
using System.Collections.Generic;

namespace Pong.Gameplay.Boss.Greed
{
    public class GreedChaseStrategy : EnemyPathStrategyBase
    {
        private readonly GreedBoss _greedBoss;
        private Transform _target;
        private List<PlayerController> _activePlayers;
        private List<Transform> _cachedPlayers;
        private const float AttackRangeThreshold = 2.5f;

        public GreedChaseStrategy(GreedBoss boss, EnemyPathFinder pathFinder) : base(boss, pathFinder, () => boss != null ? boss.ChaseSpeed : 0f)
        {
            _greedBoss = boss;
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

            if (_target != null)
            {
                Debug.Log($"<color=green>[Chase] Alvo encontrado e definido: {_target.name}</color>");
            }
            else
            {
                Debug.LogWarning("[Chase] Tentativa de SetTarget falhou: Nenhum player ativo.");
            }
        }

        private void CachePlayersIfNeeded()
        {
            if (_cachedPlayers != null)
            {
                return;
            }

            _cachedPlayers = new List<Transform>();

            foreach (var player in _activePlayers)
            {
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
                if (player == null) continue;
                float distance = Vector3.Distance(_greedBoss.transform.position, player.position);
                if (distance > maxDistance)
                {
                    maxDistance = distance;
                    farthestPlayer = player;
                }
            }

            if (farthestPlayer != null)
            {
                Debug.Log($"[Chase] Target setado: {farthestPlayer.name} (farthest)");
            }

            return farthestPlayer;
        }

        public override Node.Status Process()
        {
            if (!IsReady) return Node.Status.Failure;
            if (_target == null) SetTarget();
            if (_target == null) return Node.Status.Failure;
            float currentDistance = Vector3.Distance(_greedBoss.transform.position, _target.position);
            if (currentDistance <= AttackRangeThreshold)
            {
                Debug.Log($"<color=yellow>[Chase] Proximo do {_target.name}</color>");
                ResetPath();
                return Node.Status.Success;
            }

            if (_currentPath.Count == 0 || _currentPathIndex >= _currentPath.Count)
            {
                var currentNode = GetClosestNode(_greedBoss.transform.position);
                var targetNode = GetClosestNode(_target.position);
                if (currentNode != null && targetNode != null)
                {
                    if (TryBuildPath(currentNode, targetNode))
                    {
                        Debug.Log($"[Chase] Novo caminho para {_target.name} com {_currentPath.Count} nós.");
                    }
                }
            }

            if (_currentPath.Count > 0)
            {
                _currentTargetNode = _currentPath[_currentPathIndex];
                MoveTowardNode(_currentTargetNode);
                if (Vector3.Distance(_greedBoss.transform.position, _currentTargetNode.transform.position) < 0.6f)
                {
                    _currentPathIndex++;
                }
                return Node.Status.Running;
            }
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