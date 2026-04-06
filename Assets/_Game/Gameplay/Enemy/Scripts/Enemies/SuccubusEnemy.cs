using UnityEngine;

namespace Pong.Gameplay.Enemy
{
    public class SuccubusEnemy : EnemyActor
    {
        public override void ExecuteAttack()
        {
            Debug.Log($"{gameObject.name} executed circular attack.");
        }
    }
}