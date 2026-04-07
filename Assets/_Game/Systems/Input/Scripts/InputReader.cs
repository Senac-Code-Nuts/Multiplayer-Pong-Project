using System;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Pong.Systems.Input
{
    public class InputReader : MonoBehaviour
    {
        public event Action<Vector2> MoveEvent;
        public event Action CastEvent;

        private PlayerInput _playerInput;
        private InputAction _moveAction;
        private InputAction _castAction;

        private void Awake()
        {
            _playerInput = GetComponent<PlayerInput>();
            _moveAction = _playerInput.actions["Move"];
            _castAction = _playerInput.actions["Cast"];
        }

        private void OnEnable()
        {
            _moveAction.Enable();
            _castAction.Enable();

            _moveAction.performed += OnMove;
            _castAction.performed += OnCast;
        }

        private void OnDisable()
        {
            _moveAction.Disable();
            _castAction.Disable();

            _moveAction.performed -= OnMove;
            _castAction.performed -= OnCast;
        }
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
    }
}
