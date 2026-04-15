using System.Collections;
using System.Collections.Generic;
using Pong.Gameplay.Enemy;
using Pong.Gameplay.Player;
using UnityEngine;

namespace Pong.Gameplay.Enemy.Test
{
    public class EnemyDummy : EnemyActor
    {
        [Header("Stun Visual")]
        [SerializeField] private Renderer[] _renderers;
        [SerializeField] private Color _normalColor = Color.white;
        [SerializeField] private Color _stunnedColor = Color.cyan;
        private List<PlayerController> _activePlayers;

        protected override void Awake()
        {
            base.Awake();
            _currentHealth = _maxHealth;

            CacheDefaultColor();
        }

        public override void InitializeAI(List<PlayerController> activePlayers)
        {
            _activePlayers = activePlayers ?? new List<PlayerController>();
        }

        public override void ExecuteAttack()
        {
        }

        public override void ApplyStun(float duration)
        {
            if (_isDead || _isStunned) return;

            base.ApplyStun(duration);
            StartCoroutine(StunVisualRoutine(duration));
        }

        private IEnumerator StunVisualRoutine(float duration)
        {
            float elapsedTime = 0f;
            bool toggle = false;

            while (elapsedTime < duration)
            {
                elapsedTime += 0.15f;
                toggle = !toggle;

                SetColor(toggle ? _stunnedColor : _normalColor);

                yield return new WaitForSeconds(0.15f);
            }

            SetColor(_normalColor);
        }

        private void CacheDefaultColor()
        {
            if (_renderers == null || _renderers.Length == 0) return;

            Renderer firstRenderer = _renderers[0];

            if (firstRenderer != null && firstRenderer.material.HasProperty("_Color"))
            {
                _normalColor = firstRenderer.material.color;
            }
        }

        private void SetColor(Color color)
        {
            if (_renderers == null || _renderers.Length == 0) return;

            foreach (Renderer currentRenderer in _renderers)
            {
                if (currentRenderer != null && currentRenderer.material.HasProperty("_Color"))
                {
                    currentRenderer.material.color = color;
                }
            }
        }

        protected override void OnDamageTaken()
        {
            Debug.Log($"[DUMMY] {gameObject.name} took damage. HP: {_currentHealth}");
        }

        protected override void OnDeath()
        {
            Debug.Log($"[DUMMY] {gameObject.name} died.");

            Destroy(gameObject);
        }
    }
}