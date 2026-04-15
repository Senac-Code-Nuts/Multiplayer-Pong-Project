using Pong.Framework.BehaviourTree;
using UnityEngine;

namespace Pong.Gameplay.Boss
{
    public class GluttonyEatStrategy : INodeStrategy
    {
        private readonly GluttonyBoss _boss;
        private float _timer;
        private State _state;
        private bool _telegraphStarted;

        private enum State
        {
            Telegraph,
            Recovery
        }

        public GluttonyEatStrategy(GluttonyBoss boss)
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
                        _boss.ShowEatTelegraph();
                        _telegraphStarted = true;
                    }

                    if (_timer >= _boss.PreAttackTime)
                    {
                        _boss.ExecuteEat();
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
        }
    }
}