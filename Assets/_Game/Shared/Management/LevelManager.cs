using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using Pong.Systems.Input;
using UnityEngine;

namespace Pong.Shared.Management
{
    public class LevelManager : MonoBehaviour
    {
        public static LevelManager Instance { get; private set; }

        private enum MatchState
        {
            Setup,
            Countdown,
            Playing,
            Finished
        }

        [Header("Dependencies")]
        [SerializeField] private GamepadsManager _gamepadsManager;
        [SerializeField] private MonoBehaviour _matchUIManagerBehaviour;
        [SerializeField] private List<MonoBehaviour> _enemyControllers = new List<MonoBehaviour>();

        [Header("Flow")]
        [SerializeField, Min(0.05f)] private float _playerSpawnPollInterval = 0.1f;

        [SerializeField] private MatchState _currentState;
        private Coroutine _matchFlowRoutine;

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;

            SetEnemiesActive(false);
        }

        private void Start()
        {
            _currentState = MatchState.Setup;
            SetEnemiesActive(false);

            if (_gamepadsManager == null)
            {
                _gamepadsManager = GamepadsManager.Instance;
            }

            _matchFlowRoutine = StartCoroutine(RunMatchFlow());
        }

        private IEnumerator RunMatchFlow()
        {
            yield return WaitForPlayersToSpawn();

            _currentState = MatchState.Countdown;

            if (TryStartCountdown())
            {
                yield return WaitForCountdownToFinish();
            }

            _currentState = MatchState.Playing;

            List<Transform> activePlayers = ResolveActivePlayers();
            InitializeEnemies(activePlayers);
            TriggerGoEvent();
            yield return WaitForGoToFinish();
            SetEnemiesActive(true);
        }

        private IEnumerator WaitForPlayersToSpawn()
        {
            while (!ArePlayersReady())
            {
                yield return new WaitForSeconds(_playerSpawnPollInterval);
            }
        }

        private bool ArePlayersReady()
        {
            GamepadsManager manager = ResolveGamepadsManager();
            return manager != null && manager.IsInitialSpawnCompleted;
        }

        private GamepadsManager ResolveGamepadsManager()
        {
            if (_gamepadsManager != null)
            {
                return _gamepadsManager;
            }

            return GamepadsManager.Instance;
        }

        private List<Transform> ResolveActivePlayers()
        {
            GamepadsManager manager = ResolveGamepadsManager();
            if (manager == null)
            {
                return new List<Transform>();
            }

            return manager.GetActivePlayerTransforms();
        }

        private bool TryStartCountdown()
        {
            if (_matchUIManagerBehaviour == null)
            {
                Debug.LogWarning("[LevelManager] MatchUIManager não atribuído. O fluxo de countdown ficará aguardando a UI.");
                return false;
            }

            MethodInfo startCountdownMethod = _matchUIManagerBehaviour.GetType().GetMethod(
                "StartCountdown",
                BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic
            );

            if (startCountdownMethod == null)
            {
                Debug.LogWarning($"[LevelManager] {_matchUIManagerBehaviour.GetType().Name} não expõe StartCountdown().");
                return false;
            }

            startCountdownMethod.Invoke(_matchUIManagerBehaviour, Array.Empty<object>());
            return true;
        }

        private IEnumerator WaitForCountdownToFinish()
        {
            while (!IsCountdownFinished())
            {
                yield return null;
            }
        }

        private bool IsCountdownFinished()
        {
            if (_matchUIManagerBehaviour == null)
            {
                return false;
            }

            PropertyInfo finishedProperty = _matchUIManagerBehaviour.GetType().GetProperty(
                "IsCountdownFinished",
                BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic
            );

            if (finishedProperty != null && finishedProperty.PropertyType == typeof(bool))
            {
                return (bool)finishedProperty.GetValue(_matchUIManagerBehaviour);
            }

            MethodInfo finishedMethod = _matchUIManagerBehaviour.GetType().GetMethod(
                "IsCountdownFinished",
                BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic
            );

            if (finishedMethod != null && finishedMethod.ReturnType == typeof(bool) && finishedMethod.GetParameters().Length == 0)
            {
                return (bool)finishedMethod.Invoke(_matchUIManagerBehaviour, Array.Empty<object>());
            }

            return false;
        }

        private void InitializeEnemies(List<Transform> activePlayers)
        {
            if (_enemyControllers == null || _enemyControllers.Count == 0)
            {
                return;
            }

            for (int i = 0; i < _enemyControllers.Count; i++)
            {
                MonoBehaviour enemyController = _enemyControllers[i];
                if (enemyController == null)
                {
                    continue;
                }

                MethodInfo initializeMethod = enemyController.GetType().GetMethod(
                    "InitializeAI",
                    BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic,
                    binder: null,
                    types: new[] { typeof(List<Transform>) },
                    modifiers: null
                );

                if (initializeMethod == null)
                {
                    Debug.LogWarning($"[LevelManager] {enemyController.name} não expõe InitializeAI(List<Transform>).");
                    continue;
                }

                initializeMethod.Invoke(enemyController, new object[] { activePlayers });
            }
        }

        private void SetEnemiesActive(bool isActive)
        {
            if (_enemyControllers == null || _enemyControllers.Count == 0)
            {
                return;
            }

            for (int i = 0; i < _enemyControllers.Count; i++)
            {
                MonoBehaviour enemyController = _enemyControllers[i];
                if (enemyController == null)
                {
                    continue;
                }

                enemyController.gameObject.SetActive(isActive);
            }
        }

        private void TriggerGoEvent()
        {
            if (_matchUIManagerBehaviour == null)
            {
                return;
            }

            MethodInfo goMethod = _matchUIManagerBehaviour.GetType().GetMethod(
                "ShowGo",
                BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic
            );

            if (goMethod == null)
            {
                goMethod = _matchUIManagerBehaviour.GetType().GetMethod(
                    "PlayGoAnimation",
                    BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic
                );
            }

            if (goMethod == null)
            {
                Debug.LogWarning($"[LevelManager] {_matchUIManagerBehaviour.GetType().Name} não expõe ShowGo() nem PlayGoAnimation().");
                return;
            }

            goMethod.Invoke(_matchUIManagerBehaviour, Array.Empty<object>());
        }

        private IEnumerator WaitForGoToFinish()
        {
            while (!IsGoFinished())
            {
                yield return null;
            }
        }

        private bool IsGoFinished()
        {
            if (_matchUIManagerBehaviour == null)
            {
                return false;
            }

            PropertyInfo finishedProperty = _matchUIManagerBehaviour.GetType().GetProperty(
                "IsGoFinished",
                BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic
            );

            if (finishedProperty != null && finishedProperty.PropertyType == typeof(bool))
            {
                return (bool)finishedProperty.GetValue(_matchUIManagerBehaviour);
            }

            MethodInfo finishedMethod = _matchUIManagerBehaviour.GetType().GetMethod(
                "IsGoFinished",
                BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic
            );

            if (finishedMethod != null && finishedMethod.ReturnType == typeof(bool) && finishedMethod.GetParameters().Length == 0)
            {
                return (bool)finishedMethod.Invoke(_matchUIManagerBehaviour, Array.Empty<object>());
            }

            return false;
        }
    }
}
