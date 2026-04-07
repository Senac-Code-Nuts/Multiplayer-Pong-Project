using System.Collections;
using UnityEngine;

namespace Pong.Gameplay.Player
{
    public class SlothPlayer : PlayerActor
    {
        [Header("Ability")]
        [SerializeField] private PlayerActor[] _players;
        [SerializeField] private int _selectedTargetIndex = 0;
        [SerializeField] private bool _isSelectingTarget = false;

        [Header("Selection")]
        [SerializeField] private float _selectionInputCooldown = 0.2f;
        [SerializeField] private GameObject _selectionMarkerPrefab;
        [SerializeField] private Vector3 _selectionMarkerOffset = new Vector3(0f, 2f, 0f);

        private bool _canChangeSelection = true;
        private GameObject _selectionMarkerInstance;

        protected override void OnEnable()
        {
            base.OnEnable();

            if (_inputReader != null)
                _inputReader.MoveEvent += HandleSelectionMove;
        }

        protected override void OnDisable()
        {
            base.OnDisable();

            if (_inputReader != null)
                _inputReader.MoveEvent -= HandleSelectionMove;
        }

        protected override void UseAbility()
        {
            if (!_isSelectingTarget)
            {
                EnterSelectionMode();
                return;
            }

            ConfirmShield();
        }

        private void EnterSelectionMode()
        {
            if (_players == null || _players.Length == 0)
            {
                Debug.LogWarning($"{gameObject.name} has no players assigned.");
                return;
            }

            _isSelectingTarget = true;
            _canChangeSelection = true;

            ShowSelectionMarker();
            UpdateSelectionMarker();

            Debug.Log($"{gameObject.name} entered selection mode.");
            Debug.Log($"{gameObject.name} current target: {_players[_selectedTargetIndex].gameObject.name}");
        }

        private void ExitSelectionMode()
        {
            _isSelectingTarget = false;
            HideSelectionMarker();

            Debug.Log($"{gameObject.name} exited selection mode.");
        }

        private void HandleSelectionMove(Vector2 input)
        {
            if (!_isSelectingTarget || !_canChangeSelection)
                return;

            if (input.x <= -0.5f)
            {
                SelectLeftTarget();
                StartCoroutine(SelectionInputCooldownRoutine());
            }
            else if (input.x >= 0.5f)
            {
                SelectRightTarget();
                StartCoroutine(SelectionInputCooldownRoutine());
            }
        }

        private IEnumerator SelectionInputCooldownRoutine()
        {
            _canChangeSelection = false;
            yield return new WaitForSeconds(_selectionInputCooldown);
            _canChangeSelection = true;
        }

        private void SelectLeftTarget()
        {
            _selectedTargetIndex--;

            if (_selectedTargetIndex < 0)
                _selectedTargetIndex = _players.Length - 1;

            UpdateSelectionMarker();
            Debug.Log($"{gameObject.name} selected {_players[_selectedTargetIndex].gameObject.name}");
        }

        private void SelectRightTarget()
        {
            _selectedTargetIndex++;

            if (_selectedTargetIndex >= _players.Length)
                _selectedTargetIndex = 0;

            UpdateSelectionMarker();
            Debug.Log($"{gameObject.name} selected {_players[_selectedTargetIndex].gameObject.name}");
        }

        private void ConfirmShield()
        {
            if (_players == null || _players.Length == 0)
                return;

            PlayerActor selectedPlayer = _players[_selectedTargetIndex];

            Debug.Log($"{gameObject.name} granted shield to {selectedPlayer.gameObject.name}");

            // FUTURO:
            // selectedPlayer.ReceiveShield();

            ExitSelectionMode();
        }

        private void ShowSelectionMarker()
        {
            if (_selectionMarkerPrefab == null)
                return;

            if (_selectionMarkerInstance == null)
            {
                _selectionMarkerInstance = Instantiate(_selectionMarkerPrefab);
            }
        }

        private void UpdateSelectionMarker()
        {
            if (_selectionMarkerInstance == null)
                return;

            if (_players == null || _players.Length == 0)
                return;

            Transform target = _players[_selectedTargetIndex].transform;
            _selectionMarkerInstance.transform.position = target.position + _selectionMarkerOffset;
        }

        private void HideSelectionMarker()
        {
            if (_selectionMarkerInstance != null)
            {
                Destroy(_selectionMarkerInstance);
                _selectionMarkerInstance = null;
            }
        }

        private void LateUpdate()
        {
            if (_isSelectingTarget)
            {
                UpdateSelectionMarker();
            }
        }

        protected override void OnDamageTaken()
        {
            Debug.Log($"{gameObject.name} took damage.");
        }

        protected override void OnDeath()
        {
            Debug.Log($"{gameObject.name} died.");
            ExitSelectionMode();
        }
    }
}