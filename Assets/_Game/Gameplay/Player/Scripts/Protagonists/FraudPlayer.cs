using UnityEngine;

namespace Pong.Gameplay.Player
{
    public class FraudPlayer : PlayerActor
    {
        #region Ability
        public override void UseAbility()
        {
            Debug.Log("Fraud duplicated the relic.");
        }
        #endregion

        #region Damage
        protected override void OnDamageTaken()
        {
            base.OnDamageTaken();
        }
        #endregion

        #region Death
        protected override void OnDeath()
        {
            base.OnDeath();
        }
        #endregion
    }
}