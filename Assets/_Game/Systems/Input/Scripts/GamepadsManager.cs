using UnityEngine;
using UnityEngine.InputSystem;
using System;

namespace Pong.Systems
{
    public class GamepadsManager : MonoBehaviour
    {
        private const int MAX_PLAYERS = 4;

        [SerializeField] private PlayerSlot[] _playerSlots = new PlayerSlot[MAX_PLAYERS];
        [Serializable]
        struct PlayerSlot
        {
            public GameObject Prefab;
            public Transform SpawnPoint;
        }
        private GameObject[] _activePlayers = new GameObject[MAX_PLAYERS];

        private void Start()
        {
            foreach (var gamepad in Gamepad.all)
            {
                SpawnPlayerForDevice(gamepad);
            }

            if (Keyboard.current != null)
            {
                SpawnPlayerForDevice(Keyboard.current);
            }
        }

        private void OnEnable()
        {
            InputSystem.onDeviceChange += OnDeviceChange;
        }

        private void OnDisable()
        {
            InputSystem.onDeviceChange -= OnDeviceChange;
        }

        private void OnDeviceChange(InputDevice device, InputDeviceChange change)
        {
            if (change == InputDeviceChange.Added && (device is Gamepad || device is Keyboard))
            {
                SpawnPlayerForDevice(device);
            }

            if (change == InputDeviceChange.Removed)
            {
                int index = FindPlayerIndexForDevice(device);

                if (index == -1) return;

                Destroy(_activePlayers[index]);
                _activePlayers[index] = null;
            }
        }
        private void SpawnPlayerForDevice(InputDevice device)
        {
            int index = GetFirstEmptyIndex();
            if (index == -1 || _playerSlots.Length <= index) return;

            var slot = _playerSlots[index];
            if (slot.Prefab == null || slot.SpawnPoint == null) return;

            var playerInput = PlayerInput.Instantiate(
                slot.Prefab,
                playerIndex: index,
                pairWithDevice: device
            );

            playerInput.transform.position = slot.SpawnPoint.position;
            playerInput.name = $"Player {index + 1}";

            _activePlayers[index] = playerInput.gameObject;
        }

        #region Device Helpers
        private int FindPlayerIndexForDevice(InputDevice device)
        {
            for (int i = 0; i < _activePlayers.Length; i++)
            {
                var player = _activePlayers[i];

                if (player == null) continue;

                if (!player.TryGetComponent<PlayerInput>(out var playerInput)) continue;

                for (int j = 0; j < playerInput.devices.Count; j++)
                {
                    if (playerInput.devices[j] == device)
                    {
                        return i;
                    }
                }
            }

            return -1;
        }

        private int GetFirstEmptyIndex()
        {
            for (int i = 0; i < _activePlayers.Length; i++)
            {
                if (_activePlayers[i] == null) return i;
            }
            return -1;
        }
        #endregion
    }
}