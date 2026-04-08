using System.Collections;
using Pong.Gameplay.Boss;
using Pong.Gameplay.Enemy;
using Pong.Gameplay.Relics;
using UnityEngine;

namespace Pong.Gameplay.Player.Lust
{
    [RequireComponent(typeof(Collider))]
    [RequireComponent(typeof(Rigidbody))]
    public class LustProjectile : MonoBehaviour
    {
        [Header("Projectile Movement")]
        [SerializeField, Range(0.1f, 30f)] private float _speed = 10f;
        [SerializeField, Range(0.1f, 10f)] private float _lifeTime = 2f;

        [Header("Pull Settings")]
        [SerializeField, Range(0.01f, 2f)] private float _pullDuration = 0.2f;

        private LustPlayer _owner;
        private float _bossPullDistance;
        private float _stopDistanceFromPlayer;

        private bool _isInitialized = false;
        private bool _hasHit = false;

        public void Initialize(LustPlayer owner, float bossPullDistance, float stopDistanceFromPlayer)
        {
            if (_isInitialized)
                return;

            _owner = owner;
            _bossPullDistance = bossPullDistance;
            _stopDistanceFromPlayer = stopDistanceFromPlayer;
            _isInitialized = true;

            Destroy(gameObject, _lifeTime);
        }

        private void Update()
        {
            if (!_isInitialized || _hasHit)
                return;

            transform.position += transform.forward * _speed * Time.deltaTime;
        }

        private void OnTriggerEnter(Collider other)
        {
            if (!_isInitialized || _owner == null || _hasHit)
                return;

            if (IsIgnoredTarget(other))
                return;

            if (other.TryGetComponent(out EnemyActor enemy))
            {
                _hasHit = true;
                StartCoroutine(PullEnemyToOwner(enemy.transform));
                return;
            }

            if (other.TryGetComponent(out BossActor boss))
            {
                _hasHit = true;
                StartCoroutine(PullBossTowardOwner(boss.transform));
            }
        }

        private bool IsIgnoredTarget(Collider other)
        {
            if (other.GetComponent<PlayerActor>() != null)
                return true;

            if (other.GetComponent<Relic>() != null)
                return true;

            return false;
        }

        private IEnumerator PullEnemyToOwner(Transform targetTransform)
        {
            Vector3 directionToOwner = (_owner.transform.position - targetTransform.position).normalized;
            Vector3 targetPosition = _owner.transform.position - directionToOwner * _stopDistanceFromPlayer;

            yield return MoveTarget(targetTransform, targetPosition, _pullDuration);

            Debug.Log($"{gameObject.name} pulled enemy {targetTransform.gameObject.name}");
            Destroy(gameObject);
        }

        private IEnumerator PullBossTowardOwner(Transform targetTransform)
        {
            Vector3 directionToOwner = (_owner.transform.position - targetTransform.position).normalized;
            Vector3 targetPosition = targetTransform.position + directionToOwner * _bossPullDistance;

            yield return MoveTarget(targetTransform, targetPosition, _pullDuration);

            Debug.Log($"{gameObject.name} pulled boss {targetTransform.gameObject.name}");
            Destroy(gameObject);
        }

        private IEnumerator MoveTarget(Transform targetTransform, Vector3 destination, float duration)
        {
            Vector3 startPosition = targetTransform.position;
            float elapsedTime = 0f;

            while (elapsedTime < duration)
            {
                elapsedTime += Time.deltaTime;
                float t = elapsedTime / duration;
                targetTransform.position = Vector3.Lerp(startPosition, destination, t);
                yield return null;
            }

            targetTransform.position = destination;
        }
    }
}