using UnityEngine;
using System.Collections;
using Pong.Gameplay.Player;

namespace Pong.Gameplay.Enemy
{
    public class SuccubusEnemy : EnemyActor
    {
        [Header("Specific Attributes")]
        [SerializeField] private float _preAttackTime;
        [SerializeField] private float _atackCooldown;
        [SerializeField] private float _atackRadius;
        
        [Header("Components")]
        [SerializeField] private Renderer _renderer;

        private void OnEnable()
        {
            StartCoroutine(AtackCoroutine());
        }

        public override void ExecuteAttack()
        {
            Collider[] colididos = Physics.OverlapSphere(transform.position, _atackRadius);
            _renderer.material.color = Color.grey;

            for (int i = 0; colididos.Length > 0; i++)
            {
                if (colididos[i].TryGetComponent<PlayerController>(out PlayerController player))
                {

                    // causar dano aos jogadores

                }
            }
        }

        private void OnDrawGizmos()
        {
            Gizmos.color = new Vector4(1,0,0,0.5f);
            Gizmos.DrawSphere(transform.position, _atackRadius);
        }

        private void ExecutePreAttack()
        {

            Debug.Log($"{gameObject.name} executou o pré ataque");
            _renderer.material.color = Color.yellow;

        }

        private void ExecuteMove()
        {

            Debug.Log($"{gameObject.name} se moveu pelo grapho");

        }

        private IEnumerator AtackCoroutine()
        {
            ExecutePreAttack();

            yield return new WaitForSecondsRealtime(_preAttackTime);

            ExecuteAttack();
        }
    }
}