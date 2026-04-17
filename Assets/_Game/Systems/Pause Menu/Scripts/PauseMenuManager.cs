using Pong.Core;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Pong.Systems
{
    public class PauseMenuManager : MonoBehaviour
    {
        public static PauseMenuManager Instance { get; private set; }
        private GameStateSystem gameStateSystem => GameStateSystem.Instance;

        [SerializeField] private GameObject _pauseMenuContainer;

        private string _mainMenuSceneName = "MenuScene";

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }

            Instance = this;

            GameStateSystem.OnGameStateChanged += HandleGameStateChanged;
        }

        private void Start()
        {
            if (gameStateSystem != null)
            {
                HandleGameStateChanged(gameStateSystem.CurrentState);
            }
        }

        private void OnDestroy()
        {
            GameStateSystem.OnGameStateChanged -= HandleGameStateChanged;

            if (Instance == this)
            {
                Instance = null;
            }
        }

        public void TogglePauseMenu()
        {
            if (gameStateSystem == null)
            {
                return;
            }

            if (gameStateSystem.CurrentState == GameState.Playing)
            {
                gameStateSystem.ChangeState(GameState.Paused);
            }
            else if (gameStateSystem.CurrentState == GameState.Paused)
            {
                gameStateSystem.ChangeState(GameState.Playing);
            }
        }

        private void HandleGameStateChanged(GameState newState)
        {
            bool isPaused = newState == GameState.Paused;

            if (_pauseMenuContainer != null)
            {
                _pauseMenuContainer.SetActive(isPaused);
            }

            Time.timeScale = isPaused ? 0f : 1f;
        }
        public void ReturnToMainMenu()
        {
            Time.timeScale = 1f;
            SceneManager.LoadScene(_mainMenuSceneName);
        }
    }
}
