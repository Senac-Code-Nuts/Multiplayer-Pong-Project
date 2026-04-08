using UnityEngine;
using UnityEngine.PlayerLoop;

namespace Pong.Gameplay.Player.Lust
{
    public class LustPlayer : PlayerActor
    {
        [Header("Ability")]
        [SerializeField] private LustProjectile _projectilePrefab;
        [SerializeField] private Transform _projectileSpawnPoint;
        [SerializeField] private int _projectileCount = 1;

        [Header("Balance Settings")]
        [SerializeField, Range(0.1f, 5f)] private float _bossPullDistance = 1.5f;
        [SerializeField, Range(0.1f, 3f)] private float _stopDistanceFromPlayer = 1f;

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


        private void SpawnProjectile()
        {
            if (_projectilePrefab == null || _projectileSpawnPoint == null)
            {
                Debug.LogWarning($"{gameObject.name} is missing projectile setup.");
                return;
            }

            for(int i = 0; i < _projectileCount; i++)
            {
                LustProjectile projectile = Instantiate(
                _projectilePrefab,
                _projectileSpawnPoint.position,
                Quaternion.identity
                );

                projectile.Initialize(
                this,
                _bossPullDistance,
                _stopDistanceFromPlayer
                );
            }


            Debug.Log($"{gameObject.name} fired attraction projectile.");
        }

        protected override void OnDamageTaken()
        {
            Debug.Log($"{gameObject.name} took damage.");
        }

        protected override void OnDeath()
        {
            Debug.Log($"{gameObject.name} died.");
        }
    }
}