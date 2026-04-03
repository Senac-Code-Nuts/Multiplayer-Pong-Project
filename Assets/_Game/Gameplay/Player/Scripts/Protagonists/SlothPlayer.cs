using UnityEngine;

namespace Pong.Gameplay.Player
{
    public class SlothPlayer : PlayerActor
    {
        #region Ability
        public override void UseAbility()
        {
            Debug.Log("Sloth used shield ability.");
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