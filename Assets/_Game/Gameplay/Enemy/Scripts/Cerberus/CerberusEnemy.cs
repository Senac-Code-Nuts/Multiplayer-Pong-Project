using System.Collections.Generic;
using UnityEngine;
using Pong.Framework.BehaviourTree;
using Pong.Gameplay.Player;
using Pong.Systems.Graph;
using Pong.Core.Gizmo;
using Pong.Systems.Audio;

namespace Pong.Gameplay.Enemy.Cerberus
{
    public class CerberusEnemy : EnemyActor
    {
        [Header("Movement")]
        [SerializeField] private float _patrolSpeed = 3f;
        public float PatrolSpeed => _patrolSpeed;

        [Header("Pathfinding")]
        [SerializeField] private GraphComponent _graphComponent;

        [Header("Specific Attributes")]
        [SerializeField, Range(0.1f, 10f)] private float _timeBetweenAttacks = 3f;
        [SerializeField, Range(0.1f, 5f)] private float _preAttackTime = 0.5f;
        [SerializeField, Range(0.1f, 10f)] private float _attackCooldown = 2f;
        [SerializeField, Range(1f, 50f)] private float _projectileSpeed = 12f;
        [SerializeField, Range(1, 32)] private int _projectileCount = 7;
        [SerializeField, Range(0f, 180f)] private float _coneAngle = 120f;

        [Header("Components")]
        [SerializeField] private Transform _graphCenter;
        [SerializeField] private Transform _projectileSpawnPoint;
        [SerializeField] private CerberusShot _shotPrefab;
        [SerializeField] private Renderer _renderer;

        [Header("Audio Settings")]
        [field: SerializeField] public AudioClip AttackClip {get; private set;}
        [field: SerializeField] public AudioClip HurtClip {get; private set;}

        private Color _defaultColor;
        private BehaviourTree _tree;
        private CerberusAttackStrategy _attackStrategy;
        private CerberusPatrolStrategy _patrolStrategy;
        private List<PlayerController> _activePlayers;
        private InfluenceSystem _influenceSystem;
        private bool _isAIActive;

        public float TimeBetweenAttacks => _timeBetweenAttacks;
        public float PreAttackTime => _preAttackTime;
        public float AttackCooldown => _attackCooldown;

        protected override void Awake()
        {
            base.Awake();
            _renderer = GetComponent<Renderer>();
            if (_renderer != null)
            {
                _defaultColor = _renderer.material.color;
            }
        }

        public override void InitializeAI(List<PlayerController> activePlayers, InfluenceSystem influenceSystem)
        {
            _activePlayers = activePlayers ?? new List<PlayerController>();
            _influenceSystem = influenceSystem;

            _graphComponent = _influenceSystem.GraphComponent;

            _tree = new BehaviourTree("CerberusTree");

            var pathFinder = new EnemyPathFinder(_graphComponent);
            _patrolStrategy = new CerberusPatrolStrategy(this, pathFinder);
            _attackStrategy = new CerberusAttackStrategy(this);

            var attackNode = new Leaf("Attack", _attackStrategy, priority: 10);
            var patrolNode = new Leaf("Patrol", _patrolStrategy, priority: 1);

            var prioritySelector = new PrioritySelector("AttackOrPatrol");
            prioritySelector.AddChild(attackNode);
            prioritySelector.AddChild(patrolNode);

            _tree.AddChild(prioritySelector);
            _isAIActive = true;
        }

        private void Update()
        {
            if (!_isAIActive || _tree == null)
            {
                return;
            }

            _tree.Process();

            if (_attackStrategy != null && _attackStrategy.JustFinished && _patrolStrategy != null)
            {
                _patrolStrategy.Reset();
                Debug.Log("<color=cyan>[Cerberus] Voltando a patrulhar após ataque!</color>");
            }
        }

        public override void ApplyDamage(int damage)
        {
            if(HurtClip != null)
            {
                AudioManager.Instance.PlaySFX(HurtClip);
            }
            base.ApplyDamage(damage);
        }

        private void OnDisable()
        {
            if (_renderer != null)
            {
                _renderer.material.color = _defaultColor;
            }
        }
        public override void ExecuteAttack()
        {
            if (_shotPrefab == null) return;

            Transform spawnPoint = _projectileSpawnPoint != null ? _projectileSpawnPoint : transform;
            Vector3 forward = _graphCenter != null ? (_graphCenter.position - transform.position) : transform.forward;
            forward.y = 0f;

            if (forward.sqrMagnitude < 0.0001f)
            {
                forward = transform.forward;
            }

            forward.Normalize();

            float angleStep = _projectileCount > 1 ? _coneAngle / (_projectileCount - 1) : 0f;
            float startAngle = -_coneAngle * 0.5f;

            if(AttackClip != null)
            {
                AudioManager.Instance.PlaySFX(AttackClip);
            }

            for (int i = 0; i < _projectileCount; i++)
            {
                float currentAngle = startAngle + angleStep * i;
                Vector3 shotDirection = Quaternion.Euler(0f, currentAngle, 0f) * forward;

                CerberusShot shot = Instantiate(
                    _shotPrefab,
                    spawnPoint.position,
                    Quaternion.LookRotation(shotDirection, Vector3.up)
                );

                shot.Initialize(shotDirection, _projectileSpeed);
            }

            _renderer.material.color = _defaultColor;
        }

        public void FaceGraphCenter()
        {
            if (_graphCenter == null) return;

            Vector3 lookDirection = _graphCenter.position - transform.position;
            lookDirection.y = 0f;

            if (lookDirection.sqrMagnitude < 0.0001f) return;

            transform.rotation = Quaternion.LookRotation(lookDirection.normalized, Vector3.up);
        }

        public void ExecutePreAttack()
        {
            _renderer.material.color = Color.yellow;
        }

        #region Gizmos
        private GizmoData GetGizmoData()
        {
            if (_attackStrategy == null)
                return new GizmoData { IsInTelegraph = false };

            return new GizmoData
            {
                IsInTelegraph = _attackStrategy.IsInTelegraph,
                TelegraphProgress = _attackStrategy.TelegraphProgress,
                ConeAngle = _coneAngle,
                ProjectileCount = _projectileCount,
                SpawnPosition = _projectileSpawnPoint != null ? _projectileSpawnPoint.position : transform.position,
                Direction = _graphCenter != null ? (_graphCenter.position - transform.position).normalized : transform.forward
            };
        }

        private void OnDrawGizmos()
        {
            if (_attackStrategy == null) return;

            var gizmoData = GetGizmoData();

            if (!gizmoData.IsInTelegraph) return;

            Color coneColor = Color.Lerp(Color.yellow, Color.red, gizmoData.TelegraphProgress);
            Gizmos.color = coneColor;

            Vector3 spawnPos = gizmoData.SpawnPosition;
            Vector3 forward = gizmoData.Direction.normalized;
            float halfConeAngle = gizmoData.ConeAngle * 0.5f;
            float coneDistance = 10f;

            Gizmos.DrawLine(spawnPos, spawnPos + forward * coneDistance);

            Vector3 leftDirection = Quaternion.Euler(0f, -halfConeAngle, 0f) * forward;
            Vector3 rightDirection = Quaternion.Euler(0f, halfConeAngle, 0f) * forward;

            Gizmos.DrawLine(spawnPos, spawnPos + leftDirection * coneDistance);
            Gizmos.DrawLine(spawnPos, spawnPos + rightDirection * coneDistance);

            int arcSegments = 16;
            Vector3 prevPoint = spawnPos + leftDirection * coneDistance;

            for (int i = 1; i <= arcSegments; i++)
            {
                float angle = Mathf.Lerp(-halfConeAngle, halfConeAngle, (float)i / arcSegments);
                Vector3 direction = Quaternion.Euler(0f, angle, 0f) * forward;
                Vector3 newPoint = spawnPos + direction * coneDistance;

                Gizmos.DrawLine(prevPoint, newPoint);
                prevPoint = newPoint;
            }

            GizmoDrawer.DrawPoint(spawnPos, 0.2f, Color.red);
        }

        private struct GizmoData
        {
            public bool IsInTelegraph;
            public float TelegraphProgress;
            public float ConeAngle;
            public int ProjectileCount;
            public Vector3 SpawnPosition;
            public Vector3 Direction;
        }
        #endregion
    }
}