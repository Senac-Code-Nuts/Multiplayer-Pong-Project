using UnityEngine;
using Pong.Core;
using Pong.Gameplay.Player;
using Pong.Systems;

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

        private void HandlePlayerSpawned(GameObject playerObject, int playerIndex, int playerSide)
        {
            if (playerObject == null) return;

            PlayerActor playerActor = playerObject.GetComponent<PlayerActor>();
            if (playerActor == null) return;

            playerActor.SetPlayerOrder(playerIndex);

            PlayerController playerController = playerObject.GetComponent<PlayerController>();
            if (playerController != null)
            {
                playerController.SetPlayerSide((PlayerSide)playerSide);
            }
        }
    }
}