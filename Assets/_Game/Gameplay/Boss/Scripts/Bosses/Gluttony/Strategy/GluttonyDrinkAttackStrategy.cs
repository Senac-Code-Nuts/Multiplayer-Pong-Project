using Pong.Framework.BehaviourTree;
using UnityEngine;

namespace Pong.Gameplay.Boss
{
    public class GluttonyDrinkAttackStrategy : INodeStrategy
    {
        private readonly GluttonyBoss _boss;
        private float _timer;
        private State _state;
        private bool _telegraphStarted;
        private bool _directionLocked;
        private Transform _target;

        private enum State
        {
            Telegraph,
            Recovery
        }

        public GluttonyDrinkAttackStrategy(GluttonyBoss boss)
        {
            _boss = boss;
            Reset();
        }

        public Node.Status Process()
        {
            _timer += Time.deltaTime;

            switch (_state)
            {
                case State.Telegraph:
                    return HandleTelegraph();

                case State.Recovery:
                    return HandleRecovery();
            }

            return Node.Status.Failure;
        }

        private Node.Status HandleTelegraph()
        {
            if (!_telegraphStarted)
            {
                _target = _boss.ChooseAttackTarget();
                Vector3 direction = _boss.GetDirectionToTarget(_target, _boss.transform.forward);

                _boss.ShowDrinkTelegraph();
                _boss.BeginDrinkTelegraph(direction);

                _telegraphStarted = true;
                _directionLocked = false;
            }

            float progress = Mathf.Clamp01(_timer / Mathf.Max(0.001f, _boss.PreAttackTime));

            if (!_directionLocked)
            {
                _boss.RotateTowardsTarget(_target);
                _boss.UpdateDrinkTelegraphDirection(_target);

                if (progress >= 0.7f)
                {
                    _directionLocked = true;

                    Vector3 lockedDirection = _boss.GetDirectionToTarget(_target, _boss.transform.forward);
                    _boss.BeginDrinkTelegraph(lockedDirection);
                }
            }

            _boss.UpdateDrinkTelegraph(progress);

            if (_timer >= _boss.PreAttackTime)
            {
                _boss.ExecuteDrinkAttack();
                _timer = 0f;
                _state = State.Recovery;
            }

            return Node.Status.Running;
        }

        private Node.Status HandleRecovery()
        {
            if (_timer >= _boss.AttackRecoveryTime)
            {
                _boss.EndAttack();
                Reset();
                return Node.Status.Success;
            }

            return Node.Status.Running;
        }

        private void Reset()
        {
            _state = State.Telegraph;
            _timer = 0f;
            _telegraphStarted = false;
            _directionLocked = false;
            _target = null;
        }
    }
}