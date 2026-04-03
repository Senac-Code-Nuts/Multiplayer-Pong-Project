using UnityEngine;
using Pong.Systems;

namespace Pong.Gameplay.Player
{
    public class PlayerController : MonoBehaviour
    {
        [SerializeField, Range(0f, 10f)] float _speed = 10f;
        private Vector2 _moveInput;
        [SerializeField] private InputReader _inputReader;

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
            _moveInput = movement;
        }
        private void FixedUpdate()
        {
            Vector3 movement = new Vector3(_moveInput.x, 0 , _moveInput.y);
            
            transform.Translate(movement * _speed * Time.deltaTime);
        }
    }

}
