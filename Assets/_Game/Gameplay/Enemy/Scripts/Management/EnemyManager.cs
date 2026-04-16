using System.Collections.Generic;
using UnityEngine;
using Pong.Gameplay.Enemy;
using Pong.Gameplay.Player;
using Pong.Systems.Graph;

namespace Pong.Gameplay.Enemy
{
    public class EnemyManager : MonoBehaviour
    {
        [Header("Spawn Configuration")]
        [SerializeField] private List<EnemyActor> _enemyPrefabs;
        [SerializeField] private List<Transform> _spawnPoints;
        private List<EnemyActor> _activeEnemies = new List<EnemyActor>();

        private const string TAG = "<color=red><b>[EnemyManager]</b></color>";

        public void SpawnEnemies()
        {
            _activeEnemies.Clear();

            int spawnCount = Mathf.Min(_enemyPrefabs.Count, _spawnPoints.Count);
            for (int i = 0; i < spawnCount; i++)
            {
                EnemyActor spawnedEnemy = Instantiate(_enemyPrefabs[i], _spawnPoints[i].position, _spawnPoints[i].rotation, _spawnPoints[i]);
                _activeEnemies.Add(spawnedEnemy);
            }
            
            Debug.Log($"{TAG} {_activeEnemies.Count} inimigos preparados para a batalha.");
        }

        public void InjectTargetsAndStartAI(List<PlayerController> activePlayers, InfluenceSystem influenceSystem)
        {
            foreach (var enemy in _activeEnemies)
            {
                if (enemy != null)
                {
                    enemy.InitializeAI(activePlayers, influenceSystem);
                }
            }
        }
    }
}