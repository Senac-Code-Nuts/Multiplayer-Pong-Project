using System.Collections;
using UnityEngine;

namespace PingPong.Features.Actors
{
    public abstract class Actor : MonoBehaviour, IDamageable
    {
        [Header("Atributos")]
        [SerializeField] protected int _maxHealth;
        [SerializeField] protected int _currentHealth;
        [SerializeField] protected int _damage;

        [Header("Estados")]
        [SerializeField] protected bool _isVulnerable = true;
        [SerializeField] protected bool _isStunned = false;
        [SerializeField] protected bool _isDead = false;

        public int CurrentHealth => _currentHealth;
        public bool IsDead => _isDead;
        public bool IsAlive => !_isDead;
        public bool IsStunned => _isStunned;
        public bool IsVulnerable => _isVulnerable;

        protected virtual void Awake()
        {
            _currentHealth = _maxHealth;
        }

        // =========================
        // DANO
        // =========================

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

        protected virtual void OnDamageTaken()
        {
        }

        // =========================
        // MORTE
        // =========================

        protected virtual void Die()
        {
            if (_isDead)
                return;

            _isDead = true;
            OnDeath();
        }

        protected abstract void OnDeath();

        // =========================
        // STUN
        // =========================
        public virtual void ApplyStun(float duration)
        {
            if (_isDead || _isStunned)
                return;

            StartCoroutine(StunRoutine(duration));
        }


        private IEnumerator StunRoutine(float duration)
        {
            _isStunned = true;
            yield return new WaitForSeconds(duration);
            _isStunned = false;
        }

        // =========================
        // VULNERABILIDADE
        // =========================

        public virtual void SetVulnerable(bool value)
        {
            _isVulnerable = value;
        }
    }
}