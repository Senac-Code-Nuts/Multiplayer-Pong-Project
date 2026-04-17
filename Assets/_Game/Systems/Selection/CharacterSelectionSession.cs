using System;
using UnityEngine;

namespace Pong.Systems.Selection
{
    [CreateAssetMenu(menuName = "Pong/Selection/Character Selection Session")]
    public class CharacterSelectionSession : ScriptableObject
    {
        [SerializeField] private PlayerSelectionEntry[] _playerSelections = new PlayerSelectionEntry[4];

        public PlayerSelectionEntry[] PlayerSelections => _playerSelections;

        public bool HasAnyRegisteredPlayers
        {
            get
            {
                for (int i = 0; i < _playerSelections.Length; i++)
                {
                    if (_playerSelections[i].IsRegistered)
                        return true;
                }

                return false;
            }
        }

        public void ResetSession()
        {
            for (int i = 0; i < _playerSelections.Length; i++)
            {
                _playerSelections[i].IsRegistered = false;
                _playerSelections[i].IsConfirmed = false;
                _playerSelections[i].SelectedCharacter = CharacterType.None;
            }
        }

        public void RegisterPlayer(int playerIndex, CharacterType defaultCharacter)
        {
            if (!IsValidPlayerIndex(playerIndex)) return;

            _playerSelections[playerIndex].IsRegistered = true;

            if (_playerSelections[playerIndex].SelectedCharacter == CharacterType.None)
            {
                _playerSelections[playerIndex].SelectedCharacter = defaultCharacter;
            }
        }

        public void SetCharacter(int playerIndex, CharacterType characterType)
        {
            if (!IsValidPlayerIndex(playerIndex)) return;

            _playerSelections[playerIndex].SelectedCharacter = characterType;
        }

        public void SetConfirmed(int playerIndex, bool isConfirmed)
        {
            if (!IsValidPlayerIndex(playerIndex)) return;

            _playerSelections[playerIndex].IsConfirmed = isConfirmed;
        }

        public bool TryGetSelectedCharacter(int playerIndex, out CharacterType characterType)
        {
            characterType = CharacterType.None;

            if (!IsValidPlayerIndex(playerIndex)) return false;
            if (!_playerSelections[playerIndex].IsRegistered) return false;

            characterType = _playerSelections[playerIndex].SelectedCharacter;
            return characterType != CharacterType.None;
        }

        public bool IsPlayerConfirmed(int playerIndex)
        {
            if (!IsValidPlayerIndex(playerIndex)) return false;

            return _playerSelections[playerIndex].IsRegistered && _playerSelections[playerIndex].IsConfirmed;
        }

        private bool IsValidPlayerIndex(int playerIndex)
        {
            return playerIndex >= 0 && playerIndex < _playerSelections.Length;
        }
    }

    [Serializable]
    public struct PlayerSelectionEntry
    {
        public bool IsRegistered;
        public bool IsConfirmed;
        public CharacterType SelectedCharacter;
    }
}