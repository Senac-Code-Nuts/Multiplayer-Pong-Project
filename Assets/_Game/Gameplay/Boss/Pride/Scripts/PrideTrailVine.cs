using Pong.Framework.ObjectPool;
using UnityEngine;

namespace Pong.Gameplay.Boss
{
    [RequireComponent(typeof(Rigidbody))]
    [RequireComponent(typeof(Collider))]
    public class PrideTrailVine : MonoBehaviour, IPoolable
    {
        private const string WALL_TAG = "Wall";
        private const string RELIC_TAG = "Relic";

        private Rigidbody _rigidBody;
        private PrideTrailVinePool _pool;

        private float _lifetime;
        private float _timer;
        private float _pushSpeed;

        private void Awake()
        {
            _rigidBody = GetComponent<Rigidbody>();
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
            PrideTrailVinePool pool,
            float lifetime,
            float pushSpeed,
            Vector3 initialDirection
        )
        {
            _pool = pool;
            _lifetime = lifetime;
            _pushSpeed = pushSpeed;

            _timer = 0f;

            _rigidBody.linearVelocity = Vector3.zero;
            _rigidBody.angularVelocity = Vector3.zero;

            initialDirection.y = 0f;

            if (initialDirection.sqrMagnitude > 0.0001f)
            {
                transform.rotation = Quaternion.LookRotation(initialDirection.normalized, Vector3.up);
            }
        }

        private void OnCollisionEnter(Collision collision)
        {
            if (collision.gameObject.CompareTag(WALL_TAG))
            {
                ReturnToPool();
                return;
            }

            if (collision.gameObject.CompareTag(RELIC_TAG))
            {
                PushFromRelic(collision);
            }
        }

        private void PushFromRelic(Collision collision)
        {
            if (collision.contacts.Length == 0)
                return;

            ContactPoint contact = collision.contacts[0];
            Vector3 direction = -contact.normal;
            direction.y = 0f;

            if (direction.sqrMagnitude <= 0.0001f)
                return;

            direction.Normalize();

            _rigidBody.linearVelocity = direction * _pushSpeed;
            _rigidBody.angularVelocity = Vector3.zero;
            transform.rotation = Quaternion.LookRotation(direction, Vector3.up);
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

            _rigidBody.linearVelocity = Vector3.zero;
            _rigidBody.angularVelocity = Vector3.zero;
        }

        public void OnReturnToPool()
        {
            _timer = 0f;
            _pushSpeed = 0f;

            _rigidBody.linearVelocity = Vector3.zero;
            _rigidBody.angularVelocity = Vector3.zero;
        }
    }
}