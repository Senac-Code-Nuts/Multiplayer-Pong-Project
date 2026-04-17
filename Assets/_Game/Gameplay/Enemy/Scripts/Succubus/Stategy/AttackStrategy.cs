using Pong.Framework.BehaviourTree;
using Pong.Gameplay.Player;
using UnityEngine;

namespace Pong.Gameplay.Enemy.Succubus
{
    public class AttackStrategy : INodeStrategy
    {
        private readonly SuccubusEnemy _enemy;

        private float _telegraphTime = 1.0f;
        private float _recoveryTime = 1.5f;
        private float _attackRadius = 3.0f;

        private float _timer = 0f;
        private bool _justFinished = false;

        private enum AttackState { Telegraph, Recovery }
        private AttackState _currentState;

        public AttackStrategy(SuccubusEnemy enemy)
        {
            _enemy = enemy;
            Reset();
        }

        public Node.Status Process()
        {
            _justFinished = false;
            _timer += Time.deltaTime;

            switch (_currentState)
            {
                case AttackState.Telegraph:
                    if (_timer >= _telegraphTime)
                    {
                        ExecuteHit();
                        _currentState = AttackState.Recovery;
                        _timer = 0f;
                    }
                    return Node.Status.Running;

                case AttackState.Recovery:
                    if (_timer >= _recoveryTime)
                    {
                        _justFinished = true;
                        Reset();
                        return Node.Status.Success;
                    }
                    return Node.Status.Running;
            }

            return Node.Status.Failure;
        }

        public bool JustFinished => _justFinished;

        public bool IsInRecovery => _currentState == AttackState.Recovery;

        private void ExecuteHit()
        {
            Debug.Log("<color=red>[Attack] Succubus usou Ataque em Área!</color>");

            Collider[] hitColliders = Physics.OverlapSphere(_enemy.transform.position, _attackRadius);

            foreach (var hit in hitColliders)
            {
                if (hit.GetComponentInParent<PlayerActor>() is PlayerActor player)
                {
                    Debug.Log($"<color=orange>Acertou no jogador: {player.name}</color>");
                    player.ApplyDamage(_enemy.Damage);
                }
            }
        }

        public void Reset()
        {
            _currentState = AttackState.Telegraph;
            _timer = 0f;
            _justFinished = false;
        }

        #region Gizmos
        public struct GizmoData
        {
            public bool IsInTelegraph;
            public float ChargeProgress;
            public float CurrentRadius;
            public float MaxRadius;
        }

        public GizmoData GetGizmoData()
        {
            return new GizmoData
            {
                IsInTelegraph = _currentState == AttackState.Telegraph,
                ChargeProgress = Mathf.Clamp01(_timer / _telegraphTime),
                CurrentRadius = _attackRadius * Mathf.Clamp01(_timer / _telegraphTime),
                MaxRadius = _attackRadius
            };
        }
        #endregion
    }
}