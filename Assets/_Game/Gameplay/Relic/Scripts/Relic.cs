using UnityEngine;
using Pong.Gameplay.Actors;
using Pong.Gameplay.Player;
using Pong.Gameplay.Enemy;
using Pong.Gameplay.Boss;

namespace Pong.Gameplay.Relics
{
    [RequireComponent(typeof(Rigidbody))]
    public class Relic : MonoBehaviour
    {
        [Header("Speed")]
        [SerializeField, Range(0f, 10f)] private float _speed;

        [Header("Damage")]
        [SerializeField] private int _damage;

        [Header("Fraud Copy")]
        [SerializeField] private bool _isFraudCopy = false;
        [SerializeField] private Renderer[] _renderers;
        [SerializeField] private Color _fraudCopyColor = Color.magenta;

        private Vector3 _direction;
        private Rigidbody _rigidBody;

        public bool IsFraudCopy => _isFraudCopy;

        private void Awake()
        {
            _rigidBody = GetComponent<Rigidbody>();
        }

        private void Start()
        {
            Launch();
            ApplyCurrentVisual();
        }

        private void Launch()
        {
            float dirX = Random.Range(-1f, 1f);
            float dirZ = Random.Range(-1f, 1f);

            _direction = new Vector3(dirX, 0f, dirZ).normalized;
            _rigidBody.linearVelocity = _direction * _speed;
        }

        private void FixedUpdate()
        {
            _rigidBody.linearVelocity = _rigidBody.linearVelocity.normalized * _speed;
        }

        public void SetAsFraudCopy(bool value)
        {
            _isFraudCopy = value;
            ApplyCurrentVisual();
        }

        private void ApplyCurrentVisual()
        {
            if (!_isFraudCopy) return;
            if (_renderers == null || _renderers.Length == 0) return;

            foreach (Renderer currentRenderer in _renderers)
            {
                if (currentRenderer == null) continue;

                Material[] materials = currentRenderer.materials;

                foreach (Material currentMaterial in materials)
                {
                    if (currentMaterial.HasProperty("_BaseColor"))
                    {
                        currentMaterial.SetColor("_BaseColor", _fraudCopyColor);
                        continue;
                    }

                    if (currentMaterial.HasProperty("_Color"))
                    {
                        currentMaterial.SetColor("_Color", _fraudCopyColor);
                    }
                }
            }
        }

        private void OnCollisionEnter(Collision collision)
        {
            if (_isFraudCopy && ShouldDestroyFraudCopy(collision))
            {
                TryApplyDamage(collision);
                Destroy(gameObject);
                return;
            }

            Reflect(collision);
            TryTriggerFraudCopy(collision);
            TryApplyDamage(collision);
        }

        private void Reflect(Collision collision)
        {
            Vector3 normal = collision.contacts[0].normal;
            _direction = Vector3.Reflect(_direction, normal);
            _rigidBody.linearVelocity = _direction * _speed;
        }

        private void TryApplyDamage(Collision collision)
        {
            if (collision.gameObject.GetComponent<PlayerActor>() != null) return;

            if (collision.gameObject.TryGetComponent(out IDamageable damageable))
            {
                damageable.ApplyDamage(_damage);
            }
        }

        private void TryTriggerFraudCopy(Collision collision)
        {
            if (collision.gameObject.TryGetComponent(out FraudPlayer fraudPlayer))
            {
                fraudPlayer.TryCopyRelic(this);
            }
        }

        private bool ShouldDestroyFraudCopy(Collision collision)
        {
            if (collision.gameObject.GetComponent<EnemyActor>() != null) return true;
            if (collision.gameObject.GetComponent<BossActor>() != null) return true;
            if (collision.gameObject.CompareTag("Wall")) return true;

            return false;
        }
    }
}