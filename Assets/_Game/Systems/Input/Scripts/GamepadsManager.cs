using UnityEngine;
using UnityEngine.InputSystem;
using System;
using Pong.Core;
using Pong.Systems.Selection;
using System.Collections.Generic;

namespace Pong.Systems.Input
{
    [Serializable]
    struct CharacterSpawnEntry
    {
        public CharacterType CharacterType;
        public GameObject Prefab;
        public Transform SpawnPoint;
        public PlayerSide PlayerSide;
        public int PlayerOrder;
    }

    public class GamepadsManager : MonoBehaviour
    {
        private const int MAX_PLAYERS = 4;
        private const int MIN_PLAYERS = 2;
        private const int DEFAULT_PLAYERS = 2;
        private const string GAMEPAD_TAG = "<color=yellow><b>[Gamepads Manager]</b></color>";

        public static GamepadsManager Instance { get; private set; }

        [SerializeField] private CharacterSelectionSession _characterSelectionSession;
        [SerializeField] private CharacterSpawnEntry[] _characterSpawnEntries = new CharacterSpawnEntry[MAX_PLAYERS];

        private PlayerData[] _activePlayers = new PlayerData[MAX_PLAYERS];

        [SerializeField, Range(2, 4)] private int _playerCount = DEFAULT_PLAYERS;
        [SerializeField] private bool _enableKeyboardForTesting = false;
        [SerializeField] private Transform _playerParent;

        private bool _isSpawningAllowed = false;

        private int PlayerCount
        {
            get
            {
                if (_characterSelectionSession != null && _characterSelectionSession.HasAnyRegisteredPlayers)
                {
                    int count = _characterSelectionSession.GetRegisteredPlayerCount();

                    if (count == 2 || count == 4)
                        return count;
                }

                return _playerCount;
            }
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

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Debug.LogWarning($"{GAMEPAD_TAG} Mais de um GamepadsManager encontrado na cena.");
            }

            Instance = this;
        }

        private void OnDestroy()
        {
            if (Instance == this)
            {
                Instance = null;
            }
        }

        public void StartPlayerSpawning()
        {
            _isSpawningAllowed = true;
            Debug.Log($"{GAMEPAD_TAG} Spawning permitido pelo Bootstrapper. Procurando dispositivos ativos...");

            foreach (Gamepad gamepad in Gamepad.all)
            {
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

            bool hasSelectionSessionPlayers = HasSelectionSessionPlayers();

            int index = GetFirstEmptyIndex();
            if (index == -1) return;
            if (index >= PlayerCount) return;

            if (!TryGetSpawnEntry(index, out CharacterSpawnEntry spawnEntry))
            {
                if (!hasSelectionSessionPlayers)
                {
                    if (!TryGetDefaultSpawnEntry(index, out spawnEntry))
                    {
                        Debug.LogWarning($"{GAMEPAD_TAG} Nenhum CharacterSpawnEntry padr�o v�lido encontrado para player index {index}");
                        return;
                    }

                    Debug.Log($"{GAMEPAD_TAG} Sem sele��o do menu. Usando spawn padr�o para player index {index}.");
                }
                else
                {
                    Debug.LogWarning($"{GAMEPAD_TAG} Nenhum CharacterSpawnEntry v�lido encontrado para player index {index}");
                    return;
                }
            }

            if (spawnEntry.Prefab == null || spawnEntry.SpawnPoint == null)
            {
                Debug.LogWarning($"{GAMEPAD_TAG} SpawnEntry inv�lido para {spawnEntry.CharacterType}");
                return;
            }

            PlayerInput playerInput = PlayerInput.Instantiate(
                spawnEntry.Prefab,
                playerIndex: index,
                pairWithDevice: device
            );

            playerInput.transform.position = spawnEntry.SpawnPoint.position;
            playerInput.transform.rotation = spawnEntry.SpawnPoint.rotation;
            playerInput.name = $"{spawnEntry.CharacterType} Player";
            playerInput.transform.SetParent(_playerParent, true);

            _activePlayers[index] = new PlayerData
            {
                Instance = playerInput.gameObject,
                Device = device,
                Side = spawnEntry.PlayerSide
            };

            PlayerSpawnEvents.RaisePlayerSpawned(
                playerInput.gameObject,
                spawnEntry.PlayerOrder,
                (int)spawnEntry.PlayerSide
            );

            Debug.Log($"{GAMEPAD_TAG} Spawned {spawnEntry.CharacterType} for device: {device.displayName} at index {index}");
        }

        private bool TryGetSpawnEntry(int playerIndex, out CharacterSpawnEntry spawnEntry)
        {
            spawnEntry = default;

            if (!HasSelectionSessionPlayers())
                return false;

            if (!_characterSelectionSession.TryGetSelectedCharacter(playerIndex, out CharacterType selectedCharacter))
                return false;

            for (int i = 0; i < _characterSpawnEntries.Length; i++)
            {
                if (_characterSpawnEntries[i].CharacterType == selectedCharacter)
                {
                    spawnEntry = _characterSpawnEntries[i];
                    return true;
                }
            }

            return false;
        }

        private bool TryGetDefaultSpawnEntry(int playerIndex, out CharacterSpawnEntry spawnEntry)
        {
            spawnEntry = default;

            if (playerIndex < 0 || playerIndex >= _characterSpawnEntries.Length)
                return false;

            spawnEntry = _characterSpawnEntries[playerIndex];

            return spawnEntry.Prefab != null && spawnEntry.SpawnPoint != null;
        }

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
                if (_activePlayers[i] == null)
                    return i;
            }

            return -1;
        }

        private bool IsDeviceAlreadyPaired(InputDevice device)
        {
            return FindPlayerIndexForDevice(device) != -1;
        }

        private bool HasSelectionSessionPlayers()
        {
            return _characterSelectionSession != null && _characterSelectionSession.HasAnyRegisteredPlayers;
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

        public GameObject[] GetActivePlayerControllers()
        {
            return GetActivePlayerInstances().ToArray();
        }
    }
}