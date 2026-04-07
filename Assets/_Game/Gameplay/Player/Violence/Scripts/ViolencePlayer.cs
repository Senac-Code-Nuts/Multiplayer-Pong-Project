using UnityEngine;
using Pong.Gameplay.Enemy;
using Pong.Gameplay.Boss;

namespace Pong.Gameplay.Player
{
    public class ViolencePlayer : PlayerActor
    {
        [Header("Ability")]
        [SerializeField] private float _stunRadius = 5f;
        [SerializeField] private float _enemyStunDuration = 1f;
        [SerializeField] private float _bossStunDuration = 1f;
        [SerializeField] private int _maxEnemyTargets = 1;

        protected override void UseAbility()
        {
            Debug.Log($"Violence is using ability.");
            StunTargets();
        }

        private void StunTargets()
        {
            Collider[] hits = Physics.OverlapSphere(transform.position, _stunRadius);
            int stunnedEnemies = 0;

            foreach (Collider hit in hits)
            {
                if (hit.TryGetComponent(out EnemyActor enemy))
                {
                    if (stunnedEnemies >= _maxEnemyTargets)
                        continue;

                    enemy.ApplyStun(_enemyStunDuration);
                    stunnedEnemies++;

                    Debug.Log($"{gameObject.name} stunned enemy {hit.gameObject.name}");
                    continue;
                }

                if (hit.TryGetComponent(out BossActor boss))
                {
                    boss.ApplyStun(_bossStunDuration);
                    Debug.Log($"{gameObject.name} stunned boss {hit.gameObject.name}");
                }
            }
        }

        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, _stunRadius);
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