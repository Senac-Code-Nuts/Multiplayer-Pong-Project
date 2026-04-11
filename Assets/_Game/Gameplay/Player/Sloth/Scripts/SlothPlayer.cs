using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Pong.Gameplay.Player
{
    public class SlothPlayer : PlayerActor
    {
        [Header("Ability")]
        [SerializeField] private PlayerActor[] _players;
        [SerializeField] private bool _isSelectingTarget = false;

        [Header("Selection Markers")]
        [SerializeField] private GameObject _selectionMarkerPrefab;
        [SerializeField, Range(0.1f, 2f)] private float _rotationInterval = 0.5f;
        [SerializeField] private Vector3 _markerOffset = new Vector3(0f, 2f, 0f);

        [Header("Debug")]
        [SerializeField] private bool _useDebug;

        private int _shieldCount = 1;

        private readonly List<GameObject> _activeMarkers = new List<GameObject>();
        private readonly List<int> _currentMarkedIndexes = new List<int>();

        private Coroutine _selectionRoutine;
        private int _currentStartIndex = 0;
        private bool _canConfirmSelection = false;

        protected override void Awake()
        {
            base.Awake();
            UpgradeShieldCount();
        }

        private void Start()
        {
            CachePlayers();
        }

        protected override void LevelUp()
        {
            base.LevelUp();
            UpgradeShieldCount();
        }

        private void UpgradeShieldCount()
        {
            _shieldCount = _level;
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.F1) && _useDebug)
            {
                LevelUp();
            }
        }

        private void CachePlayers()
        {
            _players = FindObjectsByType<PlayerActor>(FindObjectsSortMode.None);

            System.Array.Sort(_players, (a, b) => a.PlayerOrder.CompareTo(b.PlayerOrder));
        }

        private void SetStartIndexToSelf()
        {
            for (int i = 0; i < _players.Length; i++)
            {
                if (_players[i] == this)
                {
                    _currentStartIndex = i;
                    return;
                }
            }

            _currentStartIndex = 0;
        }

        protected override void UseAbility()
        {
            if (!_isSelectingTarget)
            {
                EnterSelectionMode();
                return;
            }

            if (!_canConfirmSelection) return;

            ConfirmShield();
        }

        private void EnterSelectionMode()
        {
            CachePlayers();

            if (_players == null || _players.Length == 0) return;

            if (_shieldCount >= _players.Length)
            {
                ApplyShieldToAllPlayers();
                StartCoroutine(AbilityCooldownRoutine());
                return;
            }

            if (_selectionMarkerPrefab == null) return;

            _isSelectingTarget = true;
            _canConfirmSelection = false;

            SetStartIndexToSelf();

            CreateMarkers();
            UpdateMarkerTargets();

            _selectionRoutine = StartCoroutine(SelectionRotationRoutine());
            StartCoroutine(EnableConfirmNextFrame());
        }

        private void ExitSelectionMode()
        {
            _isSelectingTarget = false;
            _canConfirmSelection = false;

            if (_selectionRoutine != null)
            {
                StopCoroutine(_selectionRoutine);
                _selectionRoutine = null;
            }

            DestroyMarkers();
            _currentMarkedIndexes.Clear();
        }

        private IEnumerator EnableConfirmNextFrame()
        {
            yield return null;
            _canConfirmSelection = true;
        }

        private IEnumerator SelectionRotationRoutine()
        {
            while (_isSelectingTarget)
            {
                yield return new WaitForSeconds(_rotationInterval);

                _currentStartIndex++;

                if (_currentStartIndex >= _players.Length)
                {
                    _currentStartIndex = 0;
                }

                UpdateMarkerTargets();
            }
        }

        private void CreateMarkers()
        {
            DestroyMarkers();

            int markerCount = Mathf.Clamp(_shieldCount, 1, _players.Length);

            for (int i = 0; i < markerCount; i++)
            {
                GameObject marker = Instantiate(_selectionMarkerPrefab);
                _activeMarkers.Add(marker);
            }
        }

        private void DestroyMarkers()
        {
            for (int i = 0; i < _activeMarkers.Count; i++)
            {
                if (_activeMarkers[i] != null)
                {
                    Destroy(_activeMarkers[i]);
                }
            }

            _activeMarkers.Clear();
        }

        private void UpdateMarkerTargets()
        {
            _currentMarkedIndexes.Clear();

            int markerCount = Mathf.Clamp(_shieldCount, 1, _players.Length);

            for (int i = 0; i < markerCount; i++)
            {
                int playerIndex = (_currentStartIndex + i) % _players.Length;
                _currentMarkedIndexes.Add(playerIndex);

                PlayerActor target = _players[playerIndex];
                GameObject marker = _activeMarkers[i];

                if (target == null || marker == null)
                {
                    continue;
                }

                marker.transform.SetParent(target.transform);
                marker.transform.localPosition = _markerOffset;
                marker.transform.localRotation = Quaternion.identity;
            }
        }

        private void ConfirmShield()
        {
            if (_players == null || _players.Length == 0) return;
            if (_currentMarkedIndexes.Count == 0) return;

            for (int i = 0; i < _currentMarkedIndexes.Count; i++)
            {
                int index = _currentMarkedIndexes[i];
                PlayerActor target = _players[index];

                if (target == null) continue;

                target.ReceiveShield();
            }

            ExitSelectionMode();
            StartCoroutine(AbilityCooldownRoutine());
        }

        private void ApplyShieldToAllPlayers()
        {
            for (int i = 0; i < _players.Length; i++)
            {
                PlayerActor target = _players[i];

                if (target == null) continue;

                target.ReceiveShield();
            }
        }

        protected override void OnDamageTaken()
        {
        }

        protected override void OnDeath()
        {
            ExitSelectionMode();
        }
    }
}