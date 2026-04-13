using UnityEngine;
using Pong.Gameplay.Actors;

namespace Pong.Gameplay.Enemy
{
    public abstract class EnemyActor : Actor
    {
        public abstract void ExecuteAttack();

        protected override void OnDamageTaken()
        {
            Debug.Log($"{gameObject.name} enemy took damage. Health: {_currentHealth}/{_maxHealth}");
        }
        protected override void OnDeath()
        {
            StopAllCoroutines();
            SetVulnerable(false);
            gameObject.SetActive(false);
        }
    }
}