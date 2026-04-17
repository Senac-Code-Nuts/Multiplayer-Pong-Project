using Pong.Framework.BehaviourTree;
using UnityEngine;

namespace Pong.Gameplay.Boss
{
    public class GluttonySpitBonesStrategy : INodeStrategy
    {
        private readonly GluttonyBoss _boss;
        private float _timer;
        private State _state;
        private bool _telegraphStarted;
        private Transform _target;

        private enum State
        {
            Telegraph,
            Execute,
            Recovery
        }

        public GluttonySpitBonesStrategy(GluttonyBoss boss)
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
                        _boss.ShowSpitBonesTelegraph();
                        _telegraphStarted = true;
                    }

                    _boss.RotateTowardsTarget(_target);

                    if (_timer >= _boss.PreAttackTime)
                    {
                        _boss.LockSpitBonesDirection(_target);
                        _boss.ExecuteSpitBones();
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