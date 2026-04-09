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

        [Header("Debug")]
        [SerializeField] private bool _useDebug;
        private int _maxEnemyTargets = 1;

        protected override void UseAbility()
        {
            StunTargets();
            StartCoroutine(AbilityCooldownRoutine());
        }

        protected override void LevelUp()
        {
            base.LevelUp();
            UpgradeEnemyTargets();
        }
        
        private void Update()
        {
            if(Input.GetKeyDown(KeyCode.F1) && _useDebug)
            {
                LevelUp();
            }

        }


        private void UpgradeEnemyTargets()
        {
            _maxEnemyTargets = _level;
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
                    continue;
                }

                if (hit.TryGetComponent(out BossActor boss))
                {
                    boss.ApplyStun(_bossStunDuration);
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

        }

        protected override void OnDeath()
        {

        }
    }
}