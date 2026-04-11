using UnityEngine;

namespace Pong.Gameplay.Boss
{
    public class EnvyBoss : BossActor
    {
        public override void ExecuteAttack()
        {
            Debug.Log($"{gameObject.name} executed slot machine behavior.");
        }
    }
}