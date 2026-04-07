using UnityEngine;

namespace Pong.Gameplay.Enemy
{
    public class CondemnedSoulEnemy : EnemyActor
    {
        public override void ExecuteAttack()
        {
            Debug.Log($"{gameObject.name} entered invulnerable reflective state.");
        }
    }
}