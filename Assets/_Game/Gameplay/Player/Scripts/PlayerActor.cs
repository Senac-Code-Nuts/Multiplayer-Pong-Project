using UnityEngine;
using Pong.Gameplay.Actors;
using Pong.Systems.Input;

namespace Pong.Gameplay.Player
{
    public abstract class PlayerActor : Actor
    {
        [Header("Input")]
        [SerializeField] protected InputReader _inputReader;

        #region Unity Methods
        protected virtual void OnEnable()
        {
            if (_inputReader != null)
                _inputReader.AttackEvent += HandleAbility;
        }

        protected virtual void OnDisable()
        {
            if (_inputReader != null)
                _inputReader.AttackEvent -= HandleAbility;
        }
        #endregion

        #region Ability
        private void HandleAbility()
        {
            if (_isDead || _isStunned)
                return;

            UseAbility();
        }

        public abstract void UseAbility();
        #endregion

        #region Damage
        protected override void OnDamageTaken()
        {
            Debug.Log($"{gameObject.name} took damage.");
        }
        #endregion

        #region Death
        protected override void OnDeath()
        {
            Debug.Log($"{gameObject.name} died.");
        }
        #endregion
    }
}