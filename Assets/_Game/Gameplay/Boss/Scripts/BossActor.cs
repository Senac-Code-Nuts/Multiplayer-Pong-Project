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
        private bool _initializationFailed;

        public int Phase => _phase;
        protected bool IsInitialized => _isInitialized;
        public bool IsAIInitialized => _isInitialized;

        public virtual void InitializeAI(List<PlayerController> activePlayers, InfluenceSystem influenceSystem)
        {
            _activePlayers = activePlayers ?? new List<PlayerController>();
            _influenceSystem = influenceSystem;
            _isInitialized = false;
            _initializationFailed = false;

            if (!ValidateAISetup())
            {
                return;
            }

            OnAIInitialized();

            if (_initializationFailed)
            {
                return;
            }

            _isInitialized = true;
        }

        protected virtual bool ValidateAISetup()
        {
            return true;
        }

        protected bool TryResolveGraphComponent(ref GraphComponent graphComponent)
        {
            if (graphComponent == null && _influenceSystem != null)
            {
                graphComponent = _influenceSystem.GraphComponent;
            }

            return graphComponent != null;
        }

        protected bool RequireReference(UnityEngine.Object reference, string warningMessage)
        {
            if (reference != null)
            {
                return true;
            }

            Debug.LogWarning($"[{GetType().Name}] {warningMessage}");
            return false;
        }

        protected void FailAIInitialization(string warningMessage)
        {
            _initializationFailed = true;
            Debug.LogWarning($"[{GetType().Name}] {warningMessage}");
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
        }

        protected override void OnDeath()
        {
        }
    }
}