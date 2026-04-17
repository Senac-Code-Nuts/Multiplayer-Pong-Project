using System.Collections.Generic;
using Pong.Systems.Audio;
using UnityEngine;
using Pong.Gameplay.Actors;
using Pong.Gameplay.Player;
using Pong.Systems.Graph;

namespace Pong.Gameplay.Enemy
{
    [RequireComponent(typeof(Rigidbody))]
    public abstract class EnemyActor : Actor
    {
        protected const string HurtSfxPath = "SFX/SFX_Enemy_Attack.wav";
        protected const string AttackSfxPath = "SFX/SFX_Enemy_Attack.wav";

        private Rigidbody _rigidBody;

        protected override void Awake()
        {
            base.Awake();

            _rigidBody = GetComponent<Rigidbody>();
        }

        public abstract void InitializeAI(List<PlayerController> activePlayers, InfluenceSystem influenceSystem);
        public abstract void ExecuteAttack();

        protected override void OnDamageTaken()
        {
            PlayHurtSfx();
            Debug.Log($"{gameObject.name} enemy took damage. Health: {_currentHealth}/{_maxHealth}");
        }

        protected override void OnDeath()
        {
            StopAllCoroutines();
            SetVulnerable(false);
            gameObject.SetActive(false);
        }

        public void ResetVelocity()
        {
            _rigidBody.linearVelocity = Vector3.zero; 
        }

        protected void PlayHurtSfx()
        {
            if (AudioManager.Instance != null)
            {
                AudioManager.Instance.PlaySFX(HurtSfxPath);
            }
        }

        protected void PlayAttackSfx()
        {
            if (AudioManager.Instance != null)
            {
                AudioManager.Instance.PlaySFX(AttackSfxPath);
            }
        }
    }
}