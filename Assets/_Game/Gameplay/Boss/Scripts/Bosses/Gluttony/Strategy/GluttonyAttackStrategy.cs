using Pong.Framework.BehaviourTree;
using UnityEngine;

namespace Pong.Gameplay.Boss
{
    public class GluttonyAttackStrategy : INodeStrategy
    {
        private readonly GluttonyBoss _boss;

        private float _timer;
        private float _timeSinceLastAttack;
        private bool _justFinished;

        private int _currentAttackIndex = -1;
        private AttackPhase _currentPhase;

        private enum AttackPhase
        {
            Waiting,
            Telegraph,
            Executing,
            Cooldown
        }

        public bool JustFinished => _justFinished;
        public bool IsInTelegraph => _currentPhase == AttackPhase.Telegraph;
        public float TelegraphProgress => _currentPhase == AttackPhase.Telegraph
            ? Mathf.Clamp01(_timer / Mathf.Max(0.001f, _boss.PreAttackTime))
            : 0f;

        public int CurrentAttackIndex => _currentAttackIndex;

        public GluttonyAttackStrategy(GluttonyBoss boss)
        {
            _boss = boss;
            Reset();
        }

        public Node.Status Process()
        {
            if (_boss == null)
                return Node.Status.Failure;

            _justFinished = false;
            _timer += Time.deltaTime;
            _timeSinceLastAttack += Time.deltaTime;

            switch (_currentPhase)
            {
                case AttackPhase.Waiting:
                    if (_timeSinceLastAttack >= _boss.TimeBetweenAttacks)
                    {
                        _currentAttackIndex = _boss.GetNextAttackIndex();
                        _currentPhase = AttackPhase.Telegraph;
                        _timer = 0f;
                        _timeSinceLastAttack = 0f;
                        _boss.OnAttackTelegraphStarted(_currentAttackIndex);
                        return Node.Status.Running;
                    }

                    return Node.Status.Failure;

                case AttackPhase.Telegraph:
                    if (_timer >= _boss.PreAttackTime)
                    {
                        _currentPhase = AttackPhase.Executing;
                        _timer = 0f;
                    }

                    return Node.Status.Running;

                case AttackPhase.Executing:
                    _boss.ExecuteAttackByIndex(_currentAttackIndex);
                    _currentPhase = AttackPhase.Cooldown;
                    _timer = 0f;
                    return Node.Status.Running;

                case AttackPhase.Cooldown:
                    if (_timer >= _boss.AttackCooldown)
                    {
                        _boss.OnAttackFinished();
                        _justFinished = true;
                        _currentPhase = AttackPhase.Waiting;
                        _timeSinceLastAttack = 0f;
                        _timer = 0f;
                        _currentAttackIndex = -1;
                        return Node.Status.Failure;
                    }

                    return Node.Status.Running;
            }

            return Node.Status.Failure;
        }

        public void Reset()
        {
            _currentPhase = AttackPhase.Waiting;
            _timeSinceLastAttack = 0f;
            _timer = 0f;
            _justFinished = false;
            _currentAttackIndex = -1;
        }
    }
}