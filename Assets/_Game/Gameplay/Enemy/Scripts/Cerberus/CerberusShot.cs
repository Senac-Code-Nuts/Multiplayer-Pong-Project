using Pong.Gameplay.Player;
using UnityEngine;

namespace Pong.Gameplay.Enemy.Cerberus
{
    public class CerberusShot : MonoBehaviour
    {
        [Header("Specific Attributes")]
        [SerializeField] private int _damage = 1;
        [SerializeField] private float _lifetime = 4f;
        [SerializeField] private float _bounceAngleLimit = 80f;

        private Vector3 _direction;
        private float _speed;
        private float _lifeTimer;

        public void Initialize(Vector3 direction, float speed)
        {
            _direction = direction.normalized;
            _speed = speed;
            _lifeTimer = 0f;
        }

        private void Update()
        {
            transform.position += _direction * _speed * Time.deltaTime;

            _lifeTimer += Time.deltaTime;
            if (_lifeTimer >= _lifetime)
            {
                Destroy(gameObject);
            }
        }

        private void OnCollisionEnter(Collision collision)
        {
            if (collision.contacts.Length == 0) return;

            Vector3 normal = collision.contacts[0].normal;
            Vector3 reflectedDirection = Vector3.Reflect(_direction, normal).normalized;

            float angleToForward = Vector3.Angle(Vector3.forward, reflectedDirection);
            if (angleToForward <= _bounceAngleLimit)
            {
                _direction = reflectedDirection;
                transform.rotation = Quaternion.LookRotation(_direction, Vector3.up);
                return;
            }

            Destroy(gameObject);
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.TryGetComponent<PlayerActor>(out PlayerActor player))
            {
                player.ApplyDamage(_damage);
            }
        }
    }
}
