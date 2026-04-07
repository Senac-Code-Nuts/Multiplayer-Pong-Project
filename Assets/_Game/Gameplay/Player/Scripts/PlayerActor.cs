using System.Collections;
using UnityEngine;
using Pong.Gameplay.Actors;
using Pong.Systems.Input;

namespace Pong.Gameplay.Player
{
    public abstract class PlayerActor : Actor
    {
        [Header("Input")]
        [SerializeField] protected InputReader _inputReader;

        [Header("Ability")]
        [SerializeField] protected float _abilityCooldown;

        protected bool _canUseAbility = true;

        protected override void Awake()
        {
            base.Awake();
        }

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
            Debug.Log($"{gameObject.name} received CastEvent.");

            if (_isDead || _isStunned || !_canUseAbility) return;

            
            UseAbility();
            StartCoroutine(AbilityCooldownRoutine());
        }

        private IEnumerator AbilityCooldownRoutine()
        {
            _canUseAbility = false;
            yield return new WaitForSeconds(_abilityCooldown);
            _canUseAbility = true;
        }

        protected abstract void UseAbility();

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