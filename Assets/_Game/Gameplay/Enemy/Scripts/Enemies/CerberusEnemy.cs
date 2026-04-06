using UnityEngine;

namespace Pong.Gameplay.Enemy
{
    public class CerberusEnemy : EnemyActor
    {
        public override void ExecuteAttack()
        {
            Debug.Log($"{gameObject.name} fired cone projectiles.");
        }
    }
}