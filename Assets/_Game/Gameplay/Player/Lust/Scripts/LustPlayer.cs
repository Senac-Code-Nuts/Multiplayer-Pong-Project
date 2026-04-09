using UnityEngine;

namespace Pong.Gameplay.Player.Lust
{
    public class LustPlayer : PlayerActor
    {
        [Header("Ability")]
        [SerializeField] private LustProjectile _projectilePrefab;
        [SerializeField] private Transform _projectileSpawnPoint;
        [SerializeField] private float _spreadAngle = 30f;
        private int _projectileCount = 1;

        [Header("Balance Settings")]
        [SerializeField, Range(0.1f, 5f)] private float _bossPullDistance = 1.5f;
        [SerializeField, Range(0.1f, 3f)] private float _stopDistanceFromPlayer = 1f;

        [Header("Debug")]
        [SerializeField] private bool _useDebug;

        protected override void UseAbility()
        {
            SpawnProjectile();
            StartCoroutine(AbilityCooldownRoutine());
        }

        protected override void LevelUp()
        {
            base.LevelUp();
            UpgradeProjectileCount();
        }

        private void UpgradeProjectileCount()
        {
            _projectileCount = _level;
        }

        private void Update()
        {
            if(Input.GetKeyDown(KeyCode.F1) && _useDebug)
            {
                LevelUp();
            }
        }

        private void SpawnProjectile()
        {
            if (_projectilePrefab == null || _projectileSpawnPoint == null)
            {
                return;
            }

            for(int i = 0; i < _projectileCount; i++)
            {
                float angleStep = _projectileCount > 1 ? _spreadAngle / (_projectileCount - 1) : 0;
                float angle = -_spreadAngle / 2 + angleStep * i;

                Quaternion rotation = _projectileSpawnPoint.rotation * Quaternion.Euler(0,angle,0);


                LustProjectile projectile = Instantiate(
                _projectilePrefab,
                _projectileSpawnPoint.position,
                rotation
                );

                projectile.Initialize(
                this,
                _bossPullDistance,
                _stopDistanceFromPlayer
                );
            }
        }

        protected override void OnDamageTaken()
        {

        }

        protected override void OnDeath()
        {

        }
    }
}