using UnityEngine;

namespace Pong.Gameplay.Player
{
    public class LustPlayer : PlayerActor
    {
        #region Ability
        public override void UseAbility()
        {
            Debug.Log("Lust used attraction ability.");
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