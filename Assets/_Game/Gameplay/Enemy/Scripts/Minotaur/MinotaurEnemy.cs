using System.Collections;
using UnityEngine;
using Pong.Gameplay.Relics;

namespace Pong.Gameplay.Enemy
{
    public class MinotaurEnemy : EnemyActor
    {
        [Header("Specific Attributes")]
        [SerializeField] private bool _isReadyToAttack;
        [SerializeField] private float _preAttackTime;

        [Header("Components")]
        [SerializeField] private Relic _relic;
        [SerializeField] private Renderer _renderer;

        private void OnEnable()
        {
            StartCoroutine(PreAttackCoroutine());
        }

        public bool IsReadyToAttack => _isReadyToAttack;       
        public override void ApplyDamage(int damage)
        {
            base.ApplyDamage(damage);

            if (_isReadyToAttack)
            {
                ExecuteAttack();
                _isVulnerable = true;
                _isReadyToAttack = false;
                _renderer.material.color = Color.gray;
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
            _renderer.material.color = Color.yellow;
        }

        public override void ExecuteAttack()
        {
            _relic.InvertDirection();
        }
    }
}