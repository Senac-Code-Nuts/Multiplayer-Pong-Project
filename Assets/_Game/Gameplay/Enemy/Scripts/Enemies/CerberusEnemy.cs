using System.Collections;
using UnityEngine;

namespace Pong.Gameplay.Enemy
{
    public class CerberusEnemy : EnemyActor
    {
        [Header("Specific Attributes")]
        [SerializeField] private float _preAttackTime;
        [SerializeField] private float _atackCooldown;

        private void OnEnable()
        {
            StartCoroutine(ActCoroutine());
        }

        private void OnDisable()
        {
            StopAllCoroutines();
        }

        private void ExecutePreAttack()
        {

            Debug.Log($"{gameObject.name} executou o pré ataque");

        }

        public override void ExecuteAttack()
        {
            Debug.Log($"{gameObject.name} fired cone projectiles.");
        }

        private IEnumerator ActCoroutine()
        {

            ExecutePreAttack();

            yield return new WaitForSecondsRealtime(_preAttackTime);

            ExecuteAttack();

            yield return new WaitForSecondsRealtime(_atackCooldown);

            StopAllCoroutines();
            StartCoroutine(ActCoroutine());

        }
    }
}