using Pong.Gameplay.Boss;
using Pong.Gameplay.Enemy;
using Pong.Framework.BehaviourTree;
using UnityEngine;

namespace Pong.Gameplay.Boss
{
    public class WrathSpinAttackStrategy : EnemyTimerStrategyBase
    {
        private WrathBoss _boss;

        private float _timer = 0f;

        private enum AttackState 
        {
            Telegraph,
            Recovery,
            MovingToPlayer
        
        }
        private AttackState _currentState;

        public WrathSpinAttackStrategy(WrathBoss boss) : base(2f)
        {
            _boss = boss;
            Reset();
        }

        public override Node.Status Process()
        {
            _timer += Time.deltaTime;
            var target = GetClosestPlayer();

            if(target == null) return Node.Status.Failure;

            switch(_currentState)
            {
                case AttackState.Telegraph:
                    return HandleTelegraph(target);

                case AttackState.Recovery:
                    return HandleRecovery();

                case AttackState.MovingToPlayer:
                    return HandleMove(target);

            }   

            return Node.Status.Failure;
        }

        private Node.Status HandleTelegraph(Transform target)
        {
            _boss.StopMovement();
            _boss.SetVulnerable(false);
            _boss.RotateTowards(target.transform.position);
            _boss.ShowAttackArea();

            if(_timer >= _boss.TelegraphTime)
            {
                _boss.ExecuteAttack();
                _currentState = AttackState.Recovery;
                _timer = 0f;
            }

            return Node.Status.Running;

        }

        private Node.Status HandleRecovery()
        {
            if(_timer >= _boss.RecoveryTime)
            {
                _boss.EndAttack();
                _boss.SetVulnerable(true);
                _boss.AllowMovement();

                Reset();
                return Node.Status.Success;
            }
            return Node.Status.Running;

        }

        private Node.Status HandleMove(Transform target)
        {
            float distance = Vector3.Distance(_boss.transform.position, target.transform.position);
            if(distance > _boss.ChaseDistance)
            {
                _boss.AllowMovement();
                _boss.MoveTo(target.transform.position);
                _boss.RotateTowards(target.transform.position);
                return Node.Status.Running;
            }

            _boss.StopMovement();
            _timer = 0f;
            _currentState = AttackState.Telegraph;
            return Node.Status.Running;
        }

        private Transform GetClosestPlayer()
        {
            GameObject[] players = GameObject.FindGameObjectsWithTag("Player");

            Transform closest = null;
            float minDistance = Mathf.Infinity;

            foreach(var player in players)
            {
                float distance = Vector3.Distance(_boss.transform.position, player.transform.position);
                if(distance < minDistance)
                {
                    minDistance = distance;
                    closest = player.transform;
                }
            }

            return closest;
        }

        public override void Reset()
        {
           _currentState = AttackState.MovingToPlayer;
           _timer = 0f; 
        }

        protected override Node.Status WaitingStatus => Node.Status.Running;

        protected override void OnTimerStarted()
        {
            _boss.SetVulnerable(false);
            _boss.StopMovement();
            _boss.ShowAttackArea();
        }

        protected override void OnTimerCompleted()
        {
            _boss.ExecuteAttack();
            _boss.SetVulnerable(true);
        }
    }
}
