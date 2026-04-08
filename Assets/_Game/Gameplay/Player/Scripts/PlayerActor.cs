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
        [SerializeField, Range(0f, 10f)] protected float _abilityCooldown;

        [Header("Shield")]
        [SerializeField] protected bool _hasShield = false;
        [SerializeField] protected GameObject _activeShieldVisual;
        [SerializeField] protected Vector3 _shieldOffset = new Vector3(0f, 1.8f, 0f);
        [SerializeField] protected Vector3 _shieldVisualScale = new Vector3(0f, 0f, 0f);

        protected bool _canUseAbility = true;

        public bool HasShield => _hasShield;

        protected override void Awake()
        {
            base.Awake();
        }

        protected virtual void OnEnable()
        {
            if (_inputReader != null)
            {
                _inputReader.CastEvent += HandleAbility;
            }
        }

        protected virtual void OnDisable()
        {
            if (_inputReader != null)
            {
                _inputReader.CastEvent -= HandleAbility;
            }
        }

        private void HandleAbility()
        {
            if (_isDead || _isStunned || !_canUseAbility) return;

            UseAbility();
        }

        protected IEnumerator AbilityCooldownRoutine()
        {
            _canUseAbility = false;
            yield return new WaitForSeconds(_abilityCooldown);
            _canUseAbility = true;
        }

        public override void ApplyDamage(int damage)
        {
            if (_isDead) return;

            if (_hasShield)
            {
                ConsumeShield();
                Debug.Log($"{gameObject.name} blocked damage with shield.");
                return;
            }

            base.ApplyDamage(damage);
        }

        public void ReceiveShield(GameObject shieldVisualPrefab = null)
        {
            _hasShield = true;

            if (_activeShieldVisual != null)
            {
                Destroy(_activeShieldVisual);
            }

            if (shieldVisualPrefab != null)
            {
                _activeShieldVisual = Instantiate(
                    shieldVisualPrefab,
                    transform.position + _shieldOffset,
                    Quaternion.identity
                );

                _activeShieldVisual.transform.localScale = _shieldVisualScale;

                if (_activeShieldVisual.TryGetComponent(out ShieldVisualFollower follower))
                {
                    follower.Initialize(transform, _shieldOffset);
                }
            }

            Debug.Log($"{gameObject.name} received shield.");
        }

        private void ConsumeShield()
        {
            _hasShield = false;

            if (_activeShieldVisual != null)
            {
                Destroy(_activeShieldVisual);
                _activeShieldVisual = null;
            }

            Debug.Log($"{gameObject.name} shield consumed.");
        }

        protected abstract void UseAbility();

        protected override void OnDamageTaken()
        {
            Debug.Log($"{gameObject.name} took damage.");
        }

        protected override void OnDeath()
        {
            Debug.Log($"{gameObject.name} died.");

            if (_activeShieldVisual != null)
            {
                Destroy(_activeShieldVisual);
                _activeShieldVisual = null;
            }
        }
    }
}