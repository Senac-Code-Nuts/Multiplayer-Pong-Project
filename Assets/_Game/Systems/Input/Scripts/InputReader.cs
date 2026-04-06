using System;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Pong.Systems.Input
{
    [CreateAssetMenu(menuName = "Systems/Input Reader")]
    public class InputReader : ScriptableObject, GameInput.IPlayerMoveActions
    {
        public event Action<Vector2> MoveEvent;
        public event Action AttackEvent;

        private GameInput _gameInput;

        private void OnEnable()
        {
            if (_gameInput == null)
            {
                _gameInput = new GameInput();
                _gameInput.PlayerMove.SetCallbacks(this);
            }
            _gameInput.PlayerMove.Enable();
        }

        private void OnDisable()
        {
            if (_gameInput != null)
            {
                _gameInput.PlayerMove.Disable();
            }
        }

        public void OnMove(InputAction.CallbackContext context)
        {
            MoveEvent?.Invoke(context.ReadValue<Vector2>());
        }

        public void OnAttack(InputAction.CallbackContext context)
        {
            if (context.performed)
            {
                AttackEvent?.Invoke();
            }
        }
    }
}
