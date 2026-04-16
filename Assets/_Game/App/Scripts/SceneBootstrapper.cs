using System.Collections.Generic;
using System.Threading.Tasks;
using Pong.Gameplay;
using Pong.Gameplay.Enemy;
using Pong.Gameplay.Life;
using Pong.Gameplay.Player;
using Pong.Shared.Management;
using Pong.Systems.Input;
using Pong.Systems.Graph;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Pong.App
{
    public class SceneBootstrapper : MonoBehaviour
    {
        [Header("System References")]
        [SerializeField] private GamepadsManager _gamepadsManager;
        [SerializeField] private MatchUIManager _uiManager;
        [SerializeField] private GameManager _gameManager;
        [SerializeField] private EnemyManager _enemyManager;
        [SerializeField] private InfluenceSystem _influenceSystem;
        [SerializeField] private LifeManagerHud _lifeManagerHud;

        private enum State { Initializing, WaitingForPlayers, Countdown, Injecting, Playing }

        [SerializeField, ReadOnly]
        private State _currentState = State.Initializing;

        private async void Start()
        {
            await RunSceneSetupFlow();
        }

        private async Task RunSceneSetupFlow()
        {
            try
            {
                _currentState = State.WaitingForPlayers;

                _enemyManager.SpawnEnemies(_influenceSystem, _gameManager != null ? _gameManager.RelicObject : null);
                _enemyManager.SpawnBosses();
                _gamepadsManager.StartPlayerSpawning();

                while (!_gamepadsManager.AllPlayersReady)
                {
                    await Task.Yield();
                }

                _currentState = State.Countdown;
                _lifeManagerHud?.SetVisible(false);
                _uiManager.StartMatchCountdown();

                while (!_uiManager.IsCountdownFinished)
                {
                    await Task.Yield();
                }

                _lifeManagerHud?.SetVisible(true);

                _currentState = State.Injecting;

                List<GameObject> playerObjects = _gamepadsManager.GetActivePlayerInstances();
                List<PlayerController> activePlayers = new List<PlayerController>();

                foreach (var obj in playerObjects)
                {
                    if (obj != null && obj.TryGetComponent(out PlayerController controller))
                    {
                        activePlayers.Add(controller);
                    }
                }

                _enemyManager.InjectTargetsAndStartAI(activePlayers, _influenceSystem);
                _enemyManager.InjectBossesAndStartAI(activePlayers, _influenceSystem);
                
                await _gameManager.RevealRelic();

                _currentState = State.Playing;
                _gameManager.SetMatchActive(true);

            }
            catch (System.Exception e)
            {
                Debug.LogError($"[SceneBootstrapper] Erro crítico no fluxo: {e.Message}");
            }
        }
    }
}