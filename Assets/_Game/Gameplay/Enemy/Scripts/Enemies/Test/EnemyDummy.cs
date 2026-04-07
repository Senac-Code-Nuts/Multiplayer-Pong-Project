using UnityEngine;
using Pong.Gameplay.Enemy;

namespace Pong.Gameplay.Enemy.Test
{
    public class EnemyDummy : EnemyActor
    {
        protected override void Awake()
        {
            base.Awake();
            _currentHealth = _maxHealth;
        }

        public override void ExecuteAttack()
        {
            // Dummy não faz nada
        }

        protected override void OnDamageTaken()
        {
            Debug.Log($"[DUMMY] {gameObject.name} took damage. HP: {_currentHealth}");
        }

        protected override void OnDeath()
        {
            Debug.Log($"[DUMMY] {gameObject.name} died.");

            // Para facilitar teste:
            Destroy(gameObject);
        }
    }
}