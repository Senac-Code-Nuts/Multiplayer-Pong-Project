using UnityEngine;
using UnityEngine.InputSystem;
using System;
using Pong.Core;
using System.Collections.Generic;
using Pong.Systems.Audio;

namespace Pong.Systems.Input
{
    [Serializable]
    struct PlayerSlot
    {
        public GameObject Prefab;
        public Transform SpawnPoint;
        public PlayerSide PlayerSide;
    }

    public class GamepadsManager : MonoBehaviour
    {
        private const int MAX_PLAYERS = 4;
        private const int MIN_PLAYERS = 2;
        private const int DEFAULT_PLAYERS = 2;
        private const string GAMEPAD_TAG = "<color=yellow><b>[Gamepads Manager]</b></color>";

        [SerializeField] private PlayerSlot[] _playerSlots = new PlayerSlot[MAX_PLAYERS];
        private PlayerData[] _activePlayers = new PlayerData[MAX_PLAYERS];

        [SerializeField, Range(2, 4)] private int _playerCount = DEFAULT_PLAYERS;
        [SerializeField] private bool _enableKeyboardForTesting = false;

        [SerializeField] private Transform _playerParent;

        [Header("Audio Settings")]
        [SerializeField] private AudioClip _poofClip;

        private bool _isSpawningAllowed = false;

        private int PlayerCount
        {
            get { return _playerCount; }
        }

        public bool AllPlayersReady
        {
            get
            {
                int count = 0;
                for (int i = 0; i < PlayerCount; i++)
                {
                    if (_activePlayers[i] != null) count++;
                }
                return count == PlayerCount;
            }
        }

        public void StartPlayerSpawning()
        {
            _isSpawningAllowed = true;
            Debug.Log($"{GAMEPAD_TAG} Spawning permitido pelo Bootstrapper. Procurando dispositivos ativos...");

            foreach (var gamepad in Gamepad.all)
            {
                if(_poofClip != null)
                {
                    AudioManager.Instance.PlaySFX(_poofClip);
                }
                SpawnPlayerForDevice(gamepad);
            }

            if (_enableKeyboardForTesting && Keyboard.current != null)
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

        private void OnValidate()
        {
            if (_playerCount != MIN_PLAYERS && _playerCount != MAX_PLAYERS)
            {
                _playerCount = DEFAULT_PLAYERS;
            }
        }

        private void OnDeviceChange(InputDevice device, InputDeviceChange change)
        {
            if (change == InputDeviceChange.Added && _isSpawningAllowed && !AllPlayersReady)
            {
                if (device is Gamepad || (_enableKeyboardForTesting && device is Keyboard))
                {
                    Debug.Log($"{GAMEPAD_TAG} Device added: {device.displayName}");
                    SpawnPlayerForDevice(device);
                }
            }

            if (change == InputDeviceChange.Removed)
            {
                int index = FindPlayerIndexForDevice(device);

                if (index == -1) return;

                if (_activePlayers[index]?.Instance != null)
                {
                    Destroy(_activePlayers[index].Instance);
                }

                _activePlayers[index] = null;

                Debug.Log($"{GAMEPAD_TAG} Device removed: {device.displayName} (Index: {index})");
            }
        }

        private void SpawnPlayerForDevice(InputDevice device)
        {
            if (!_isSpawningAllowed) return; 
            if (IsDeviceAlreadyPaired(device)) return;

            int index = GetFirstEmptyIndex();
            if (index == -1) return;
            if (index >= PlayerCount) return;

            var slot = _playerSlots[index];
            if (slot.Prefab == null || slot.SpawnPoint == null) return;

            var playerInput = PlayerInput.Instantiate(
                slot.Prefab,
                playerIndex: index,
                pairWithDevice: device
            );

            playerInput.transform.position = slot.SpawnPoint.position;
            playerInput.name = $"Player {index + 1}";
            playerInput.transform.SetParent(_playerParent, true);

            _activePlayers[index] = new PlayerData
            {
                Instance = playerInput.gameObject,
                Device = device,
                Side = slot.PlayerSide
            };
            PlayerSpawnEvents.RaisePlayerSpawned(playerInput.gameObject, index, (int)slot.PlayerSide);

            Debug.Log($"{GAMEPAD_TAG} Spawned player for device: {device.displayName} at index {index}");
        }

        #region Device Helpers
        private int FindPlayerIndexForDevice(InputDevice device)
        {
            for (int i = 0; i < _activePlayers.Length; i++)
            {
                if (_activePlayers[i] != null && _activePlayers[i].Device == device)
                    return i;
            }
            return -1;
        }

        private int GetFirstEmptyIndex()
        {
            for (int i = 0; i < PlayerCount; i++)
            {
                if (_activePlayers[i] == null) return i;
            }
            return -1;
        }

        private bool IsDeviceAlreadyPaired(InputDevice device)
        {
            return FindPlayerIndexForDevice(device) != -1;
        }

        public List<GameObject> GetActivePlayerInstances()
        {
            List<GameObject> instances = new List<GameObject>();
            for (int i = 0; i < PlayerCount; i++)
            {
                if (_activePlayers[i] != null && _activePlayers[i].Instance != null)
                {
                    instances.Add(_activePlayers[i].Instance);
                }
            }
            return instances;
        }
        #endregion
    }
}