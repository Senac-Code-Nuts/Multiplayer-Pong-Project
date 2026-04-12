using Pong.Framework.BehaviourTree;
using UnityEngine;

namespace Pong.Gameplay.Enemy
{
    public abstract class EnemyTimerStrategyBase : INodeStrategy
    {
        private readonly float _duration;
        private float _timer;
        private bool _isRunning;

        protected EnemyTimerStrategyBase(float duration)
        {
            _duration = Mathf.Max(0f, duration);
            _timer = 0f;
            _isRunning = false;
        }

        protected float ElapsedTime => _timer;
        protected float Duration => _duration;
        protected bool IsRunning => _isRunning;

        protected abstract Node.Status WaitingStatus { get; }

        protected virtual Node.Status CompletionStatus => Node.Status.Success;

        public virtual Node.Status Process()
        {
            if (!_isRunning)
            {
                _isRunning = true;
                _timer = 0f;
                OnTimerStarted();
            }

            OnTimerTick();
            _timer += Time.deltaTime;

            if (_timer >= _duration)
            {
                OnTimerCompleted();
                ResetTimerState();
                return CompletionStatus;
            }

            return WaitingStatus;
        }

        protected virtual void OnTimerStarted() {}
        protected virtual void OnTimerTick() {}
        protected virtual void OnTimerCompleted() {}

        public virtual void Reset()
        {
            ResetTimerState();
        }

        protected void ResetTimerState()
        {
            _timer = 0f;
            _isRunning = false;
        }
    }
}