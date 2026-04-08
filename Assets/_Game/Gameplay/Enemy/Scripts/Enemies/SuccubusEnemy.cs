using UnityEngine;
using System.Collections;

namespace Pong.Gameplay.Enemy
{
    public class SuccubusEnemy : EnemyActor
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

        public override void ExecuteAttack()
        {
            Debug.Log($"{gameObject.name} executed circular attack.");
        }
        
        private void ExecutePreAttack()
        {

            Debug.Log($"{gameObject.name} executou o pré ataque");

        }

        private void ExecuteMove()
        {

            Debug.Log($"{gameObject.name} se moveu pelo grapho");

        }

        private IEnumerator ActCoroutine()
        {

            ExecuteMove();

            ExecutePreAttack();

            yield return new WaitForSecondsRealtime(_preAttackTime);

            ExecuteAttack();

            yield return new WaitForSecondsRealtime(_atackCooldown);

            StopAllCoroutines();
            StartCoroutine(ActCoroutine());
        } 
    }
}