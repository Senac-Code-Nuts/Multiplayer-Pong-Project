using UnityEngine;
using Pong.Systems.Input;
using Pong.Systems;
using Pong.Systems.Graph;

namespace Pong.Gameplay.Player
{
    [RequireComponent(typeof(InputReader))]
    [RequireComponent(typeof(InfluenceSource))]
    public class PlayerController : MonoBehaviour
    {
        [SerializeField, Range(0f, 10f)] float _speed = 10f;
        private InputReader _inputReader;

        [Header("Movement")]
        private Vector2 _moveInput;
        [SerializeField] private PlayerSide _playerSide;

        private InfluenceSource _influenceSource;
        [SerializeField] private InfluenceSystem _influenceSystem;

        private Rigidbody _rigidbody;

        private void Awake()
        {
            _inputReader = GetComponent<InputReader>();
            _influenceSource = GetComponent<InfluenceSource>();
            _influenceSystem = FindFirstObjectByType<InfluenceSystem>();
            _rigidbody = GetComponent<Rigidbody>();
        }
        private void Start()
        {
            if (_influenceSystem != null)
            {
                _influenceSystem.RegisterSource(_influenceSource);
            }
        }

        private void OnEnable()
        {
            _inputReader.MoveEvent += HandleMovement;
            _inputReader.PauseEvent += OpenPauseMenu;
        }

        private void OnDisable()
        {
            _inputReader.MoveEvent -= HandleMovement;
            _inputReader.PauseEvent -= OpenPauseMenu;
        }
        public void SetPlayerSide(PlayerSide side)
        {
            _playerSide = side;
        }
        private void HandleMovement(Vector2 movement)
        {
            if (_playerSide == PlayerSide.Vertical)
            {
                _moveInput.y = movement.y;
            }
            else
            {
                _moveInput.x = movement.x;
            }
        }
        private void FixedUpdate()
        {
            Vector3 movement = new Vector3(_moveInput.x, 0, _moveInput.y);

            //transform.Translate(movement * _speed * Time.fixedDeltaTime);

           _rigidbody.MovePosition(_rigidbody.position + movement * _speed * Time.fixedDeltaTime);
        }

        public void ResetVelocity()
        {
            _rigidbody.linearVelocity = Vector3.zero;
        }

        private void OpenPauseMenu()
        {
            PauseMenuManager.Instance.TogglePauseMenu();
        }
    }
}
