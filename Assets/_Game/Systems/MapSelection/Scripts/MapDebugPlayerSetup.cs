using Pong.Systems.Selection;
using UnityEngine;

namespace Pong.Systems.MapSelection
{
    public class MapDebugPlayerSetup : MonoBehaviour
    {
        [SerializeField] private CharacterSelectionSession _selectionSession;
        [SerializeField, Range(0, 4)] private int _debugPlayerCount = 2;
        [SerializeField] private bool _applyOnStart = true;

        private void Start()
        {
            if (!_applyOnStart)
                return;

            ApplyDebugPlayers();
        }

        [ContextMenu("Apply Debug Players")]
        public void ApplyDebugPlayers()
        {
            if (_selectionSession == null)
            {
                Debug.LogWarning("CharacterSelectionSession não foi configurada.");
                return;
            }

            _selectionSession.ResetSession();

            for (int i = 0; i < _debugPlayerCount; i++)
            {
                CharacterType defaultCharacter = GetDefaultCharacter(i);

                _selectionSession.RegisterPlayer(i, defaultCharacter);
                _selectionSession.SetCharacter(i, defaultCharacter);
                _selectionSession.SetConfirmed(i, true);
            }

            Debug.Log($"[MapDebugPlayerSetup] Jogadores de teste aplicados: {_debugPlayerCount}");
        }

        private CharacterType GetDefaultCharacter(int playerIndex)
        {
            return playerIndex switch
            {
                0 => CharacterType.Sloth,
                1 => CharacterType.Lust,
                2 => CharacterType.Fraud,
                3 => CharacterType.Violence,
                _ => CharacterType.None
            };
        }
    }
}