using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using Pong.Systems.Input;
using Pong.Gameplay.Enemy;
using Pong.Gameplay.Player;
using Pong.Shared.Management;
using Pong.Systems;
using Sirenix.OdinInspector;

namespace Pong.Gameplay.Scene
{
    public class SceneBootstrapper : MonoBehaviour
    {
        [Header("System References")]
        [SerializeField] private GamepadsManager _gamepadsManager;
        [SerializeField] private MatchUIManager _uiManager;
        [SerializeField] private GameManager _gameManager;
        [SerializeField] private EnemyManager _enemyManager; // <--- NOVA REFERÊNCIA

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
                // 1. SETUP: Abre as portas e Spawna Inimigos
                _currentState = State.WaitingForPlayers;

                _enemyManager.SpawnEnemies(); 
                _gamepadsManager.StartPlayerSpawning();

                while (!_gamepadsManager.AllPlayersReady) { await Task.Yield(); }

                // 2. COUNTDOWN
                _currentState = State.Countdown;
                _uiManager.StartMatchCountdown();

                while (!_uiManager.IsCountdownFinished) { await Task.Yield(); }

                // 3. INJECTION
                _currentState = State.Injecting;

                List<GameObject> playerObjects = _gamepadsManager.GetActivePlayerInstances();
                List<PlayerController> activePlayers = new List<PlayerController>();
                foreach (var obj in playerObjects)
                {
                    if (obj.TryGetComponent(out PlayerController controller))
                    {
                        activePlayers.Add(controller);
                    }
                }

                // <--- DELEGA A INJEÇÃO DE DEPENDÊNCIAS
                _enemyManager.InjectTargetsAndStartAI(activePlayers);

                // 4. PLAY
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