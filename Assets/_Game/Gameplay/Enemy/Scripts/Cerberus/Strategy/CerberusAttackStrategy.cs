using Pong.Framework.Strategy;
using Pong.Framework.BehaviourTree;
using UnityEngine;

namespace Pong.Gameplay.Enemy.Cerberus
{
    public class CerberusAttackStrategy : INodeStrategy
    {
        private readonly CerberusEnemy _enemy;
        private float _timer;
        private float _timeSinceLastAttack;
        private bool _justFinished = false;
        
        private enum AttackPhase { Waiting, Telegraph, Cooldown }
        private AttackPhase _currentPhase;

        public bool JustFinished => _justFinished;
        public bool IsInTelegraph => _currentPhase == AttackPhase.Telegraph;
        public float TelegraphProgress => _currentPhase == AttackPhase.Telegraph ? _timer / _enemy.PreAttackTime : 0f;

        public CerberusAttackStrategy(CerberusEnemy enemy)
        {
            _enemy = enemy;
            Reset();
        }

        public Node.Status Process()
        {
            _justFinished = false;
            _timer += Time.deltaTime;
            _timeSinceLastAttack += Time.deltaTime;

            switch (_currentPhase)
            {
                case AttackPhase.Waiting:
              
                    if (_timeSinceLastAttack >= _enemy.TimeBetweenAttacks)
                    {
                        _currentPhase = AttackPhase.Telegraph;
                        _timer = 0f;
                        _timeSinceLastAttack = 0f;
                    }
                    return Node.Status.Failure;

                case AttackPhase.Telegraph:
                    _enemy.FaceGraphCenter();
                    _enemy.ExecutePreAttack();

                    if (_timer >= _enemy.PreAttackTime)
                    {
                        _enemy.ExecuteAttack();
                        _currentPhase = AttackPhase.Cooldown;
                        _timer = 0f;
                    }
                    return Node.Status.Running;

                case AttackPhase.Cooldown:
                    if (_timer >= _enemy.AttackCooldown)
                    {
                        _justFinished = true;
                        _currentPhase = AttackPhase.Waiting;
                        _timeSinceLastAttack = 0f;
                        _timer = 0f;
                        return Node.Status.Failure; 
                    }
                    return Node.Status.Running;
            }

            return Node.Status.Failure;
        }

        public void Reset()
        {
            _currentPhase = AttackPhase.Waiting;
            _timer = 0f;
            _timeSinceLastAttack = 0f;
        }
    }
}
