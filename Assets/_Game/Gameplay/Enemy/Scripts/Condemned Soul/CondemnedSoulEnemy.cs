using System.Collections;
using UnityEngine;
using Pong.Gameplay.Relics;

namespace Pong.Gameplay.Enemy
{
    public class CondemnedSoulEnemy : EnemyActor
    {
        [Header("Specific Attributes")]
        [SerializeField] private float _invunerableTime;
        [SerializeField] private bool _isAttacking;

        public bool IsAttacking => _isAttacking;

        [Header("Components")]
        [SerializeField] private Relic _relic;
        [SerializeField] private Renderer _renderer;

        private void OnEnable()
        {
            ExecuteAttack();
        }

        public override void ExecuteAttack()
        {
            StopAllCoroutines();
            StartCoroutine(InvunerableCoroutine());
        }

        private void ExecuteMove()
        {
            Debug.Log($"{gameObject.name} se moveu pelo grapho");
            ExecuteAttack();
        }

        private IEnumerator InvunerableCoroutine()
        {
            _isVulnerable = false;
            _isAttacking = true;
            _renderer.material.color = Color.yellow;

            yield return new WaitForSecondsRealtime(_invunerableTime);

            _isVulnerable = true;
            _isAttacking = false;
            _renderer.material.color = Color.gray;
        }
    }
}