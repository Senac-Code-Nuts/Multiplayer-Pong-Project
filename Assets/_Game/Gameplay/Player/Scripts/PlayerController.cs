using UnityEngine;
using Pong.Systems.Input;
using Pong.Systems;
using Pong.Systems.Graph;

namespace Pong.Gameplay.Player
{
    [RequireComponent(typeof(InputReader))]
    [RequireComponent(typeof(InfluenceSource))]
    [RequireComponent(typeof(Rigidbody))]
    public class PlayerController : MonoBehaviour
    {
        [SerializeField, Range(0f, 10f)] float _speed = 10f;
        private InputReader _inputReader;

        [Header("Movement")]
        private Vector2 _moveInput;
        [SerializeField] private PlayerSide _playerSide;

        private InfluenceSource _influenceSource;
        [SerializeField] private InfluenceSystem _influenceSystem;
        private Rigidbody _rigidBody;

        private void Awake()
        {
            _inputReader = GetComponent<InputReader>();
            _influenceSource = GetComponent<InfluenceSource>();
            _influenceSystem = FindFirstObjectByType<InfluenceSystem>();
            _rigidBody = GetComponent<Rigidbody>();
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
            if (_playerSide == PlayerSide.West || _playerSide == PlayerSide.East)
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

            _rigidBody.MovePosition(_rigidBody.position + movement * _speed * Time.fixedDeltaTime);
        }
        private void OpenPauseMenu()
        {
            PauseMenuManager.Instance.TogglePauseMenu();
        }
    }
}
