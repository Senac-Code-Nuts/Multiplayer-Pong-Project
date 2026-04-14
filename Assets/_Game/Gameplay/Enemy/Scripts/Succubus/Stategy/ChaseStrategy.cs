using UnityEngine;
using Pong.Gameplay.Player;
using Pong.Framework.BehaviourTree;
using Pong.Systems.Input;

namespace Pong.Gameplay.Enemy.Succubus
{
    public class ChaseStrategy : EnemyPathStrategyBase
    {
        private readonly SuccubusEnemy _succubusEnemy;
        private Transform _target;

        private PlayerController[] _cachedPlayers; // Cache dos players

        public ChaseStrategy(SuccubusEnemy enemy, EnemyPathFinder pathFinder)
            : base(enemy, pathFinder, () => enemy != null ? enemy.ChaseSpeed : 0f)
        {
            _succubusEnemy = enemy;
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
                    float distance = Vector3.Distance(_succubusEnemy.transform.position, player.transform.position);
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
        }
    }
}
