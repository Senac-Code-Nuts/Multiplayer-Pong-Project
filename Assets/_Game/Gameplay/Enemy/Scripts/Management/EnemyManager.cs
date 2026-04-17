using System.Collections.Generic;
using UnityEngine;
using Pong.Gameplay.Enemy;
using Pong.Gameplay.Boss;
using Pong.Gameplay.Player;
using Pong.Gameplay.Relics;
using Pong.Systems.Graph;
using Sirenix.OdinInspector;

namespace Pong.Gameplay.Enemy
{
    public class EnemyManager : MonoBehaviour
    {
        [Header("Enemy Spawn Configuration")]
        [SerializeField] private List<EnemyActor> _enemyPrefabs = new List<EnemyActor>();
        [SerializeField] private List<Transform> _spawnPoints = new List<Transform>();

        [FoldoutGroup("Boss Spawn Configuration")]
        [ToggleLeft]
        [LabelText("Enable Bosses")]
        [SerializeField] private bool _useBosses;

        [FoldoutGroup("Boss Spawn Configuration")]
        [ShowIf(nameof(_useBosses))]
        [SerializeField] private List<BossActor> _bossPrefabs = new List<BossActor>();

        [FoldoutGroup("Boss Spawn Configuration")]
        [ShowIf(nameof(_useBosses))]
        [SerializeField] private List<Transform> _bossSpawnPoints = new List<Transform>();

        private List<EnemyActor> _activeEnemies = new List<EnemyActor>();
        private List<BossActor> _activeBosses = new List<BossActor>();

        [SerializeField] private Transform _enemyParent;

        private const string TAG = "<color=red><b>[EnemyManager]</b></color>";

        public void SpawnEnemies(InfluenceSystem influenceSystem, Relic relic = null)
        {
            _activeEnemies.Clear();

            int spawnCount = Mathf.Min(_enemyPrefabs.Count, _spawnPoints.Count);
            for (int i = 0; i < spawnCount; i++)
            {
                Transform spawnPoint = _spawnPoints[i];
                Transform parent = _enemyParent != null ? _enemyParent : spawnPoint;

                EnemyActor spawnedEnemy = Instantiate(_enemyPrefabs[i], spawnPoint.position, spawnPoint.rotation, parent);
                if (spawnedEnemy is MinotaurEnemy minotaur)
                {
                    GraphComponent graphComponent = influenceSystem != null ? influenceSystem.GraphComponent : null;
                    minotaur.InitializeSpawnDependencies(graphComponent, relic);
                }

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

        public void SpawnBosses()
        {
            _activeBosses.Clear();

            if (!_useBosses)
            {
                return;
            }

            int spawnCount = Mathf.Min(_bossPrefabs.Count, _bossSpawnPoints.Count);
            for (int i = 0; i < spawnCount; i++)
            {
                Transform spawnPoint = _bossSpawnPoints[i];
                Transform parent = _enemyParent != null ? _enemyParent : spawnPoint;

                BossActor spawnedBoss = Instantiate(
                    _bossPrefabs[i],
                    spawnPoint.position,
                    spawnPoint.rotation,
                    parent
                );

                spawnedBoss.gameObject.SetActive(false);
                _activeBosses.Add(spawnedBoss);
            }

            if (_activeBosses.Count > 0)
            {
                Debug.Log($"{TAG} {_activeBosses.Count} bosses preparados para a batalha.");
            }
        }

        public void InjectBossesAndStartAI(List<PlayerController> activePlayers, InfluenceSystem influenceSystem)
        {
            if (!_useBosses)
            {
                return;
            }

            foreach (var boss in _activeBosses)
            {
                if (boss == null)
                {
                    continue;
                }

                boss.InitializeAI(activePlayers, influenceSystem);

                if (!boss.IsAIInitialized)
                {
                    Debug.LogWarning($"{TAG} Boss {boss.name} não foi ativado porque a inicialização da IA falhou.");
                    continue;
                }

                boss.gameObject.SetActive(true);
            }
        }
    }
}