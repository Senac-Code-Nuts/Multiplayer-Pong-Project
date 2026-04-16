using System.Threading.Tasks;
using Pong.Core;
using Pong.Gameplay.Relics;
using UnityEngine;

namespace Pong.Gameplay
{
    public class GameManager : MonoBehaviour
    {
        public static GameManager Instance { get; private set; }

        [SerializeField] private Relic _relicObject;

        private const string TAG = "<color=green><b>[GameManager]</b></color> ";

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
                Debug.Log($"{TAG} A partida começou!");

                GameStateSystem.Instance.ChangeState(GameState.Playing);
            }
            else
            {
                Debug.Log($"{TAG} Partida interrompida.");
            }
        }

        public async Task RevealRelic()
        {
            if (_relicObject == null)
            {
                Debug.LogWarning($"{TAG} Nenhuma Relíquia atribuída para revelar.");
                return;
            }

            await _relicObject.AnimateAppearanceAsync(1.5f);
        }
    }
}