namespace Pong.Player
{
    using UnityEngine;
    using UnityEngine.InputSystem;

    public class Player_Controller : MonoBehaviour
    {
        [SerializeField, Range(0f, 10f)] float _speed = 10f;
        private Vector2 _moveInput;

        public void OnMove(InputValue value)
        {
            _moveInput = value.Get<Vector2>();
        }

        private void FixedUpdate()
        {
            Vector3 movement = new Vector3(_moveInput.x, 0 , _moveInput.y);
            
            transform.Translate(movement * _speed * Time.deltaTime);
        }
    }

}
