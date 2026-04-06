using UnityEngine;

namespace Pong.Gameplay.Boss
{
    public class WrathBoss : BossActor
    {
        public override void ExecuteAttack()
        {
            Debug.Log($"{gameObject.name} executed axe attack behavior.");
        }
    }
}