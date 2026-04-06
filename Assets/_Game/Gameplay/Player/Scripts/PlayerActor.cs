using UnityEngine;
using Pong.Gameplay.Actors;
using Pong.Systems.Input;

namespace Pong.Gameplay.Player
{
    public abstract class PlayerActor : Actor
    {
        [Header("Input")]
        [SerializeField] protected InputReader _inputReader;
        protected virtual void OnEnable()
        {
            if (_inputReader != null)
                _inputReader.CastEvent += HandleAbility;
        }

        protected virtual void OnDisable()
        {
            if (_inputReader != null)
                _inputReader.CastEvent -= HandleAbility;
        }

        private void HandleAbility()
        {
            if (_isDead || _isStunned)
                return;

            UseAbility();
        }

        public abstract void UseAbility();

        protected override void OnDamageTaken()
        {
            Debug.Log($"{gameObject.name} took damage.");
        }

        protected override void OnDeath()
        {
            Debug.Log($"{gameObject.name} died.");
        }
    }
}