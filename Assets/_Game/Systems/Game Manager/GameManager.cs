using Pong.Core;
using UnityEngine;

namespace Pong.Systems
{
    public class GameManager : MonoBehaviour
    {
        public static GameManager Instance { get; private set; }
        [SerializeField] private GameState _initialState = GameState.Playing;

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }

            Instance = this;
        }
        private void Start()
        {
            GameStateSystem.Instance.ChangeState(_initialState);
        }
    }
}
