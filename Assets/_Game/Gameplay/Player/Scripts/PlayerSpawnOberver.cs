using UnityEngine;
using Pong.Core;
using Pong.Gameplay.Player;

namespace Pong.Gameplay
{
    public class PlayerSpawnObserver : MonoBehaviour
    {
        private void OnEnable()
        {
            PlayerSpawnEvents.OnPlayerSpawned += HandlePlayerSpawned;
        }

        private void OnDisable()
        {
            PlayerSpawnEvents.OnPlayerSpawned -= HandlePlayerSpawned;
        }

        private void HandlePlayerSpawned(GameObject playerObject, int playerIndex)
        {
            if (playerObject == null) return;

            PlayerActor playerActor = playerObject.GetComponent<PlayerActor>();
            if (playerActor == null) return;

            playerActor.SetPlayerOrder(playerIndex);
        }
    }
}