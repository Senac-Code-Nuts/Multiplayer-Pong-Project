using System.Collections;
using UnityEngine;

namespace Pong.Gameplay.Enemy
{
    public class CerberusEnemy : EnemyActor
    {
        [Header("Specific Attributes")]
        [SerializeField, Range(0.1f, 5f)] private float _preAttackTime = 0.5f;
        [SerializeField, Range(0.1f, 10f)] private float _attackCooldown = 2f;
        [SerializeField, Range(1f, 50f)] private float _projectileSpeed = 12f;
        [SerializeField, Range(1, 32)] private int _projectileCount = 7;
        [SerializeField, Range(0f, 180f)] private float _coneAngle = 120f;

        [Header("Components")]
        [SerializeField] private Transform _graphCenter;
        [SerializeField] private Transform _projectileSpawnPoint;
        [SerializeField] private CerberusShot _shotPrefab;
        [SerializeField] private Renderer _renderer;

        private Color _defaultColor;

        protected override void Awake()
        {
            base.Awake();
            _renderer = GetComponent<Renderer>();
            _defaultColor = _renderer.material.color;
        }

        private void OnEnable()
        {
            StopAllCoroutines();
            StartCoroutine(AttackRoutine());
        }

        public override void ExecuteAttack()
        {
            if (_shotPrefab == null) return;

            Transform spawnPoint = _projectileSpawnPoint != null ? _projectileSpawnPoint : transform;
            Vector3 forward = _graphCenter != null ? (_graphCenter.position - transform.position) : transform.forward;
            forward.y = 0f;

            if (forward.sqrMagnitude < 0.0001f)
            {
                forward = transform.forward;
            }

            forward.Normalize();

            float angleStep = _projectileCount > 1 ? _coneAngle / (_projectileCount - 1) : 0f;
            float startAngle = -_coneAngle * 0.5f;

            for (int i = 0; i < _projectileCount; i++)
            {
                float currentAngle = startAngle + angleStep * i;
                Vector3 shotDirection = Quaternion.Euler(0f, currentAngle, 0f) * forward;

                CerberusShot shot = Instantiate(
                    _shotPrefab, 
                    spawnPoint.position, 
                    Quaternion.LookRotation(shotDirection, Vector3.up)
                );


                shot.Initialize(shotDirection, _projectileSpeed);
            }

            _renderer.material.color = _defaultColor;
        }

        private IEnumerator AttackRoutine()
        {
            while (isActiveAndEnabled)
            {
                FaceGraphCenter();
                ExecutePreAttack();

                yield return new WaitForSecondsRealtime(_preAttackTime);

                ExecuteAttack();

                yield return new WaitForSecondsRealtime(_attackCooldown);
            }
        }

        private void FaceGraphCenter()
        {
            if (_graphCenter == null) return;

            Vector3 lookDirection = _graphCenter.position - transform.position;
            lookDirection.y = 0f;

            if (lookDirection.sqrMagnitude < 0.0001f) return;

            transform.rotation = Quaternion.LookRotation(lookDirection.normalized, Vector3.up);
        }

        private void ExecutePreAttack()
        {
            _renderer.material.color = Color.yellow;
        }

        private void OnDisable()
        {
            StopAllCoroutines();

            _renderer.material.color = _defaultColor;
        }
    }
}