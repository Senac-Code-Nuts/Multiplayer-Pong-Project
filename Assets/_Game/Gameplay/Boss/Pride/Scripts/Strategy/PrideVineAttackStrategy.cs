using Pong.Framework.BehaviourTree;
using UnityEngine;

namespace Pong.Gameplay.Boss
{
    public class PrideVineAttackStrategy : INodeStrategy
    {
        private readonly PrideBoss _boss;
        private float _timer;
        private State _state;
        private bool _telegraphStarted;
        private Transform _target;

        private enum State
        {
            Telegraph,
            Vulnerable,
            Recovery
        }

        public PrideVineAttackStrategy(PrideBoss boss)
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

                case State.Vulnerable:
                    return HandleVulnerable();

                case State.Recovery:
                    return HandleRecovery();
            }

            return Node.Status.Failure;
        }

        private Node.Status HandleTelegraph()
        {
            if (!_telegraphStarted)
            {
                _boss.StopMovement();
                _boss.SetVulnerable(false);
                _target = _boss.ChooseAttackTarget();
                _telegraphStarted = true;

                _boss.ShowTelegraphVisual();
                Debug.Log("<color=yellow>[Pride] Telegraph: Vine Attack</color>");
            }

            _boss.RotateTowardsTarget(_target);

            if (_timer >= _boss.TelegraphTime)
            {
                if (!_boss.IsFacingTarget(_target, 4f))
                    return Node.Status.Running;

                _boss.ShowExecuteVisual();
                _boss.ExecuteVineAttack(_target);
                _boss.SetVulnerable(true);
                _boss.ShowVulnerableVisual();

                _timer = 0f;
                _state = State.Vulnerable;
            }

            return Node.Status.Running;
        }

        private Node.Status HandleVulnerable()
        {
            if (_timer >= _boss.VulnerableTime)
            {
                _timer = 0f;
                _state = State.Recovery;
            }

            return Node.Status.Running;
        }

        private Node.Status HandleRecovery()
        {
            if (_timer >= _boss.RecoveryTime)
            {
                _boss.EndAttack();
                Reset();
                return Node.Status.Success;
            }

            return Node.Status.Running;
        }

        public void Reset()
        {
            _state = State.Telegraph;
            _timer = 0f;
            _telegraphStarted = false;
            _target = null;
        }
    }
}