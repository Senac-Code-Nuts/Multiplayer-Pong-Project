using Pong.Systems.Selection;
using TMPro;
using UnityEngine;

namespace Pong.UI.CharacterSelection
{
    public class CharacterSelectionSlotUI : MonoBehaviour
    {
        [SerializeField] private int _playerIndex;
        [SerializeField] private CharacterSelectionManager _selectionManager;

        [Header("Texts")]
        [SerializeField] private TMP_Text _playerLabelText;
        [SerializeField] private TMP_Text _characterNameText;
        [SerializeField] private TMP_Text _statusText;

        private void Update()
        {
            Refresh();
        }

        private void Refresh()
        {
            if (_playerLabelText != null)
            {
                _playerLabelText.text = $"Player {_playerIndex + 1}";
            }

            if (_selectionManager == null) return;

            if (!_selectionManager.IsPlayerRegistered(_playerIndex))
            {
                if (_characterNameText != null)
                {
                    _characterNameText.text = "---";
                }

                if (_statusText != null)
                {
                    _statusText.text = "Aguardando";
                }

                return;
            }

            CharacterType selectedCharacter = _selectionManager.GetSelectedCharacter(_playerIndex);

            if (_characterNameText != null)
            {
                _characterNameText.text = selectedCharacter.ToString();
            }

            if (_statusText != null)
            {
                _statusText.text = _selectionManager.IsPlayerConfirmed(_playerIndex)
                    ? "Confirmado"
                    : "Selecionando";
            }
        }
    }
}