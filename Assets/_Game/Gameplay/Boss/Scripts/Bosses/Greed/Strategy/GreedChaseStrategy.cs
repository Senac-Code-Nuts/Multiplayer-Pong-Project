using Pong.Gameplay.Enemy;
using UnityEngine;
using Pong.Framework.BehaviourTree;
using Pong.Gameplay.Player;
using Pong.Systems.Input;
using Pong.Gameplay.Boss.Greed;

namespace Pong.Gameplay
{
    public class GreedChaseStrategy : EnemyPathStrategyBase
    {
        private readonly GreedBoss _greedBoss;
        private Transform _target;
        private PlayerController[] _cachedPlayers;
        private const float AttackRangeThreshold = 2.5f;

        public GreedChaseStrategy(GreedBoss boss, EnemyPathFinder pathFinder) : base(boss, pathFinder, () => boss != null ? boss.ChaseSpeed : 0f)
        {
            _greedBoss = boss;
        }

        public void SetTarget()
        {
            _cachedPlayers = null; 
            CachePlayersIfNeeded();
            _target = FindFarthestPlayer()?.transform;

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
            var manager = GamepadsManager.Instance;
            if (manager == null) return;
            GameObject[] gameObjects = manager.GetActivePlayerControllers();
            if (gameObjects == null) return;
            var validPlayers = new System.Collections.Generic.List<PlayerController>();
            foreach (var go in gameObjects)
            {
                if (go != null)
                {
                    var pc = go.GetComponent<PlayerController>();
                    if (pc != null) validPlayers.Add(pc);
                }
            }
            _cachedPlayers = validPlayers.ToArray();
        }

        private PlayerController FindFarthestPlayer()
        {
            if (_cachedPlayers == null || _cachedPlayers.Length == 0) return null;
            PlayerController farthestPlayer = null;
            float maxDistance = float.MinValue;
            foreach (var player in _cachedPlayers)
            {
                if (player == null) continue;
                float distance = Vector3.Distance(_greedBoss.transform.position, player.transform.position);
                if (distance > maxDistance)
                {
                    maxDistance = distance;
                    farthestPlayer = player;
                }
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
        }
    }
}