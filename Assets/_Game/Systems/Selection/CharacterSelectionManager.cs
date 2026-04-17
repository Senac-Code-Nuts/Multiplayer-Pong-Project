using System.Collections.Generic;
using UnityEngine;

namespace Pong.Systems.Selection
{
    public class CharacterSelectionManager : MonoBehaviour
    {
        [SerializeField] private CharacterSelectionSession _selectionSession;
        [SerializeField]
        private List<CharacterType> _availableCharacters = new List<CharacterType>()
        {
            CharacterType.Sloth,
            CharacterType.Lust,
            CharacterType.Fraud,
            CharacterType.Violence
        };

        [SerializeField, Range(2, 4)] private int _requiredPlayers = 2;
        [SerializeField] private bool _allowDuplicateCharacters = false;

        private readonly Dictionary<int, int> _currentIndices = new();

        public CharacterSelectionSession SelectionSession => _selectionSession;
        public IReadOnlyList<CharacterType> AvailableCharacters => _availableCharacters;

        public void ClearSelections()
        {
            _currentIndices.Clear();

            if (_selectionSession != null)
            {
                _selectionSession.ResetSession();
            }
        }

        public void RegisterPlayer(int playerIndex)
        {
            if (_selectionSession == null) return;
            if (_currentIndices.ContainsKey(playerIndex)) return;
            if (_availableCharacters.Count == 0) return;

            int defaultIndex = Mathf.Clamp(playerIndex, 0, _availableCharacters.Count - 1);

            _currentIndices.Add(playerIndex, defaultIndex);
            _selectionSession.RegisterPlayer(playerIndex, _availableCharacters[defaultIndex]);
        }

        public bool IsPlayerRegistered(int playerIndex)
        {
            return _currentIndices.ContainsKey(playerIndex);
        }

        public CharacterType GetSelectedCharacter(int playerIndex)
        {
            if (_selectionSession == null) return CharacterType.None;

            if (_selectionSession.TryGetSelectedCharacter(playerIndex, out CharacterType characterType))
                return characterType;

            return CharacterType.None;
        }

        public void ChangeSelection(int playerIndex, int direction)
        {
            if (_selectionSession == null) return;
            if (!_currentIndices.TryGetValue(playerIndex, out int currentIndex)) return;
            if (_selectionSession.IsPlayerConfirmed(playerIndex)) return;
            if (_availableCharacters.Count == 0) return;

            int nextIndex = (currentIndex + direction + _availableCharacters.Count) % _availableCharacters.Count;

            if (_allowDuplicateCharacters)
            {
                _currentIndices[playerIndex] = nextIndex;
                _selectionSession.SetCharacter(playerIndex, _availableCharacters[nextIndex]);
                return;
            }

            int startIndex = nextIndex;

            while (IsCharacterTakenByOtherConfirmedPlayer(playerIndex, _availableCharacters[nextIndex]))
            {
                nextIndex = (nextIndex + direction + _availableCharacters.Count) % _availableCharacters.Count;

                if (nextIndex == startIndex)
                    return;
            }

            _currentIndices[playerIndex] = nextIndex;
            _selectionSession.SetCharacter(playerIndex, _availableCharacters[nextIndex]);
        }

        public bool ConfirmSelection(int playerIndex)
        {
            if (_selectionSession == null) return false;
            if (!_currentIndices.ContainsKey(playerIndex)) return false;

            CharacterType selectedCharacter = GetSelectedCharacter(playerIndex);

            if (!_allowDuplicateCharacters && IsCharacterTakenByOtherConfirmedPlayer(playerIndex, selectedCharacter))
                return false;

            _selectionSession.SetConfirmed(playerIndex, true);
            return true;
        }

        public void CancelSelection(int playerIndex)
        {
            if (_selectionSession == null) return;

            _selectionSession.SetConfirmed(playerIndex, false);
        }

        public bool CanStartMatch()
        {
            if (_selectionSession == null) return false;

            int registeredPlayers = 0;
            int confirmedPlayers = 0;

            for (int i = 0; i < _selectionSession.PlayerSelections.Length; i++)
            {
                if (_selectionSession.PlayerSelections[i].IsRegistered)
                    registeredPlayers++;

                if (_selectionSession.PlayerSelections[i].IsRegistered &&
                    _selectionSession.PlayerSelections[i].IsConfirmed)
                {
                    confirmedPlayers++;
                }
            }

            Debug.Log($"[Selection] Registered: {registeredPlayers} | Confirmed: {confirmedPlayers}");

            bool isValidPlayerCount = registeredPlayers == 2 || registeredPlayers == 4;
            if (!isValidPlayerCount) return false;

            return confirmedPlayers == registeredPlayers;
        }

        private bool IsCharacterTakenByOtherConfirmedPlayer(int playerIndex, CharacterType characterType)
        {
            if (_selectionSession == null) return false;

            for (int i = 0; i < _selectionSession.PlayerSelections.Length; i++)
            {
                if (i == playerIndex) continue;
                if (!_selectionSession.PlayerSelections[i].IsRegistered) continue;
                if (!_selectionSession.PlayerSelections[i].IsConfirmed) continue;
                if (_selectionSession.PlayerSelections[i].SelectedCharacter == characterType)
                    return true;
            }

            return false;
        }

        public bool IsPlayerConfirmed(int playerIndex)
        {
            if (_selectionSession == null) return false;

            return _selectionSession.IsPlayerConfirmed(playerIndex);
        }
    }
}