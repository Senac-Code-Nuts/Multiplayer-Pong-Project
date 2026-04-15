using Pong.Core;
using UnityEngine;

namespace Pong.Systems
{
    public class GameManager : MonoBehaviour
    {
        public static GameManager Instance { get; private set; }

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }

            Instance = this;
        }

        public void SetMatchActive(bool isActive)
        {
            if (isActive)
            {
                Debug.Log("<color=green><b>[GameManager]</b> A partida começou!</color>");
                
                GameStateSystem.Instance.ChangeState(GameState.Playing);
            }
            else
            {
                Debug.Log("<color=orange><b>[GameManager]</b> Partida interrompida.</color>");
            }
        }
    }
}