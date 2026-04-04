using System;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Pong.Systems
{
    public class InputReader : MonoBehaviour
    {
        public event Action<Vector2> MoveEvent;
        public event Action AttackEvent;

        private GameInput _gameInput;
        private void OnEnable()
        {
            _gameInput = new GameInput();

            _gameInput.Enable();
            
            _gameInput.PlayerMove.Move.performed += OnMove;
            _gameInput.PlayerMove.Move.canceled += OnMove;
            //_gameInput.Player.Attack.performed += OnAttack;
        }

        private void OnDisable()
        {
            _gameInput.PlayerMove.Move.performed -= OnMove;
            _gameInput.PlayerMove.Move.canceled -= OnMove;
            //_gameInput.Player.Attack.performed -= OnAttack;

            _gameInput.Disable();
        }
        private void OnMove(InputAction.CallbackContext context)
        {
            Vector2 movement = context.canceled ? Vector2.zero : context.ReadValue<Vector2>();
            MoveEvent?.Invoke(movement);
        }
        private void OnAttack(InputAction.CallbackContext context)
        {
            if (context.performed)
            {
                AttackEvent?.Invoke();
            }
        }
    }
}
