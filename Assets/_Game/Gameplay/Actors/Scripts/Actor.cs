using System.Collections;
using UnityEngine;

namespace Pong.Gameplay.Actors
{
    public abstract class Actor : MonoBehaviour, IDamageable
    {
        [Header("General Attributes")]
        [SerializeField] protected int _maxHealth;
        [SerializeField] protected int _currentHealth;
        [SerializeField] protected int _damage;

        [Header("Damage Visual")]
        [SerializeField] private Renderer[] _damageFlashRenderers;
        [SerializeField] private Color _damageFlashColor = Color.red;
        [SerializeField, Min(0f)] private float _damageFlashDuration = 0.1f;

        [Header("States")]
        [SerializeField] protected bool _isVulnerable = true;
        [SerializeField] protected bool _isStunned = false;
        [SerializeField] protected bool _isDead = false;

        [Header("Stun Visual")]
        [SerializeField] protected GameObject _stunVfxPrefab;
        [SerializeField] protected Vector3 _stunOffset = new Vector3(0f, 1.8f, 0f);

        protected GameObject _activeStunVfx;
        private Color[] _damageFlashOriginalColors;
        private Coroutine _damageFlashRoutine;

        public int CurrentHealth => _currentHealth;
        public int Damage => _damage;
        public bool IsDead => _isDead;
        public bool IsAlive => !_isDead;
        public bool IsStunned => _isStunned;
        public bool IsVulnerable => _isVulnerable;

        protected virtual void Awake()
        {
            _currentHealth = _maxHealth;

            if (_damageFlashRenderers == null || _damageFlashRenderers.Length == 0)
            {
                _damageFlashRenderers = GetComponentsInChildren<Renderer>(true);
            }
        }

        #region Damage
        public virtual void ApplyDamage(int damage)
        {
            if (!_isVulnerable || _isDead)
                return;

            _currentHealth -= damage;

            OnDamageTaken();
            FlashDamageVisual();

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

            StopDamageFlash();
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

        private void FlashDamageVisual()
        {
            Renderer[] renderers = GetDamageFlashRenderers();

            if (renderers.Length == 0 || _damageFlashDuration <= 0f)
                return;

            if (_damageFlashRoutine != null)
            {
                StopCoroutine(_damageFlashRoutine);
                _damageFlashRoutine = null;
                RestoreDamageFlashColors(renderers);
            }

            CacheDamageFlashColors(renderers);
            SetDamageFlashColors(renderers, _damageFlashColor);

            _damageFlashRoutine = StartCoroutine(DamageFlashRoutine(renderers));
        }

        private IEnumerator DamageFlashRoutine(Renderer[] renderers)
        {
            yield return new WaitForSeconds(_damageFlashDuration);

            RestoreDamageFlashColors(renderers);
            _damageFlashRoutine = null;
        }

        private void StopDamageFlash()
        {
            if (_damageFlashRoutine != null)
            {
                StopCoroutine(_damageFlashRoutine);
                _damageFlashRoutine = null;
            }

            RestoreDamageFlashColors(GetDamageFlashRenderers());
        }

        private Renderer[] GetDamageFlashRenderers()
        {
            if (_damageFlashRenderers == null)
            {
                _damageFlashRenderers = GetComponentsInChildren<Renderer>(true);
            }

            return _damageFlashRenderers;
        }

        private void CacheDamageFlashColors(Renderer[] renderers)
        {
            if (_damageFlashOriginalColors == null || _damageFlashOriginalColors.Length != renderers.Length)
            {
                _damageFlashOriginalColors = new Color[renderers.Length];
            }

            for (int i = 0; i < renderers.Length; i++)
            {
                _damageFlashOriginalColors[i] = GetRendererColor(renderers[i]);
            }
        }

        private void RestoreDamageFlashColors(Renderer[] renderers)
        {
            if (_damageFlashOriginalColors == null || _damageFlashOriginalColors.Length != renderers.Length)
                return;

            for (int i = 0; i < renderers.Length; i++)
            {
                SetRendererColor(renderers[i], _damageFlashOriginalColors[i]);
            }
        }

        private void SetDamageFlashColors(Renderer[] renderers, Color color)
        {
            for (int i = 0; i < renderers.Length; i++)
            {
                SetRendererColor(renderers[i], color);
            }
        }

        private static Color GetRendererColor(Renderer renderer)
        {
            if (renderer == null)
                return Color.white;

            if (renderer is SpriteRenderer spriteRenderer)
            {
                return spriteRenderer.color;
            }

            Material sharedMaterial = renderer.sharedMaterial;

            if (sharedMaterial != null)
            {
                if (sharedMaterial.HasProperty("_BaseColor"))
                {
                    return sharedMaterial.GetColor("_BaseColor");
                }

                if (sharedMaterial.HasProperty("_Color"))
                {
                    return sharedMaterial.GetColor("_Color");
                }
            }

            return Color.white;
        }

        private static void SetRendererColor(Renderer renderer, Color color)
        {
            if (renderer == null)
                return;

            if (renderer is SpriteRenderer spriteRenderer)
            {
                spriteRenderer.color = color;
                return;
            }

            Material sharedMaterial = renderer.sharedMaterial;

            if (sharedMaterial == null)
                return;

            Material material = renderer.material;

            if (sharedMaterial.HasProperty("_BaseColor"))
            {
                material.SetColor("_BaseColor", color);
            }

            if (sharedMaterial.HasProperty("_Color"))
            {
                material.SetColor("_Color", color);
            }
        }
    }
}