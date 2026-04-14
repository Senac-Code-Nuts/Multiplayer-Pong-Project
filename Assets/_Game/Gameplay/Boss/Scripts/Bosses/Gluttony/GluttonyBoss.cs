using Pong.Framework.BehaviourTree;
using Pong.Gameplay.Enemy;
using Pong.Systems.Graph;
using UnityEngine;
using UnityEngine.VFX;

namespace Pong.Gameplay.Boss
{
    public class GluttonyBoss : BossActor
    {
        [Header("Movement")]
        [SerializeField] private float _patrolSpeed = 3f;
        [SerializeField] private GraphComponent _graphComponent;

        [Header("Attack Timings")]
        [SerializeField, Range(0.1f, 10f)] private float _timeBetweenAttacks;
        [SerializeField, Range(0.1f, 5f)] private float _preAttackTime;
        [SerializeField, Range(0.1f, 10f)] private float _attackCooldown;

        [Header("Projectile Attack - Spit Bones")]
        [SerializeField] private Transform _projectileSpawnPoint;
        [SerializeField] private GluttonyProjectile _boneProjectilePrefab;
        [SerializeField, Range(1f, 50f)] private float _projectileSpeed;
        [SerializeField, Range(1, 8)] private int _projectileCount;
        [SerializeField, Range(0f, 90f)] private float _projectileSpreadAngle;
        [SerializeField, Range(1, 10)] private int _projectileDamage;
        [SerializeField, Range(1, 32)] private int _poolSize;
        [SerializeField] private Transform _projectilePoolParent;

        [Header("Cone Attack - Throw Drink")]
        [SerializeField, Range(1, 10)] private int _drinkDamage = 1;
        [SerializeField] private GameObject _drinkConeTelegraph;
        [SerializeField, Range(0.5f, 10f)] private float _coneRadius;
        [SerializeField, Range(1f, 180f)] private float _coneAngle;
        [SerializeField] private LayerMask _playerLayerMask = ~0;

        [Header("VFX")]
        [SerializeField] private VisualEffect _drinkVfx;
        [SerializeField] private Transform _drinkVfxSpawnPoint;
        [SerializeField, Range(0.1f, 2f)] private float _drinkVfxStopDelay = 0.4f;

        [Header("Attack Helpers")]
        [SerializeField] private GluttonyConeAttack _coneAttack;

        [Header("Visual Debug")]
        [SerializeField] private Renderer _renderer;
        [SerializeField] private Color _defaultColor = Color.white;
        [SerializeField] private Color _telegraphColor = Color.yellow;
        [SerializeField] private Color _executeColor = Color.red;

        private BehaviourTree _tree;
        private GluttonyAttackStrategy _attackStrategy;
        //private GluttonyPatrolStrategy _patrolStrategy;
        private GluttonyProjectilePool _boneProjectilePool;
        private Vector3 _lockedConeDirection;

        public float PatrolSpeed => _patrolSpeed;

        private int _lastAttackIndex = -1;
        private int _repeatCount = 0;

        public float TimeBetweenAttacks => _timeBetweenAttacks;
        public float PreAttackTime => _preAttackTime;
        public float AttackCooldown => _attackCooldown;

        public bool IsInTelegraph => _attackStrategy != null && _attackStrategy.IsInTelegraph;
        public float TelegraphProgress => _attackStrategy != null ? _attackStrategy.TelegraphProgress : 0f;
        public int CurrentAttackIndex => _attackStrategy != null ? _attackStrategy.CurrentAttackIndex : -1;

        protected override void Awake()
        {
            base.Awake();

            if (_renderer == null)
            {
                _renderer = GetComponent<Renderer>();
            }

            if (_renderer != null)
            {
                _defaultColor = _renderer.material.color;
            }

            if (_boneProjectilePrefab != null)
            {
                _boneProjectilePool = new GluttonyProjectilePool(
                    _boneProjectilePrefab,
                    _poolSize,
                    _projectilePoolParent
                );
            }

            if (_drinkConeTelegraph != null)
            {
                _drinkConeTelegraph.SetActive(false);
            }

            _tree = new BehaviourTree("GluttonyTree");

            var pathFinder = new EnemyPathFinder(_graphComponent);
            //_patrolStrategy = new GluttonyPatrolStrategy(this, pathFinder);
            _attackStrategy = new GluttonyAttackStrategy(this);

            var attackNode = new Leaf("Attack", _attackStrategy, priority: 10);
            //var patrolNode = new Leaf("Patrol", _patrolStrategy, priority: 1);

            var selector = new PrioritySelector("AttackOrPatrol");
            selector.AddChild(attackNode);
            //selector.AddChild(patrolNode);

            _tree.AddChild(selector);
        }

        protected override void Update()
        {
            base.Update();

            if (_isDead || _isStunned)
                return;

            _tree?.Process();

            UpdateDrinkAttack();
        }

        public int GetNextAttackIndex()
        {
            int next = Random.Range(0, 3);

            if (next == _lastAttackIndex)
            {
                _repeatCount++;
            }
            else
            {
                _repeatCount = 1;
            }

            if (_repeatCount >= 3)
            {
                next = (next + 1) % 3;
                _repeatCount = 1;
            }

            _lastAttackIndex = next;
            return next;
        }

        public void OnAttackTelegraphStarted(int attackIndex)
        {
            SetTelegraphVisual();

            if (attackIndex == 2)
            {
                BeginDrinkTelegraph();
            }

            switch (attackIndex)
            {
                case 0:
                    Debug.Log("<color=yellow>[Gluttony] Telegraph: Empanturrar</color>");
                    break;
                case 1:
                    Debug.Log("<color=yellow>[Gluttony] Telegraph: Cuspir Ossos</color>");
                    break;
                case 2:
                    Debug.Log("<color=yellow>[Gluttony] Telegraph: Arremessar Bebida</color>");
                    break;
                default:
                    Debug.LogWarning("[Gluttony] Telegraph com attackIndex inválido.");
                    break;
            }
        }

        public void ExecuteAttackByIndex(int attackIndex)
        {
            SetExecuteVisual();

            switch (attackIndex)
            {
                case 0:
                    ExecuteEat();
                    break;

                case 1:
                    ExecuteSpitBones();
                    break;

                case 2:
                    ExecuteDrinkAttack();
                    break;

                default:
                    Debug.LogWarning("[Gluttony] ExecuteAttackByIndex recebeu attackIndex inválido.");
                    break;
            }
        }

        public void OnAttackFinished()
        {
            ResetVisual();

            if (_coneAttack != null)
            {
                _coneAttack.EndTelegraph(_drinkConeTelegraph);
            }
        }

        public void ExecuteEat()
        {
            Debug.Log("<color=orange>[Gluttony] Empanturrou.</color>");
        }

        public void ExecuteSpitBones()
        {
            Debug.Log("<color=orange>[Gluttony] Cuspiu ossos.</color>");

            if (_boneProjectilePool == null)
            {
                Debug.LogWarning("[Gluttony] Pool de projétil não foi inicializado.");
                return;
            }

            Transform spawnPoint = _projectileSpawnPoint != null ? _projectileSpawnPoint : transform;

            if (_projectileCount <= 1)
            {
                SpawnBoneProjectile(spawnPoint.position, transform.forward);
                return;
            }

            float angleStep = _projectileSpreadAngle / (_projectileCount - 1);
            float startAngle = -_projectileSpreadAngle * 0.5f;

            for (int i = 0; i < _projectileCount; i++)
            {
                float currentAngle = startAngle + angleStep * i;
                Vector3 direction = Quaternion.Euler(0f, currentAngle, 0f) * transform.forward;
                SpawnBoneProjectile(spawnPoint.position, direction.normalized);
            }
        }

        private void ExecuteDrinkAttack()
        {
            if (_coneAttack == null)
                return;

            _coneAttack.Execute(
                transform,
                _drinkDamage,
                _coneRadius,
                _coneAngle,
                _playerLayerMask,
                _drinkConeTelegraph,
                _drinkVfx,
                _drinkVfxSpawnPoint,
                _drinkVfxStopDelay
            );
        }

        private void UpdateDrinkAttack()
        {
            if (_coneAttack == null || _attackStrategy == null)
                return;

            if (!_attackStrategy.IsInTelegraph || _attackStrategy.CurrentAttackIndex != 2)
                return;

            _coneAttack.UpdateTelegraph(
                _attackStrategy.TelegraphProgress,
                _coneRadius,
                _coneAngle,
                _drinkConeTelegraph
            );
        }

        private void BeginDrinkTelegraph()
        {
            _lockedConeDirection = transform.forward;

            _coneAttack.BeginTelegraph(
                _lockedConeDirection,
                _drinkConeTelegraph
            );
        }

        private void SpawnBoneProjectile(Vector3 position, Vector3 direction)
        {
            GluttonyProjectile projectile = _boneProjectilePool.Get();
            if (projectile == null)
            {
                Debug.LogWarning("[Gluttony] Pool vazio.");
                return;
            }

            projectile.transform.SetPositionAndRotation(
                position,
                Quaternion.LookRotation(direction, Vector3.up)
            );

            projectile.Initialize(
                direction,
                _projectileSpeed,
                _projectileDamage,
                _boneProjectilePool,
                transform
            );
        }

        private void SetTelegraphVisual()
        {
            if (_renderer == null) return;
            _renderer.material.color = _telegraphColor;
        }

        private void SetExecuteVisual()
        {
            if (_renderer == null) return;
            _renderer.material.color = _executeColor;
        }

        private void ResetVisual()
        {
            if (_renderer == null) return;
            _renderer.material.color = _defaultColor;
        }

        public override void ExecuteAttack()
        {
            Debug.Log("[Gluttony] ExecuteAttack() base chamado.");
        }

        protected override void OnDeath()
        {
            base.OnDeath();
            ResetVisual();
        }

        private void OnDisable()
        {
            ResetVisual();
        }

        private void OnDrawGizmos()
        {
            if (!Application.isPlaying) return;
            if (_attackStrategy == null) return;
            if (!_attackStrategy.IsInTelegraph) return;

            switch (_attackStrategy.CurrentAttackIndex)
            {
                case 0:
                    DrawEatGizmo();
                    break;

                case 1:
                    DrawSpitBonesGizmo();
                    break;
            }
        }

        private void DrawEatGizmo()
        {
            Gizmos.color = Color.Lerp(Color.yellow, Color.red, TelegraphProgress);
            Gizmos.DrawWireSphere(transform.position, 1.25f);
        }

        private void DrawSpitBonesGizmo()
        {
            Gizmos.color = Color.Lerp(Color.yellow, Color.red, TelegraphProgress);

            Vector3 origin = _projectileSpawnPoint != null ? _projectileSpawnPoint.position : transform.position;

            if (_projectileCount <= 1)
            {
                Gizmos.DrawLine(origin, origin + transform.forward * 4f);
                return;
            }

            float angleStep = _projectileSpreadAngle / (_projectileCount - 1);
            float startAngle = -_projectileSpreadAngle * 0.5f;

            for (int i = 0; i < _projectileCount; i++)
            {
                float currentAngle = startAngle + angleStep * i;
                Vector3 direction = Quaternion.Euler(0f, currentAngle, 0f) * transform.forward;
                Gizmos.DrawLine(origin, origin + direction.normalized * 4f);
            }
        }
    }
}