using Pong.Framework.ObjectPool;
using Pong.Gameplay.Player;
using UnityEngine;

namespace Pong.Gameplay.Boss
{
    public class GluttonyProjectile : MonoBehaviour, IPoolable
    {
        [Header("Projectile")]
        [SerializeField] private float _lifetime = 4f;
        [SerializeField] private int _maxBounces = 2;
        //[SerializeField] private float _bounceAngleLimit = 85f;

        private Vector3 _direction;
        private float _speed;
        private int _damage;
        private float _lifeTimer;
        private int _currentBounces;
        private bool _hasHitPlayer;
        private Transform _owner;

        private GluttonyProjectilePool _pool;

        public void Initialize(
            Vector3 direction,
            float speed,
            int damage,
            GluttonyProjectilePool pool,
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
            _pool = pool;
            _owner = owner;

            if (_direction.sqrMagnitude > 0.0001f)
            {
                transform.rotation = Quaternion.LookRotation(_direction, Vector3.up);
            }
        }
        private void Update()
        {
            transform.position += _direction * _speed * Time.deltaTime;

            _lifeTimer += Time.deltaTime;
            if (_lifeTimer >= _lifetime)
            {
                ReturnToPool();
            }
        }

        private void OnCollisionEnter(Collision collision)
        {
            if (_hasHitPlayer)
                return;

            if (_owner != null)
            {
                if (collision.transform == _owner)
                    return;

                if (collision.transform.IsChildOf(_owner))
                    return;
            }

            if (collision.gameObject.GetComponent<GluttonyProjectile>() != null)
                return;

            if (collision.contacts.Length == 0)
                return;

            if (_currentBounces >= _maxBounces)
            {
                ReturnToPool();
                return;
            }

            Vector3 normal = collision.contacts[0].normal;
            Vector3 reflectedDirection = Vector3.Reflect(_direction, normal);

            if (reflectedDirection.sqrMagnitude <= 0.0001f)
            {
                ReturnToPool();
                return;
            }

            reflectedDirection.Normalize();
            _direction = reflectedDirection;

            if (_direction.sqrMagnitude > 0.0001f)
            {
                transform.rotation = Quaternion.LookRotation(_direction, Vector3.up);
            }

            _currentBounces++;
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

                Debug.Log($"[GluttonyProjectile] Acertou o player {player.name} e causou {_damage} de dano.");

                player.ApplyDamage(_damage);
                ReturnToPool();
            }
        }

        public void OnGetFromPool()
        {
            _lifeTimer = 0f;
            _currentBounces = 0;
            _hasHitPlayer = false;
        }

        public void OnReturnToPool()
        {
            _lifeTimer = 0f;
            _currentBounces = 0;
            _hasHitPlayer = false;
            _direction = Vector3.zero;
            _speed = 0f;
            _damage = 0;
            _owner = null;
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
    }
}