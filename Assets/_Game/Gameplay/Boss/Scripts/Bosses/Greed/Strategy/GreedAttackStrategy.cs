using Pong.Framework.BehaviourTree;
using Pong.Gameplay.Player;
using UnityEngine;

namespace Pong.Gameplay.Boss.Greed
{
    public class GreedAttackStrategy : INodeStrategy
    {
        private readonly GreedBoss _greedBoss;
        private float _telegraphTime = 1.0f;
        private float _recoveryTime = 1.5f;
        private float _attackRadius = 3.0f;
        private float _timer = 0f;
        private bool _justFinished = false;

        private enum AttackState { Telegraph, Recovery }
        private AttackState _currentState;

        public GreedAttackStrategy(GreedBoss boss)
        {
            _greedBoss = boss;
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
            _greedBoss.ExecuteAttack();
            Collider[] hitColliders = Physics.OverlapSphere(_greedBoss.transform.position, _attackRadius);
            foreach (var hit in hitColliders)
            {
                if (hit.GetComponentInParent<PlayerActor>() is PlayerActor player)
                {
                    Debug.Log($"<color=orange>[Attack] Jogador {player.name} tomou dano</color>");
                    player.ApplyDamage(_greedBoss.Damage);
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
        public void DrawGizmos(Vector3 bossPosition)
        {
            float progress = Mathf.Clamp01(_timer / _telegraphTime);
            if (_currentState == AttackState.Telegraph)
            {
                Gizmos.color = new Color(1, 1, 0, 0.5f); 
                Gizmos.DrawSphere(bossPosition, _attackRadius * progress);

                // Aro externo fixo
                Gizmos.color = Color.yellow;
                Gizmos.DrawWireSphere(bossPosition, _attackRadius);
            }
            else if (_currentState == AttackState.Recovery)
            {
                Gizmos.color = Color.red;
                Gizmos.DrawWireSphere(bossPosition, _attackRadius);
            }
        }
        #endregion
    }
}