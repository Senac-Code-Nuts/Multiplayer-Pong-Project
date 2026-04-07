using UnityEngine;

namespace Pong.Gameplay.Enemy
{
    public class MinotaurEnemy : EnemyActor
    {
        public override void ExecuteAttack()
        {
            Debug.Log($"{gameObject.name} reflected the relic.");
        }
    }
}