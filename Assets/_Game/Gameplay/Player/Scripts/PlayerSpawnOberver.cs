using UnityEngine;
using Pong.Core;
using Pong.Gameplay.Player;
using Pong.Systems;

namespace Pong.Gameplay
{
    public class PlayerSpawnObserver : MonoBehaviour
    {
        [Header("VFX")]
        [SerializeField] private GameObject _playerSpawnVfxPrefab;
        [SerializeField, Min(0f)] private float _playerSpawnVfxLifetime = 2f;

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

            SpawnPlayerVfx(playerObject.transform.position);

            PlayerActor playerActor = playerObject.GetComponent<PlayerActor>();
            if (playerActor == null) return;

            playerActor.SetPlayerOrder(playerIndex);

            PlayerController playerController = playerObject.GetComponent<PlayerController>();
            if (playerController != null)
            {
                playerController.SetPlayerSide((PlayerSide)playerSide);
            }
        }

        private void SpawnPlayerVfx(Vector3 position)
        {
            if (_playerSpawnVfxPrefab == null) return;

            GameObject vfxInstance = Instantiate(_playerSpawnVfxPrefab, position, Quaternion.identity);
            Destroy(vfxInstance, _playerSpawnVfxLifetime);
        }
    }
}