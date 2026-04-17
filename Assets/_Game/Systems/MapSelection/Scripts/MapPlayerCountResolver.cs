using Pong.Systems.Selection;
using UnityEngine;

namespace Pong.Systems.MapSelection
{
    public class MapPlayerCountResolver : MonoBehaviour
    {
        [SerializeField] private CharacterSelectionSession _selectionSession;

        public int GetRegisteredPlayerCount()
        {
            if (_selectionSession == null)
                return 0;

            int registeredPlayers = 0;

            for (int i = 0; i < _selectionSession.PlayerSelections.Length; i++)
            {
                if (_selectionSession.PlayerSelections[i].IsRegistered)
                    registeredPlayers++;
            }

            return registeredPlayers;
        }
    }
}