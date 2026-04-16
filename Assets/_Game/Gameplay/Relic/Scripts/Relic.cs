using System.Threading.Tasks;
using Pong.Gameplay.Actors;
using Pong.Gameplay.Enemy;
using Pong.Gameplay.Player;
using Pong.Systems.Audio;
using UnityEngine;

namespace Pong.Gameplay.Relics
{
    [RequireComponent(typeof(Rigidbody))]
    public class Relic : MonoBehaviour
    {
        [Header("Speed")]
        [SerializeField, Range(0f, 25f)] private float _speed;

        [Header("Visual")]
        private Renderer _renderer;
        [SerializeField] private Color _boostColor = Color.yellow;

        [Header("Damage")]
        [SerializeField] private int _damage;

        [Header("Audio Settings")]
        [SerializeField] private AudioClip _bumpClip;

        private Vector3 _direction;
        private Rigidbody _rigidBody;
        private float _currentSpeed;
        private Material _material;
        private Color _defaultColor;

        private Vector3 _originalScale;
        private bool _isRevealing;

        private void Awake()
        {
            _rigidBody = GetComponent<Rigidbody>();
            _originalScale = transform.localScale;
            transform.localScale = Vector3.zero;
            gameObject.SetActive(false);

            if (_renderer == null)
            {
                _renderer = GetComponent<Renderer>();
            }

            if (_renderer != null)
            {
                _material = _renderer.material;
                _defaultColor = _material.color;
            }

            _currentSpeed = _speed;
        }

        private void Start()
        {
            _rigidBody.linearVelocity = Vector3.zero;
            UpdateVisual(false);
        }

        private void OnEnable()
        {
            if (_isRevealing)
            {
                return;
            }

            Launch();
        }

        public async Task AnimateAppearanceAsync(float duration = 1.5f)
        {
            if (duration <= 0f)
            {
                transform.localScale = _originalScale;
                gameObject.SetActive(true);
                Launch();
                return;
            }

            _isRevealing = true;
            _rigidBody.linearVelocity = Vector3.zero;
            _rigidBody.angularVelocity = Vector3.zero;
            _rigidBody.isKinematic = true;

            transform.localScale = Vector3.zero;
            gameObject.SetActive(true);

            float elapsedTime = 0f;

            while (elapsedTime < duration)
            {
                float normalizedTime = Mathf.Clamp01(elapsedTime / duration);
                float smoothTime = Mathf.SmoothStep(0f, 1f, normalizedTime);

                transform.localScale = Vector3.LerpUnclamped(Vector3.zero, _originalScale, smoothTime);
                transform.Rotate(0f, 360f * Time.deltaTime / duration, 0f, Space.Self);

                elapsedTime += Time.deltaTime;
                await Task.Yield();
            }

            transform.localScale = _originalScale;
            _rigidBody.isKinematic = false;
            _isRevealing = false;
            Launch();
        }

        private void Launch()
        {
            float dirX = Random.Range(-1f, 1f);
            float dirZ = Random.Range(-1f, 1f);

            _direction = new Vector3(dirX, 0, dirZ).normalized;
            _currentSpeed = _speed;
            _rigidBody.linearVelocity = _direction * _currentSpeed;
            UpdateVisual(false);
        }

        private void FixedUpdate()
        {
            _rigidBody.linearVelocity = _rigidBody.linearVelocity.normalized * _currentSpeed;
        }

        private void OnCollisionEnter(Collision collision)
        {
            if(_bumpClip != null)
            {
                AudioManager.Instance.PlaySFX(_bumpClip);
            }
            TryApplyDamage(collision);

            if (collision.collider.TryGetComponent(out PlayerController player)) {
                player.ResetVelocity();
            }

            if (collision.collider.TryGetComponent(out EnemyActor actor) || collision.collider.GetComponentInParent<EnemyActor>() != null)
            {
                Reflect(collision);
                return;
            }

            if (collision.gameObject.TryGetComponent(out MinotaurEnemy minotaur) && minotaur.ConsumeCounterAttackTriggered())
            {
                Debug.Log("<color=cyan>[Relic] Minotaur parry collision detected. Speed boost applied on hit.</color>");
                return;
            }

            Reflect(collision);
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
            InvertDirection(_direction);
        }

        public void InvertDirection(Vector3 attackAxis)
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
            _currentSpeed = _speed;
            _rigidBody.linearVelocity = _direction * _currentSpeed;
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