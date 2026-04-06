using UnityEngine;
using UnityEngine.InputSystem;

namespace Pong.Systems.Input
{
    internal class PlayerData
    {
        public GameObject Instance;
        public InputDevice Device;
        public PlayerSide Side;
    }
}