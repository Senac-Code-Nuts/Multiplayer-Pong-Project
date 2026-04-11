using System;
using UnityEngine;

namespace Pong.Core
{
    public static class PlayerSpawnEvents
    {
        public static event Action<GameObject, int> OnPlayerSpawned;

        public static void RaisePlayerSpawned(GameObject playerObject, int playerIndex)
        {
            OnPlayerSpawned?.Invoke(playerObject, playerIndex);
        }
    }
}