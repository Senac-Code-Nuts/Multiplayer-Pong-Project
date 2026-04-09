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
        private int _shieldCount = 1;

        [Header("Selection")]
        [SerializeField, Range(0.05f, 1f)] private float _selectionInputCooldown = 0.2f;
        [SerializeField] private TMP_Text _selectionText;

        [Header("Debug")]
        [SerializeField] private bool _useDebug;

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

        private void Update()
        {
            if(Input.GetKeyDown(KeyCode.F1) && _useDebug)
            {
                LevelUp();
            }

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
                return;
            }

            _isSelectingTarget = true;
            _canChangeSelection = true;

            if (_selectedTargetIndex < 0 || _selectedTargetIndex >= _players.Length)
            {
                _selectedTargetIndex = 0;
            }

            UpdateSelectionVisual();
        }

        private void ExitSelectionMode()
        {
            _isSelectingTarget = false;
            UpdateSelectionVisual();

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
        }

        private void SelectRightTarget()
        {
            _selectedTargetIndex++;

            if (_selectedTargetIndex >= _players.Length)
            {
                _selectedTargetIndex = 0;
            }

            UpdateSelectionVisual();
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
            
        }

        protected override void OnDeath()
        {
            ExitSelectionMode();
        }
    }
}