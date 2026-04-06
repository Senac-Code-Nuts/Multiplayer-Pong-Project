using UnityEngine;
using Pong.Systems.Input;

namespace Pong.Gameplay.Player
{
    [RequireComponent(typeof(InputReader))]
    public class PlayerController : MonoBehaviour
    {
        [SerializeField, Range(0f, 10f)] float _speed = 10f;
        private InputReader _inputReader;

        [Header("Movement")]
        private Vector2 _moveInput;
        private enum PlayerType { Vertical, Horizontal }
        [SerializeField] private PlayerType _playerType;

        private void Awake()
        {
            _inputReader = GetComponent<InputReader>();
        }

        private void OnEnable()
        {
            _inputReader.MoveEvent += HandleMovement;
        }

        private void OnDisable()
        {
            _inputReader.MoveEvent -= HandleMovement;
        }
        private void HandleMovement(Vector2 movement)
        {
            if (_playerType == PlayerType.Vertical)
            {
                _moveInput = new Vector2(0, movement.y);
            }
            else
            {
                _moveInput = new Vector2(movement.x, 0);
            }
        }
        private void FixedUpdate()
        {
            Vector3 movement = new Vector3(_moveInput.x, 0, _moveInput.y);

            transform.Translate(movement * _speed * Time.fixedDeltaTime);
        }
    }

}
