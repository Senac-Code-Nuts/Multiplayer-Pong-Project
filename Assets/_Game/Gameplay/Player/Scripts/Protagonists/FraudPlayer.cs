using UnityEngine;

namespace Pong.Gameplay.Player
{
    public class FraudPlayer : PlayerActor
    {
        public override void UseAbility()
        {
            Debug.Log("Fraud duplicated the relic.");
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