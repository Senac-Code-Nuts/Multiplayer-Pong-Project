using UnityEngine;

namespace Pong.Gameplay.Player
{
    public class LustPlayer : PlayerActor
    {
        [Header("Ability")]
        [SerializeField] private GameObject _projectilePrefab;
        [SerializeField] private Transform _projectileSpawnPoint;

        public override void UseAbility()
        {
            if (_projectilePrefab == null || _projectileSpawnPoint == null)
            {
                Debug.LogWarning($"{gameObject.name} is missing projectile setup.");
                return;
            }

            Instantiate(_projectilePrefab, _projectileSpawnPoint.position, _projectileSpawnPoint.rotation);

            Debug.Log($"{gameObject.name} fired attraction projectile.");
        }
    }
}