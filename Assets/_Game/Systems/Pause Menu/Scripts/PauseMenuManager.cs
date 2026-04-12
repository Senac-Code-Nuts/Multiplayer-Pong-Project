using Pong.Core;
using UnityEngine;

namespace Pong.Systems
{
    public class PauseMenuManager : MonoBehaviour
    {
        public static PauseMenuManager Instance { get; private set; }
        private GameStateSystem gameStateSystem => GameStateSystem.Instance;

        [SerializeField] private GameObject _pauseMenuContainer;

        public void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }

            Instance = this;
        }
        public void TogglePauseMenu()
        {
            if (gameStateSystem.CurrentState == GameState.Playing)
            {
                gameStateSystem.ChangeState(GameState.Paused);
            }
            else if (gameStateSystem.CurrentState == GameState.Paused)
            {
                gameStateSystem.ChangeState(GameState.Playing);
            }
        }
        private void OnEnable()
        {
            GameStateSystem.OnGameStateChanged += HandleGameStateChanged;
        }
        private void OnDisable()
        {
            GameStateSystem.OnGameStateChanged -= HandleGameStateChanged;
        }
        private void HandleGameStateChanged(GameState newState)
        {
            switch (newState)
            {
                case GameState.Playing:
                    _pauseMenuContainer.SetActive(false);
                    Time.timeScale = 1f;
                    break;
                case GameState.Paused:
                    _pauseMenuContainer.SetActive(true);
                    Time.timeScale = 0f;
                    break;
                default:
                    break;
            }
        }
    }
}
