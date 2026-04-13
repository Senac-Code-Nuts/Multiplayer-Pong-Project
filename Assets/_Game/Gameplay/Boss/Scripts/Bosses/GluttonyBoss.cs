using UnityEngine;

namespace Pong.Gameplay.Boss
{
    public class GluttonyBoss : BossActor
    {
        public override void ExecuteAttack()
        {
            Debug.Log($"{gameObject.name} executed eating and projectile behavior.");
        }
    }
}