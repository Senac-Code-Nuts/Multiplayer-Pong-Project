using Pong.Framework.BehaviourTree;
using Pong.Framework.Strategy;
using Pong.Gameplay.Enemy;
using Pong.Systems.Audio;
using Pong.Systems.Graph;
using UnityEngine;

namespace Pong.Gameplay.Boss
{
    public class PrideBoss : BossActor
    {
        [Header("Movement")]
        [SerializeField] private float _moveSpeed = 2f;
        [SerializeField] private GraphComponent _graphComponent;
        [SerializeField] private float _rotationSpeed = 180f;

        public float MoveSpeed => _moveSpeed;
        public bool CanMove { get; private set; } = true;

        [Header("Attack Settings")]
        [SerializeField] private float _attackCooldown = 2f;
        [field: SerializeField] public float TelegraphTime { get; private set; } = 1.5f;
        [field: SerializeField] public float VulnerableTime { get; private set; } = 10f;
        [field: SerializeField] public float RecoveryTime { get; private set; } = 0.5f;

        private float _cooldownTimer;
        public bool CanAttack => _cooldownTimer <= 0f;
        public bool IsAttacking { get; private set; }

        [Header("Trail Vine")]
        [SerializeField] private bool _useTrailVines = false;
        [SerializeField] private PrideTrailVine _trailVinePrefab;
        [SerializeField] private Transform _trailSpawnPoint;
        [SerializeField, Range(0.5f, 10f)] private float _trailSpawnInterval = 4f;
        [SerializeField, Range(1f, 10f)] private float _trailLifetime = 4f;
        [SerializeField, Range(0.5f, 15f)] private float _trailPushSpeed = 5f;
        [SerializeField, Range(1, 12)] private int _trailPoolSize = 3;
        [SerializeField] private Transform _trailPoolParent;

        private float _trailTimer;

        public bool UseTrailVines => _useTrailVines;
        public float TrailSpawnInterval => _trailSpawnInterval;
        public float TrailLifetime => _trailLifetime;
        public float TrailPushSpeed => _trailPushSpeed;

        [Header("Attack Vine")]
        [SerializeField] private PrideAttackVine _attackVinePrefab;
        [SerializeField] private Transform _projectileSpawnPoint;
        [SerializeField] private Transform _projectilePoolParent;
        [SerializeField, Range(1f, 20f)] private float _projectileSpeed = 8f;
        [SerializeField, Range(1, 5)] private int _projectileDamage = 1;
        [SerializeField, Range(1f, 10f)] private float _projectileLifetime = 4f;
        [SerializeField, Range(1, 12)] private int _projectilePoolSize = 9;

        [Header("Attack Target")]
        [SerializeField] private float _distanceWeight = 1f;
        [SerializeField] private float _lastTargetPenalty = 2f;
        [SerializeField] private int _maxSameTargetInRow = 2;

        [Header("Visual Debug")]
        [SerializeField] private bool _useVisualDebug = true;
        [SerializeField] private Renderer _renderer;
        [SerializeField] private Color _defaultColor = Color.white;
        [SerializeField] private Color _telegraphColor = Color.yellow;
        [SerializeField] private Color _executeColor = Color.red;
        [SerializeField] private Color _vulnerableColor = Color.magenta;

        [Header("Audio Settings")]
        [field: SerializeField] public AudioClip AttackClip {get; private set;}
        [field: SerializeField] public AudioClip HurtClip {get; private set;}

        private Transform _lastTarget;
        private int _sameTargetCount = 0;

        private BehaviourTree _tree;
        private PrideTrailVinePool _trailPool;
        private PrideAttackVinePool _projectilePool;

        public enum AttackType
        {
            Vine = 0
        }

        public AttackType CurrentAttack { get; private set; }

        protected override void Awake()
        {
            base.Awake();

            _cooldownTimer = _attackCooldown;
            _trailTimer = 0f;

            if (_renderer == null)
            {
                _renderer = GetComponent<Renderer>();
            }

            if (_renderer != null)
            {
                _defaultColor = _renderer.material.color;
            }

            if (_useTrailVines && _trailVinePrefab != null)
            {
                _trailPool = new PrideTrailVinePool(
                    _trailVinePrefab,
                    _trailPoolSize,
                    _trailPoolParent
                );
            }

            if (_attackVinePrefab != null)
            {
                _projectilePool = new PrideAttackVinePool(
                    _attackVinePrefab,
                    _projectilePoolSize,
                    _projectilePoolParent
                );
            }

            BuildTree();
        }

        private void OnDisable()
        {
            ResetVisual();
        }

        protected override void Update()
        {
            base.Update();

            if (_isDead || _isStunned)
                return;

            _tree?.Process();

            if (_cooldownTimer > 0f)
            {
                _cooldownTimer -= Time.deltaTime;
            }
        }

        private void BuildTree()
        {
            var pathFinder = new EnemyPathFinder(_graphComponent);

            _tree = new BehaviourTree("Pride");

            var root = new PrioritySelector("Root");

            root.AddChild(BuildAttackSelector());

            var chooseAttack = new Sequence("ChooseAttack");
            chooseAttack.AddChild(new Leaf("NotAttacking", new ConditionStrategy(() => !IsAttacking)));
            chooseAttack.AddChild(new Leaf("CooldownReady", new ConditionStrategy(() => CanAttack)));
            chooseAttack.AddChild(new Leaf("Choose", new PrideChooseAttackStrategy(this)));

            root.AddChild(chooseAttack);
            root.AddChild(new Leaf("MoveGraph", new PrideMoveGraphStrategy(this, pathFinder)));

            _tree.AddChild(root);
        }

        private Node BuildAttackSelector()
        {
            var selector = new PrioritySelector("AttackSelector");

            var attackSequence = new Sequence("DoAttack");
            attackSequence.AddChild(new Leaf("IsAttacking", new ConditionStrategy(() => IsAttacking)));

            var attackChoice = new PrioritySelector("AttackChoice");

            var vineSequence = new Sequence("VineAttack", 10);
            vineSequence.AddChild(new Leaf("CheckVine", new ConditionStrategy(() => CurrentAttack == AttackType.Vine)));
            vineSequence.AddChild(new Leaf("Vine", new PrideVineAttackStrategy(this)));

            attackChoice.AddChild(vineSequence);
            attackSequence.AddChild(attackChoice);
            selector.AddChild(attackSequence);

            return selector;
        }

        public override void ApplyDamage(int damage)
        {
            if(HurtClip != null)
            {
                AudioManager.Instance.PlaySFX(HurtClip);
            }
            
            base.ApplyDamage(damage);
        }

        public void SetAttack(AttackType type)
        {
            CurrentAttack = type;
        }

        public void StartAttack()
        {
            IsAttacking = true;
            if(AttackClip != null)
            {
                AudioManager.Instance.PlaySFX(AttackClip);
            }

            StopMovement();
        }

        public void EndAttack()
        {
            IsAttacking = false;
            _cooldownTimer = _attackCooldown;
            AllowMovement();
            ResetVisual();
        }

        public void AllowMovement()
        {
            CanMove = true;
        }

        public void StopMovement()
        {
            CanMove = false;
        }

        public bool ShouldSpawnTrailVine()
        {
            if (!_useTrailVines)
                return false;

            _trailTimer += Time.deltaTime;
            return _trailTimer >= _trailSpawnInterval;
        }

        public void ResetTrailVineTimer()
        {
            _trailTimer = 0f;
        }

        public Transform ChooseAttackTarget()
        {
            GameObject[] players = GameObject.FindGameObjectsWithTag("Player");

            if (players == null || players.Length == 0)
                return null;

            Transform chosenTarget = null;
            float bestScore = float.MinValue;

            foreach (GameObject player in players)
            {
                if (player == null)
                    continue;

                float distance = Vector3.Distance(transform.position, player.transform.position);
                float score = -distance * _distanceWeight;

                if (_lastTarget != null && player.transform == _lastTarget)
                {
                    score -= _lastTargetPenalty;

                    if (_sameTargetCount >= _maxSameTargetInRow)
                    {
                        score -= 999f;
                    }
                }

                if (score > bestScore)
                {
                    bestScore = score;
                    chosenTarget = player.transform;
                }
            }

            if (chosenTarget == null)
                return null;

            if (chosenTarget == _lastTarget)
            {
                _sameTargetCount++;
            }
            else
            {
                _lastTarget = chosenTarget;
                _sameTargetCount = 1;
            }

            return chosenTarget;
        }

        public override void ExecuteAttack()
        {
        }

        public void RotateTowardsTarget(Transform target)
        {
            if (target == null)
                return;

            RotateTowardsPosition(target.position);
        }

        public void RotateTowardsPosition(Vector3 targetPosition)
        {
            Vector3 direction = targetPosition - transform.position;
            direction.y = 0f;

            if (direction.sqrMagnitude <= 0.0001f)
                return;

            Quaternion targetRotation = Quaternion.LookRotation(direction.normalized, Vector3.up);

            transform.rotation = Quaternion.RotateTowards(
                transform.rotation,
                targetRotation,
                _rotationSpeed * Time.deltaTime
            );
        }

        public bool IsFacingTarget(Transform target, float maxAngleDifference = 8f)
        {
            if (target == null)
                return true;

            Vector3 direction = target.position - transform.position;
            direction.y = 0f;

            if (direction.sqrMagnitude <= 0.0001f)
                return true;

            float angle = Vector3.Angle(transform.forward, direction.normalized);
            return angle <= maxAngleDifference;
        }

        public Vector3 GetDirectionToTarget(Transform target, Vector3 fallbackDirection)
        {
            if (target == null)
                return fallbackDirection.normalized;

            Vector3 direction = target.position - transform.position;
            direction.y = 0f;

            if (direction.sqrMagnitude <= 0.0001f)
                return fallbackDirection.normalized;

            return direction.normalized;
        }

        public void SpawnTrailVine()
        {
            if (!_useTrailVines || _trailPool == null)
                return;

            Transform spawnPoint = _trailSpawnPoint != null ? _trailSpawnPoint : transform;

            PrideTrailVine vine = _trailPool.Get();
            if (vine == null)
                return;

            vine.transform.SetPositionAndRotation(
                spawnPoint.position,
                Quaternion.identity
            );

            vine.Initialize(
                _trailPool,
                _trailLifetime,
                _trailPushSpeed,
                -transform.forward
            );
        }

        public void ExecuteVineAttack(Transform target)
        {
            if (_projectilePool == null)
            {
                Debug.LogWarning("[Pride] Projectile pool n�o foi inicializado.");
                return;
            }

            Transform spawnPoint = _projectileSpawnPoint != null ? _projectileSpawnPoint : transform;
            Vector3 baseDirection = GetDirectionToTarget(target, transform.forward);
            baseDirection.y = 0f;

            if (baseDirection.sqrMagnitude <= 0.0001f)
            {
                baseDirection = transform.forward;
            }

            baseDirection.Normalize();

            Vector3 leftDirection = Quaternion.Euler(0f, -90f, 0f) * baseDirection;
            Vector3 rightDirection = Quaternion.Euler(0f, 90f, 0f) * baseDirection;

            SpawnProjectile(spawnPoint.position, baseDirection);
            SpawnProjectile(spawnPoint.position, leftDirection);
            SpawnProjectile(spawnPoint.position, rightDirection);
        }

        #region Projectile Spawn

        private void SpawnProjectile(Vector3 position, Vector3 direction)
        {
            PrideAttackVine projectile = _projectilePool.Get();
            if (projectile == null)
            {
                Debug.LogWarning("[Pride] Pool de proj�til vazio.");
                return;
            }

            projectile.transform.SetPositionAndRotation(
                position,
                Quaternion.LookRotation(direction.normalized, Vector3.up)
            );

            projectile.Initialize(
                direction.normalized,
                _projectileSpeed,
                _projectileDamage,
                _projectileLifetime,
                _projectilePool,
                transform
            );
        }

        #endregion

        #region Visual Debug

        public void ShowTelegraphVisual()
        {
            if (!_useVisualDebug || _renderer == null)
                return;

            _renderer.material.color = _telegraphColor;
        }

        public void ShowExecuteVisual()
        {
            if (!_useVisualDebug || _renderer == null)
                return;

            _renderer.material.color = _executeColor;
        }

        public void ShowVulnerableVisual()
        {
            if (!_useVisualDebug || _renderer == null)
                return;

            _renderer.material.color = _vulnerableColor;
        }

        public void ResetVisual()
        {
            if (!_useVisualDebug || _renderer == null)
                return;

            _renderer.material.color = _defaultColor;
        }

        #endregion
    }
}