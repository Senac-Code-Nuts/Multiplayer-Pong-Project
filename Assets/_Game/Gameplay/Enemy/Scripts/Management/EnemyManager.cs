using Pong.Gameplay.Boss;
using Pong.Gameplay.Enemy;
using Pong.Gameplay.Player;
using Pong.Gameplay.Relics;
using Pong.Systems.Audio;
using Pong.Systems.Graph;
using Pong.Systems.MapSelection;
using Sirenix.OdinInspector;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

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

        [Header("Scene")]
        [SerializeField] private string _mapSceneName = "Map";

        [Header("Music")]
        [SerializeField] private string _normalRoomMusicPath = "Musica/musica_pong_final";
        [SerializeField] private string _bossRoomMusicPath = "Musica/Musica_Boss_Pongatorio";
        [SerializeField, Range(0f, 1f)] private float _musicVolume = 0.35f;
        [SerializeField] private int _musicChannel = 0;

        private bool _hasFinishedRoom;

        private List<EnemyActor> _activeEnemies = new List<EnemyActor>();
        private List<BossActor> _activeBosses = new List<BossActor>();

        [SerializeField] private Transform _enemyParent;

        private const string TAG = "<color=red><b>[EnemyManager]</b></color>";

        private void Update()
        {
            if (_hasFinishedRoom)
                return;

            if(_useBosses)
            {
                if(AreAllBossesDefeated())
                {
                    CompleteEnemyRoom();
                    return;
                }
            }

            if (!_useBosses && AreAllEnemiesDefeated())
            {
                CompleteEnemyRoom();
                return;
            }

            if (AreAllPlayersDead())
            {
                ReturnToMapWithoutCompleting();
            }
        }

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

            PlayRoomMusic(false);
            
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

            PlayRoomMusic(true);

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

        private void CompleteEnemyRoom()
        {
            _hasFinishedRoom = true;

            Debug.Log($"{TAG} Sala concluída.");

            if (MapProgressManager.Instance != null)
            {
                MapProgressManager.Instance.CompleteCurrentPhase();
            }

            SceneManager.LoadScene(_mapSceneName);
        }

        private void ReturnToMapWithoutCompleting()
        {
            _hasFinishedRoom = true;

            Debug.Log($"{TAG} Todos os jogadores morreram. Voltando para o mapa.");

            SceneManager.LoadScene(_mapSceneName);
        }

        private bool AreAllPlayersDead()
        {
            PlayerActor[] players = FindObjectsByType<PlayerActor>(FindObjectsSortMode.None);

            if (players == null || players.Length == 0)
                return false;

            for (int i = 0; i < players.Length; i++)
            {
                if (players[i] != null && players[i].IsAlive)
                    return false;
            }

            return true;
        }

        private bool AreAllEnemiesDefeated()
        {
            if (_activeEnemies.Count == 0)
                return false;

            for (int i = 0; i < _activeEnemies.Count; i++)
            {
                EnemyActor enemy = _activeEnemies[i];

                if (enemy != null && enemy.gameObject.activeInHierarchy)
                    return false;
            }

            return true;
        }

        private bool AreAllBossesDefeated()
        {
            if(_activeBosses.Count == 0)
            {
                return false;
            }

            for(int i = 0; i < _activeBosses.Count; i++)
            {
                BossActor boss = _activeBosses[i];

                if(boss != null && !boss.IsDead)
                {
                    return false;
                }

                
            }

            return true;
        }

        private void PlayRoomMusic(bool bossRoom)
        {
            if (AudioManager.Instance == null)
            {
                return;
            }

            string musicPath = bossRoom ? _bossRoomMusicPath : _normalRoomMusicPath;
            AudioClip clip = Resources.Load<AudioClip>(musicPath);

            if (clip == null)
            {
                Debug.LogWarning($"{TAG} Não foi possível carregar a música '{musicPath}'.");
                return;
            }

            AudioManager.Instance.StopTrack(_musicChannel);
            AudioManager.Instance.PlayTrack(clip, _musicChannel, true, _musicVolume, 1f, 1f, musicPath);
        }
    }
}