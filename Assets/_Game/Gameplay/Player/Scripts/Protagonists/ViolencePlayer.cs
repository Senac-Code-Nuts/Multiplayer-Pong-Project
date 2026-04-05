using UnityEngine;

namespace Pong.Gameplay.Player
{
    public class ViolencePlayer : PlayerActor
    {
        public override void UseAbility()
        {
            Debug.Log("Violence stunned enemies.");
        }

        protected override void OnDamageTaken()
        {
            base.OnDamageTaken();
        }

        protected override void OnDeath()
        {
            base.OnDeath();
        }
    }
}