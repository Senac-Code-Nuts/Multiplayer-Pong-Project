using System.Collections;
using TMPro;
using UnityEngine;
using Pong.Gameplay.Player.Lust;

namespace Pong.Gameplay.Player
{
    public class SlothPlayer : PlayerActor
    {
        [Header("Ability")]
        [SerializeField] private PlayerActor[] _players;
        [SerializeField] private int _selectedTargetIndex = 0;
        [SerializeField] private bool _isSelectingTarget = false;

        [Header("Shield Settings")]
        [SerializeField] private GameObject _shieldVisualPrefab;
        [SerializeField] private int _shieldCount = 1;

        [Header("Selection")]
        [SerializeField, Range(0.05f, 1f)] private float _selectionInputCooldown = 0.2f;
        [SerializeField] private TMP_Text _selectionText;

        private bool _canChangeSelection = true;

        protected override void Awake()
        {
            base.Awake();

            if (_selectionText != null)
            {
                _selectionText.gameObject.SetActive(false);
            }
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

        protected override void OnEnable()
        {
            base.OnEnable();

            if (_inputReader != null)
            {
                _inputReader.MoveEvent += HandleSelectionMove;
            }

            if (_selectionText != null)
            {
                _selectionText.gameObject.SetActive(false);
            }
        }

        protected override void OnDisable()
        {
            base.OnDisable();

            if (_inputReader != null)
            {
                _inputReader.MoveEvent -= HandleSelectionMove;
            }
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

            if (_selectedTargetIndex < 0 || _selectedTargetIndex >= _players.Length)
            {
                _selectedTargetIndex = 0;
            }

            UpdateSelectionVisual();

            Debug.Log($"{gameObject.name} entered selection mode.");
            Debug.Log($"{gameObject.name} current target: {_players[_selectedTargetIndex].gameObject.name}");
        }

        private void ExitSelectionMode()
        {
            _isSelectingTarget = false;
            UpdateSelectionVisual();

            Debug.Log($"{gameObject.name} exited selection mode.");
        }

        private void HandleSelectionMove(Vector2 input)
        {
            if (!_isSelectingTarget || !_canChangeSelection) return;

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
            {
                _selectedTargetIndex = _players.Length - 1;
            }

            UpdateSelectionVisual();
            Debug.Log($"{gameObject.name} selected {_players[_selectedTargetIndex].gameObject.name}");
        }

        private void SelectRightTarget()
        {
            _selectedTargetIndex++;

            if (_selectedTargetIndex >= _players.Length)
            {
                _selectedTargetIndex = 0;
            }

            UpdateSelectionVisual();
            Debug.Log($"{gameObject.name} selected {_players[_selectedTargetIndex].gameObject.name}");
        }

        private void ConfirmShield()
        {
            if (_players == null || _players.Length == 0) return;
            int shieldsToAplly = _shieldCount;

            for(int i = 0; i < shieldsToAplly; i++)
            {
                int index = (_selectedTargetIndex + i) % _players.Length;
                PlayerActor target = _players[index];

                target.ReceiveShield(_shieldVisualPrefab);
                Debug.Log($"{gameObject.name} granted shield to {target.gameObject.name}");
            }
            
            ExitSelectionMode();
            StartCoroutine(AbilityCooldownRoutine());
        }

        private void UpdateSelectionVisual()
        {
            if (_selectionText == null) return;

            _selectionText.gameObject.SetActive(_isSelectingTarget);

            if (!_isSelectingTarget || _players == null || _players.Length == 0) return;

            _selectionText.text = GetTargetLabel(_players[_selectedTargetIndex]);
        }

        private string GetTargetLabel(PlayerActor targetPlayer)
        {
            if (targetPlayer == null) return "NONE";

            if (targetPlayer == this) return "ME";
            if (targetPlayer is LustPlayer) return "LUST";
            if (targetPlayer is ViolencePlayer) return "VIOLENCE";
            if (targetPlayer is FraudPlayer) return "FRAUD";
            if (targetPlayer is SlothPlayer) return "ME";

            return targetPlayer.gameObject.name.ToUpper();
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