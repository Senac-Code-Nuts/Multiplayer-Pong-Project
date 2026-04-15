using System.Collections.Generic;
using UnityEngine;
using Pong.Gameplay.Actors;
using Pong.Gameplay.Player; // Adicionado para acessar o PlayerController

namespace Pong.Gameplay.Enemy
{
    [RequireComponent(typeof(Rigidbody))]
    public abstract class EnemyActor : Actor
    {
        private Rigidbody _rigidBody;

        protected override void Awake()
        {
            base.Awake();

            _rigidBody = GetComponent<Rigidbody>();
        }

        public abstract void InitializeAI(List<PlayerController> activePlayers);
        public abstract void ExecuteAttack();

        protected override void OnDamageTaken()
        {
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
    }
}