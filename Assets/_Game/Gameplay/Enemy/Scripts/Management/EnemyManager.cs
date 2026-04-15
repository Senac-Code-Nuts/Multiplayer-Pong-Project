using System.Collections.Generic;
using UnityEngine;
using Pong.Gameplay.Enemy;
using Pong.Gameplay.Player;

namespace Pong.Gameplay.Enemy
{
    public class EnemyManager : MonoBehaviour
    {
        [Header("Spawn Configuration")]
        [SerializeField] private List<EnemyActor> _enemyPrefabs;
        [SerializeField] private List<Transform> _spawnPoints;
        private List<EnemyActor> _activeEnemies = new List<EnemyActor>();

        // O Bootstrapper chama isso na Fase 1 (Setup)
        public void SpawnEnemies()
        {
            _activeEnemies.Clear();

            int spawnCount = Mathf.Min(_enemyPrefabs.Count, _spawnPoints.Count);
            for (int i = 0; i < spawnCount; i++)
            {
                EnemyActor spawnedEnemy = Instantiate(_enemyPrefabs[i], _spawnPoints[i].position, _spawnPoints[i].rotation);
                _activeEnemies.Add(spawnedEnemy);
            }
            
            Debug.Log($"<color=red><b>[EnemyManager]</b> {_activeEnemies.Count} inimigos preparados para a batalha.</color>");
        }

        public void InjectTargetsAndStartAI(List<PlayerController> activePlayers)
        {
            foreach (var enemy in _activeEnemies)
            {
                if (enemy != null)
                {
                    enemy.InitializeAI(activePlayers);
                }
            }
        }
    }
}