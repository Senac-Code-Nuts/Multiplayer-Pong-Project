using System.Collections.Generic;
using UnityEngine;
using Pong.Gameplay.Actors;
using Pong.Gameplay.Enemy;
using Pong.Gameplay.Player;
using Pong.Systems.Graph;

namespace Pong.Gameplay.Boss
{
    public abstract class BossActor : Actor
    {
        [Header("Boss")]
        [SerializeField, Range(0, 3)] protected int _phase;

        protected float _stateTime;
        protected IBossState _currentState;
        protected List<PlayerController> _activePlayers = new List<PlayerController>();
        protected InfluenceSystem _influenceSystem;
        protected bool _isInitialized;

        public int Phase => _phase;
        protected bool IsInitialized => _isInitialized;

        public virtual void InitializeAI(List<PlayerController> activePlayers, InfluenceSystem influenceSystem)
        {
            _activePlayers = activePlayers ?? new List<PlayerController>();
            _influenceSystem = influenceSystem;
            _isInitialized = true;
            OnAIInitialized();
        }

        protected virtual void Update()
        {
            if (_currentState == null) return;

            _stateTime += Time.deltaTime;
            _currentState.OnUpdate();
        }

        protected virtual void ChangeState(IBossState newState)
        {
            _currentState?.OnExit();

            _currentState = newState;
            _stateTime = 0f;

            _currentState?.OnEnter();
        }

        protected virtual void SetPhase(int phase)
        {
            _phase = phase;
        }

        protected virtual void OnAIInitialized()
        {
        }

        public abstract void ExecuteAttack();
        protected virtual void AdvancePhase()
        {
            _phase++;
        }

        protected override void OnDamageTaken()
        {
            Debug.Log($"{gameObject.name} boss took damage.");
        }

        protected override void OnDeath()
        {
            Debug.Log($"{gameObject.name} boss died.");
        }
    }
}