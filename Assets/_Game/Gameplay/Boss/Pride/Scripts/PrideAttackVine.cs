using Pong.Framework.ObjectPool;
using Pong.Gameplay.Player;
using UnityEngine;

namespace Pong.Gameplay.Boss
{
    [RequireComponent(typeof(Rigidbody))]
    [RequireComponent(typeof(Collider))]
    public class PrideAttackVine : MonoBehaviour, IPoolable
    {
        private const string WALL_TAG = "Wall";
        private const string RELIC_TAG = "Relic";

        private Rigidbody _rigidBody;
        private PrideAttackVinePool _pool;

        private Vector3 _originalScale;

        private Vector3 _direction;
        private float _speed;
        private int _damage;

        private float _lifetime;
        private float _timer;

        private bool _hasHitPlayer;
        private Transform _owner;

        private void Awake()
        {
            _rigidBody = GetComponent<Rigidbody>();
            _originalScale = transform.localScale;
        }

        private void FixedUpdate()
        {
            _rigidBody.linearVelocity = _direction.normalized * _speed;
            _rigidBody.angularVelocity = Vector3.zero;
        }

        private void Update()
        {
            _timer += Time.deltaTime;

            if (_timer >= _lifetime)
            {
                ReturnToPool();
            }
        }

        public void Initialize(
            Vector3 direction,
            float speed,
            int damage,
            float lifetime,
            PrideAttackVinePool pool,
            Transform owner
        )
        {
            if (direction.sqrMagnitude <= 0.0001f)
            {
                direction = transform.forward;
            }

            if (direction.sqrMagnitude <= 0.0001f)
            {
                direction = Vector3.forward;
            }

            _direction = direction.normalized;
            _speed = speed;
            _damage = damage;
            _lifetime = lifetime;

            _pool = pool;
            _owner = owner;

            _timer = 0f;
            _hasHitPlayer = false;

            transform.localScale = _originalScale;

            _rigidBody.linearVelocity = _direction * _speed;
            _rigidBody.angularVelocity = Vector3.zero;

            transform.rotation = Quaternion.LookRotation(_direction, Vector3.up);
        }

        private void OnCollisionEnter(Collision collision)
        {
            if (_owner != null)
            {
                if (collision.transform == _owner)
                    return;

                if (collision.transform.IsChildOf(_owner))
                    return;
            }

            if (collision.gameObject.CompareTag(WALL_TAG))
            {
                ReturnToPool();
                return;
            }

            if (collision.gameObject.CompareTag(RELIC_TAG))
            {
                Reflect(collision);
            }
        }

        private void OnTriggerEnter(Collider other)
        {
            if (_hasHitPlayer)
                return;

            if (_owner != null)
            {
                if (other.transform == _owner)
                    return;

                if (other.transform.IsChildOf(_owner))
                    return;
            }

            if (other.TryGetComponent<PlayerActor>(out PlayerActor player))
            {
                _hasHitPlayer = true;
                player.ApplyDamage(_damage);
                ReturnToPool();
            }
        }

        private void Reflect(Collision collision)
        {
            if (collision.contacts.Length == 0)
                return;

            Vector3 normal = collision.contacts[0].normal;
            _direction = Vector3.Reflect(_direction, normal).normalized;

            _rigidBody.linearVelocity = _direction * _speed;
            _rigidBody.angularVelocity = Vector3.zero;
            transform.rotation = Quaternion.LookRotation(_direction, Vector3.up);
        }

        private void ReturnToPool()
        {
            if (_pool == null)
            {
                gameObject.SetActive(false);
                return;
            }

            _pool.Return(this);
        }

        public void OnGetFromPool()
        {
            _timer = 0f;
            _hasHitPlayer = false;

            transform.localScale = _originalScale;

            _rigidBody.linearVelocity = Vector3.zero;
            _rigidBody.angularVelocity = Vector3.zero;
        }

        public void OnReturnToPool()
        {
            _timer = 0f;
            _hasHitPlayer = false;

            _direction = Vector3.zero;
            _speed = 0f;
            _damage = 0;
            _owner = null;

            transform.localScale = _originalScale;

            _rigidBody.linearVelocity = Vector3.zero;
            _rigidBody.angularVelocity = Vector3.zero;
        }
    }
}