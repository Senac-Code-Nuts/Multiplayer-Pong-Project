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

        [Header("Damage")]
        [SerializeField] private int _damage;

        private Vector3 _direction;
        private Rigidbody _rigidBody;

        private void Awake()
        {
            _rigidBody = GetComponent<Rigidbody>();
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
            _rigidBody.linearVelocity = _direction * _speed;
        }

        private void FixedUpdate()
        {
            _rigidBody.linearVelocity = _rigidBody.linearVelocity.normalized * _speed;
        }

        private void OnCollisionEnter(Collision collision)
        {

            if (collision.gameObject.TryGetComponent<MinotaurEnemy>(out var m))
            {
                if (m.IsReadyToAttack)
                {
                    TryApplyDamage(collision);
                    return;
                }
            }

            if (collision.gameObject.TryGetComponent<CondemnedSoulEnemy>(out CondemnedSoulEnemy c))
            {
                if (c.IsAttacking)
                {
                    TryApplyDamage(collision);
                    return;
                }

            }

            TryApplyDamage(collision);

            Reflect(collision);

        }

        private void Reflect(Collision collision)
        {
            Vector3 normal = collision.contacts[0].normal;
            _direction = Vector3.Reflect(_direction, normal);
            _rigidBody.linearVelocity = _direction * _speed;
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
         


        }
    }
}