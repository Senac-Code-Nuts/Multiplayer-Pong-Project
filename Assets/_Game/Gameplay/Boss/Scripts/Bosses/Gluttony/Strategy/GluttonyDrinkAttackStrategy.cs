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
                    if (!_telegraphStarted)
                    {
                        _target = _boss.ChooseAttackTarget();
                        Vector3 direction = _boss.GetDirectionToTarget(_target, _boss.transform.forward);
                        _boss.ShowDrinkTelegraph();
                        _boss.BeginDrinkTelegraph(direction);
                        _telegraphStarted = true;
                    }

                    _boss.RotateTowardsTarget(_target);
                    _boss.UpdateDrinkTelegraphDirection(_target);
                    _boss.UpdateDrinkTelegraph(
                        Mathf.Clamp01(_timer / Mathf.Max(0.001f, _boss.PreAttackTime))
                    );

                    if (_timer >= _boss.PreAttackTime)
                    {
                        if (!_boss.IsFacingTarget(_target, 4f))
                        {
                            return Node.Status.Running;
                        }

                        _boss.ExecuteDrinkAttack();
                        _timer = 0f;
                        _state = State.Recovery;
                    }

                    return Node.Status.Running;

                case State.Recovery:
                    if (_timer >= _boss.AttackRecoveryTime)
                    {
                        _boss.EndAttack();
                        Reset();
                        return Node.Status.Success;
                    }

                    return Node.Status.Running;
            }

            return Node.Status.Failure;
        }

        private void Reset()
        {
            _state = State.Telegraph;
            _timer = 0f;
            _telegraphStarted = false;
            _target = null;
        }
    }
}