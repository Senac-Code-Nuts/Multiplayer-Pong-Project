using UnityEngine;

namespace Pong.Gameplay.Boss
{
    public class GreedBoss : BossActor
    {
        public override void ExecuteAttack()
        {
            Debug.Log($"{gameObject.name} executed treasure protection behavior.");
        }
    }
}