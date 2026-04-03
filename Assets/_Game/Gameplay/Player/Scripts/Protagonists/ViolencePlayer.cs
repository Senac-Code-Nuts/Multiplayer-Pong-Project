using UnityEngine;

namespace Pong.Gameplay.Player
{
    public class ViolencePlayer : PlayerActor
    {
        #region Ability
        public override void UseAbility()
        {
            Debug.Log("Violence stunned enemies.");
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