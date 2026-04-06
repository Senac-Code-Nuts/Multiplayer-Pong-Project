using UnityEngine;

namespace Pong.Gameplay.Player
{
    public class SlothPlayer : PlayerActor
    {
        [Header("Sloth Ability")]
        [SerializeField] private PlayerActor[] _players;
        [SerializeField] private int _selectedTargetIndex = 0;
        [SerializeField] private bool _isSelectingTarget = false;

        public override void UseAbility()
        {
            if (_players == null || _players.Length == 0)
            {
                Debug.LogWarning("Sloth has no players assigned.");
                return;
            }

            _isSelectingTarget = !_isSelectingTarget;
            Debug.Log($"{gameObject.name} selection mode: {_isSelectingTarget}");
        }

        public void SelectLeftTarget()
        {
            if (!_isSelectingTarget || _players.Length == 0)
                return;

            _selectedTargetIndex--;
            if (_selectedTargetIndex < 0)
                _selectedTargetIndex = _players.Length - 1;

            Debug.Log($"{gameObject.name} selected {_players[_selectedTargetIndex].gameObject.name}");
        }

        public void SelectRightTarget()
        {
            if (!_isSelectingTarget || _players.Length == 0)
                return;

            _selectedTargetIndex++;
            if (_selectedTargetIndex >= _players.Length)
                _selectedTargetIndex = 0;

            Debug.Log($"{gameObject.name} selected {_players[_selectedTargetIndex].gameObject.name}");
        }

        public void ConfirmShield()
        {
            if (!_isSelectingTarget || _players.Length == 0)
                return;

            PlayerActor selectedPlayer = _players[_selectedTargetIndex];

            Debug.Log($"{gameObject.name} granted shield to {selectedPlayer.gameObject.name}");

            // selectedPlayer.ReceiveShield();
            _isSelectingTarget = false;
        }
    }
}