using System;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Pong.Core
{
    public class GameStateSystem : MonoBehaviour
    {
        public static GameStateSystem Instance { get; private set; }

        [field: SerializeField, EnumPaging] public GameState CurrentState { get; private set; }

        public static event Action<GameState> OnGameStateChanged;

        private const string GAME_STATE_TAG = "<color=blue>[GameState]:</color>";


        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }

            Instance = this;
            ResetToMenu();
        }

        public void ResetToMenu()
        {
            CurrentState = GameState.Menu;
            OnGameStateChanged?.Invoke(CurrentState);
        }

        public void ChangeState(GameState newState)
        {
            if (CurrentState == newState) return;

            CurrentState = newState;
            OnGameStateChanged?.Invoke(CurrentState);

            Debug.Log($"{GAME_STATE_TAG} State changed to: {CurrentState}");
        }
    }
}
