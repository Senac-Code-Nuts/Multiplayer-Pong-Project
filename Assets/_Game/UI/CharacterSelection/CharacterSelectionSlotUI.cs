using Pong.Systems.Selection;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

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

        [Header("Image")]
        [SerializeField] private Image _characterImage;

        [System.Serializable]
        public struct CharacterSpriteEntry {
            public CharacterType CharacterType;
            public Sprite Sprite;
        }

        [SerializeField] private CharacterSpriteEntry[] _characterSprites;

        private void Update()
        {
            Refresh();
        }

        private Sprite GetSprite(CharacterType type) {
            for (int i = 0; i < _characterSprites.Length; i++) {
                if (_characterSprites[i].CharacterType == type)
                    return _characterSprites[i].Sprite;
            }

            return null;
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

            if (_characterImage != null) {
                Sprite sprite = GetSprite(selectedCharacter);
                _characterImage.sprite = sprite;
                _characterImage.enabled = sprite != null;
            }
        }
    }
}