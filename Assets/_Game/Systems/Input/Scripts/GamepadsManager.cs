using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using System;
using Pong.Core;

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
        public static GamepadsManager Instance { get; private set; }

        private const int MAX_PLAYERS = 4;
        private const int MIN_PLAYERS = 2;
        private const int DEFAULT_PLAYERS = 2;
        private const string GAMEPAD_TAG = "<color=yellow><b>[Gamepads Manager]</b></color>";

        [SerializeField] private PlayerSlot[] _playerSlots = new PlayerSlot[MAX_PLAYERS];
        private PlayerData[] _activePlayers = new PlayerData[MAX_PLAYERS];

        [SerializeField, Range(2, 4)] private int _playerCount = DEFAULT_PLAYERS;
        [SerializeField] private bool _enableKeyboardForTesting = false;

        [SerializeField] private Transform _playerContainer;
        [SerializeField] private GameObject _spawnVfxPrefab;
        [SerializeField, Min(0f)] private float _spawnVfxDuration = 0.35f;
        [SerializeField, Min(0f)] private float _spawnVfxLifetime = 2f;
        [SerializeField, Min(0f)] private float _spawnSequenceGap = 0.15f;

        private bool _initialSpawnCompleted;
        private int _pendingInitialSpawns;

        private int PlayerCount
        {
            get
            {
                return _playerCount;
            }
        }

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
            _initialSpawnCompleted = false;
            _pendingInitialSpawns = 0;

            var initialDevices = new List<InputDevice>();
            var hasSpawnRequests = false;

            foreach (var gamepad in Gamepad.all)
            {
                initialDevices.Add(gamepad);
            }

            if (_enableKeyboardForTesting && Keyboard.current != null)
            {
                initialDevices.Add(Keyboard.current);
            }

            if (initialDevices.Count > 0)
            {
                hasSpawnRequests = true;
                StartCoroutine(SpawnInitialPlayersSequence(initialDevices));
            }

            if (!hasSpawnRequests)
            {
                _initialSpawnCompleted = true;
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
            if (change == InputDeviceChange.Added && (device is Gamepad || (_enableKeyboardForTesting && device is Keyboard)))
            {
                if (device is Keyboard && IsDeviceAlreadyPaired(device))
                {
                    return;
                }

                Debug.Log($"{GAMEPAD_TAG} Device added: {device.displayName}");
                StartCoroutine(SpawnPlayerRoutine(device, false));
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

        private IEnumerator SpawnInitialPlayersSequence(List<InputDevice> initialDevices)
        {
            for (int i = 0; i < initialDevices.Count; i++)
            {
                InputDevice device = initialDevices[i];
                if (device is Keyboard)
                {
                    if (IsDeviceAlreadyPaired(device))
                    {
                        continue;
                    }

                    int keyboardIndex = GetFirstEmptyIndex();
                    if (keyboardIndex != -1)
                    {
                        yield return SpawnPlayerTestRoutine(keyboardIndex, _playerSlots[keyboardIndex], Keyboard.current, true);
                    }
                }
                else
                {
                    yield return SpawnPlayerRoutine(device, true);
                }

                if (_spawnSequenceGap > 0f)
                {
                    yield return new WaitForSeconds(_spawnSequenceGap);
                }
            }
        }

        private IEnumerator SpawnPlayerRoutine(InputDevice device, bool countsAsInitialSpawn)
        {
            if (device == null || IsDeviceAlreadyPaired(device))
            {
                yield break;
            }

            int index = GetFirstEmptyIndex();
            if (index == -1 || index >= PlayerCount)
            {
                yield break;
            }

            var slot = _playerSlots[index];
            if (slot.Prefab == null || slot.SpawnPoint == null)
            {
                yield break;
            }

            if (countsAsInitialSpawn)
            {
                _pendingInitialSpawns++;
            }

            var playerInput = PlayerInput.Instantiate(
                slot.Prefab,
                playerIndex: index,
                pairWithDevice: device
            );

            playerInput.transform.position = slot.SpawnPoint.position;
            playerInput.name = $"Player {index + 1}";
            playerInput.transform.SetParent(_playerContainer, true);

            if (countsAsInitialSpawn)
            {
                playerInput.gameObject.SetActive(false);
            }

            _activePlayers[index] = new PlayerData
            {
                Instance = playerInput.gameObject,
                Device = device,
                Side = slot.PlayerSide
            };

            PlayerSpawnEvents.RaisePlayerSpawned(playerInput.gameObject, index);

            Debug.Log($"{GAMEPAD_TAG} Spawned player for device: {device.displayName} at index {index}");

            if (countsAsInitialSpawn)
            {
                _pendingInitialSpawns = Mathf.Max(0, _pendingInitialSpawns - 1);

                if (_pendingInitialSpawns == 0)
                {
                    _initialSpawnCompleted = true;
                }
            }
        }

        private IEnumerator SpawnPlayerTestRoutine(int index, PlayerSlot slot, InputDevice device, bool countsAsInitialSpawn)
        {
            if (slot.Prefab == null || slot.SpawnPoint == null || device == null)
            {
                yield break;
            }

            if (countsAsInitialSpawn)
            {
                _pendingInitialSpawns++;
            }

            // Spawna sem parear com device (apenas para testes)
            var playerInput = PlayerInput.Instantiate(
                slot.Prefab,
                playerIndex: index,
                pairWithDevice: device
            );

            playerInput.transform.position = slot.SpawnPoint.position;
            playerInput.name = $"Player {index + 1} (Test)";

            if (countsAsInitialSpawn)
            {
                playerInput.gameObject.SetActive(false);
            }

            _activePlayers[index] = new PlayerData
            {
                Instance = playerInput.gameObject,
                Device = device,
                Side = slot.PlayerSide
            };

            Debug.Log($"{GAMEPAD_TAG} <color=orange>Spawned test player at index {index} (no device)</color>");

            if (countsAsInitialSpawn)
            {
                _pendingInitialSpawns = Mathf.Max(0, _pendingInitialSpawns - 1);

                if (_pendingInitialSpawns == 0)
                {
                    _initialSpawnCompleted = true;
                }
            }
        }

        public IEnumerator RevealPlayerRoutine(int index)
        {
            if (index < 0 || index >= _activePlayers.Length)
            {
                yield break;
            }

            PlayerData playerData = _activePlayers[index];
            if (playerData == null || playerData.Instance == null)
            {
                yield break;
            }

            Transform spawnPoint = _playerSlots[index].SpawnPoint;
            if (spawnPoint != null && _spawnVfxPrefab != null)
            {
                GameObject vfxInstance = Instantiate(
                    _spawnVfxPrefab,
                    spawnPoint.position,
                    spawnPoint.rotation
                );

                if (_spawnVfxLifetime > 0f)
                {
                    Destroy(vfxInstance, _spawnVfxLifetime);
                }

                if (_spawnVfxDuration > 0f)
                {
                    yield return new WaitForSeconds(_spawnVfxDuration);
                }
            }

            playerData.Instance.SetActive(true);
        }

        private IEnumerator PlaySpawnVfxRoutine(Transform spawnPoint)
        {
            if (_spawnVfxPrefab == null || spawnPoint == null)
            {
                yield break;
            }

            GameObject vfxInstance = Instantiate(
                _spawnVfxPrefab,
                spawnPoint.position,
                spawnPoint.rotation
            );

            if (_spawnVfxLifetime > 0f)
            {
                Destroy(vfxInstance, _spawnVfxLifetime);
            }

            if (_spawnVfxDuration > 0f)
            {
                yield return new WaitForSeconds(_spawnVfxDuration);
            }
        }

        public GameObject[] GetActivePlayerControllers()
        {
            var playerObjects = new GameObject[_activePlayers.Length];
            for (int i = 0; i < _activePlayers.Length; i++)
            {
                playerObjects[i] = _activePlayers[i]?.Instance;
            }
            return playerObjects;
        }

        public bool IsInitialSpawnCompleted => _initialSpawnCompleted;

        public int GetPlayerCount()
        {
            return _playerCount;
        }

        public List<Transform> GetActivePlayerTransforms()
        {
            var activePlayers = new List<Transform>();

            for (int i = 0; i < _activePlayers.Length; i++)
            {
                if (_activePlayers[i]?.Instance != null)
                {
                    activePlayers.Add(_activePlayers[i].Instance.transform);
                }
            }

            return activePlayers;
        }

        public void ActivatePlayerAtIndex(int index)
        {
            if (index < 0 || index >= _activePlayers.Length)
            {
                return;
            }

            GameObject playerObject = _activePlayers[index]?.Instance;
            if (playerObject == null || playerObject.activeSelf)
            {
                return;
            }

            playerObject.SetActive(true);
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
        #endregion
    }
}