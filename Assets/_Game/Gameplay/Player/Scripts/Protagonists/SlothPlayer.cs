using UnityEngine;

namespace Pong.Gameplay.Player
{
    public class SlothPlayer : PlayerActor
    {
        public override void UseAbility()
        {
            Debug.Log("Sloth used shield ability.");
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