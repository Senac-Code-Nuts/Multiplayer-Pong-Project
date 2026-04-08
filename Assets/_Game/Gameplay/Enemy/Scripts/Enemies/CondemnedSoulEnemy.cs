using System.Collections;
using UnityEngine;

namespace Pong.Gameplay.Enemy
{
    public class CondemnedSoulEnemy : EnemyActor
    {
        [Header("Specific Attributes")]
        [SerializeField] private float _invunerableTime;

        public override void ExecuteAttack()
        {
            StartCoroutine(InvunerableCoroutine());
        }

        private void ExecuteMove()
        {

            Debug.Log($"{gameObject.name} se moveu pelo grapho");

        }

        private IEnumerator InvunerableCoroutine()
        {
            
            _isVulnerable = false;

            yield return new WaitForSecondsRealtime(_invunerableTime);

            _isVulnerable = true;  

        }

    }
}