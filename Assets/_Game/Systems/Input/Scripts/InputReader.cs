using System;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Pong.Systems.Input
{
    [RequireComponent(typeof(PlayerInput))]
    public class InputReader : MonoBehaviour
    {
        public event Action<Vector2> MoveEvent;
        public event Action CastEvent;
        public event Action PauseEvent;

        public void OnMove(InputAction.CallbackContext context)
        {
            MoveEvent?.Invoke(context.ReadValue<Vector2>());
        }

        public void OnCast(InputAction.CallbackContext context)
        {
            if (context.performed)
            {
                CastEvent?.Invoke();
            }
        }

        public void OnPause(InputAction.CallbackContext context)
        {
            if (context.performed)
            {
                PauseEvent?.Invoke();
            }
        }
    }
}
