using System.Collections;
using UnityEngine;

namespace Pong.Gameplay.Actors
{
    public abstract class Actor : MonoBehaviour, IDamageable
    {
        [Header("Attributes")]
        [SerializeField] protected int _maxHealth;
        [SerializeField] protected int _currentHealth;
        [SerializeField] protected int _damage;

        [Header("States")]
        [SerializeField] protected bool _isVulnerable = true;
        [SerializeField] protected bool _isStunned = false;
        [SerializeField] protected bool _isDead = false;

        [Header("Stun Visual")]
        [SerializeField] protected GameObject _stunVfxPrefab;
        [SerializeField] protected Vector3 _stunOffset = new Vector3(0f, 1.8f, 0f);

        protected GameObject _activeStunVfx;

        public int CurrentHealth => _currentHealth;
        public int Damage => _damage;
        public bool IsDead => _isDead;
        public bool IsAlive => !_isDead;
        public bool IsStunned => _isStunned;
        public bool IsVulnerable => _isVulnerable;

        protected virtual void Awake()
        {
            _currentHealth = _maxHealth;
        }

        #region Damage
        public virtual void ApplyDamage(int damage)
        {
            if (!_isVulnerable || _isDead)
                return;

            _currentHealth -= damage;

            OnDamageTaken();

            if (_currentHealth <= 0)
            {
                _currentHealth = 0;
                Die();
            }
        }

        protected abstract void OnDamageTaken();
        #endregion

        #region Death
        protected virtual void Die()
        {
            if (_isDead)
                return;

            _isDead = true;
            OnDeath();
        }
        
        protected abstract void OnDeath();
        #endregion

        #region Stun
        public virtual void ApplyStun(float duration)
        {
            if (_isDead || _isStunned) return;

            StartCoroutine(StunRoutine(duration));
        }


        private IEnumerator StunRoutine(float duration)
        {
            _isStunned = true;
            ShowStunVfx();

            yield return new WaitForSeconds(duration);

            _isStunned = false;
            HideStunVfx();
        }

        protected virtual void ShowStunVfx()
        {
            if (_stunVfxPrefab == null) return;
            if (_activeStunVfx != null) return;

            _activeStunVfx = Instantiate(
                _stunVfxPrefab,
                transform.position + _stunOffset,
                Quaternion.identity
            );

            if (_activeStunVfx.TryGetComponent(out StunVfxFollower follower))
            {
                follower.Initialize(transform, _stunOffset);
            }
        }

        protected virtual void HideStunVfx()
        {
            if (_activeStunVfx == null) return;

            Destroy(_activeStunVfx);
            _activeStunVfx = null;
        }

        #endregion

        #region Vulnerability
        public virtual void SetVulnerable(bool value)
        {
            _isVulnerable = value;
        }
        #endregion
    }
}