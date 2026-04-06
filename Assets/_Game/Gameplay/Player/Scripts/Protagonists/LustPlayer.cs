using UnityEngine;

namespace Pong.Gameplay.Player
{
    public class LustPlayer : PlayerActor
    {
        public override void UseAbility()
        {
            Debug.Log("Lust used attraction ability.");
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