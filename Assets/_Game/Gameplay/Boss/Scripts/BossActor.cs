using UnityEngine;
using Pong.Gameplay.Actors;
using Pong.Gameplay.Enemy;

namespace Pong.Gameplay.Boss
{
    public abstract class BossActor : Actor
    {
        [Header("Boss")]
        [SerializeField, Range(0, 3)] protected int _phase;

        protected float _stateTime;
        protected IBossState _currentState;

        public int Phase => _phase;

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

        protected virtual void AdvancePhase()
        {
            _phase++;
        }

        public abstract void ExecuteAttack();

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