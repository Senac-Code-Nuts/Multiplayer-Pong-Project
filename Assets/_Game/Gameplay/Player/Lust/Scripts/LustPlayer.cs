using UnityEngine;

namespace Pong.Gameplay.Player.Lust
{
    public class LustPlayer : PlayerActor
    {
        [Header("Ability")]
        [SerializeField] private LustProjectile _projectilePrefab;
        [SerializeField] private Transform _projectileSpawnPoint;

        [Header("Balance Settings")]
        [SerializeField] private float _bossPullDistance;
        [SerializeField] private float _stopDistanceFromPlayer;

        protected override void UseAbility()
        {
            SpawnProjectile();
        }

        private void SpawnProjectile()
        {
            if (_projectilePrefab == null || _projectileSpawnPoint == null)
            {
                Debug.LogWarning($"{gameObject.name} is missing projectile setup.");
                return;
            }

            LustProjectile.Spawn(
                _projectilePrefab,
                _projectileSpawnPoint.position,
                _projectileSpawnPoint.rotation,
                this,
                _bossPullDistance,
                _stopDistanceFromPlayer
            );

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