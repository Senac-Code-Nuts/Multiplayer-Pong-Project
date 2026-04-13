using UnityEngine;
using Pong.Gameplay.Actors;
using Pong.Gameplay.Enemy;

namespace Pong.Gameplay.Relics
{
    [RequireComponent(typeof(Rigidbody))]
    public class Relic : MonoBehaviour
    {
        [Header("Speed")]
        [SerializeField, Range(0f, 10f)] private float _speed;
        [SerializeField, Range(1f, 4f)] private float _attackBoostMultiplier = 1.5f;

        [Header("Visual")]
        [SerializeField] private Renderer _renderer;
        [SerializeField] private Color _boostColor = Color.yellow;

        [Header("Damage")]
        [SerializeField] private int _damage;

        private Vector3 _direction;
        private Rigidbody _rigidBody;
        private float _baseSpeed;
        private float _currentSpeed;
        private bool _returnToBaseSpeedOnNextCollision;
        private Material _material;
        private Color _defaultColor;

        private void Awake()
        {
            _rigidBody = GetComponent<Rigidbody>();
            if (_renderer == null)
            {
                _renderer = GetComponent<Renderer>();
            }

            if (_renderer != null)
            {
                _material = _renderer.material;
                _defaultColor = _material.color;
            }

            _baseSpeed = _speed;
            _currentSpeed = _speed;
        }

        private void Start()
        {
            Launch();
        }

        private void Launch()
        {
            float dirX = Random.Range(-1f, 1f);
            float dirZ = Random.Range(-1f, 1f);

            _direction = new Vector3(dirX, 0, dirZ).normalized;
            _currentSpeed = _baseSpeed;
            _rigidBody.linearVelocity = _direction * _currentSpeed;
            UpdateVisual(false);
        }

        private void FixedUpdate()
        {
            _rigidBody.linearVelocity = _rigidBody.linearVelocity.normalized * _currentSpeed;
        }

        private void OnCollisionEnter(Collision collision)
        {
            TryApplyDamage(collision);

            if (collision.gameObject.TryGetComponent(out MinotaurEnemy minotaur) && minotaur.ConsumeCounterAttackTriggered())
            {
                Debug.Log("<color=cyan>[Relic] Minotaur parry collision detected. Speed boost applied on hit.</color>");
                return;
            }

            Reflect(collision);

            if (_returnToBaseSpeedOnNextCollision)
            {
                RestoreBaseSpeed();
            }
        }

        private void Reflect(Collision collision)
        {
            Vector3 normal = collision.contacts[0].normal;
            _direction = Vector3.Reflect(_direction, normal);
            _rigidBody.linearVelocity = _direction.normalized * _currentSpeed;
            _rigidBody.linearVelocity = _rigidBody.linearVelocity.normalized * _currentSpeed;
        }

        private void TryApplyDamage(Collision collision)
        {
            if (collision.gameObject.TryGetComponent(out IDamageable damageable))
            {
                damageable.ApplyDamage(_damage);
            }
        }

        public void InvertDirection()
        {
            InvertDirection(_direction, false, _attackBoostMultiplier);
        }

        public void InvertDirection(Vector3 attackAxis, bool shouldBoost = false, float speedMultiplier = 1.5f)
        {
            if (attackAxis.sqrMagnitude < 0.0001f)
            {
                attackAxis = _direction;
            }

            attackAxis.y = 0f;

            if (Mathf.Abs(attackAxis.x) >= Mathf.Abs(attackAxis.z))
            {
                _direction.x *= -1f;
            }
            else
            {
                _direction.z *= -1f;
            }

            _direction = _direction.normalized;
            _currentSpeed = shouldBoost ? _baseSpeed * speedMultiplier : _baseSpeed;
            _rigidBody.linearVelocity = _direction * _currentSpeed;
            _returnToBaseSpeedOnNextCollision = shouldBoost;
            UpdateVisual(shouldBoost);
        }

        private void RestoreBaseSpeed()
        {
            _currentSpeed = _baseSpeed;
            _rigidBody.linearVelocity = _direction * _currentSpeed;
            _returnToBaseSpeedOnNextCollision = false;
            Debug.Log("<color=gray>[Relic] Returned to base speed.</color>");
            UpdateVisual(false);
        }

        private void UpdateVisual(bool isBoosted)
        {
            if (_material == null)
            {
                return;
            }

            _material.color = isBoosted ? _boostColor : _defaultColor;
        }
    }
}