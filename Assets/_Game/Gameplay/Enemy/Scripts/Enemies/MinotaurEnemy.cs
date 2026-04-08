using System.Collections;
using UnityEngine;

namespace Pong.Gameplay.Enemy
{
    public class MinotaurEnemy : EnemyActor
    {
        [Header("Specific Attributes")]
        [SerializeField] private bool _isReadyToAttack;
        [SerializeField] private float _preAttackTime;

        public override void ApplyDamage(int damage)
        {
            base.ApplyDamage(damage);

            if (_isReadyToAttack) {

                ExecuteAttack();
                _isVulnerable = true;
            }
        }

        private void ExecuteMove()
        {

            Debug.Log($"{gameObject.name} se moveu pelo grapho");

        }

        private IEnumerator PreAttackCoroutine()
        {
            yield return new WaitForSecondsRealtime(_preAttackTime);

            _isVulnerable = false;
            _isReadyToAttack = true;
        }

        public override void ExecuteAttack()
        {
            Debug.Log($"{gameObject.name} reflected the relic.");
        }
    }
}