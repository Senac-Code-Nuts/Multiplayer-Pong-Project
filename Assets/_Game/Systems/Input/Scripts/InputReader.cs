using System;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Pong.Systems.Input
{
    public class InputReader : MonoBehaviour
    {
        public event Action<Vector2> MoveEvent;
        public event Action CastEvent;
        public event Action PauseEvent;

        private PlayerInput _playerInput;
        private InputAction _moveAction;
        private InputAction _castAction;
        private InputAction _pauseAction;

        private void Awake()
        {
            _playerInput = GetComponent<PlayerInput>();
            _moveAction = _playerInput.actions["Move"];
            _castAction = _playerInput.actions["Cast"];
            _pauseAction = _playerInput.actions["Pause"];
        }

        private void OnEnable()
        {
            _moveAction.Enable();
            _castAction.Enable();
            _pauseAction.Enable();

            _moveAction.performed += OnMove;
            _castAction.performed += OnCast;
            _pauseAction.performed += OnPause;
        }

        private void OnDisable()
        {
            _moveAction.Disable();
            _castAction.Disable();
            _pauseAction.Disable();

            _moveAction.performed -= OnMove;
            _castAction.performed -= OnCast;
            _pauseAction.performed -= OnPause;
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
        public void OnPause(InputAction.CallbackContext context)
        {
            if (context.performed)
            {
                PauseEvent?.Invoke();
            }
        }
    }
}
