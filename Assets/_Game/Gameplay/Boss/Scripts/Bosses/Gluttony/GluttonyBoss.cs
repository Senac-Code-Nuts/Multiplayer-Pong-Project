using Pong.Framework.BehaviourTree;
using Pong.Framework.Strategy;
using Pong.Gameplay.Enemy;
using Pong.Systems.Audio;
using Pong.Systems.Graph;
using UnityEngine;
using UnityEngine.VFX;

namespace Pong.Gameplay.Boss
{
    public class GluttonyBoss : BossActor
    {
        private const string TAG = "<color=yellow>[Gluttony]</color>";

        [Header("Movement")]
        [SerializeField] private float _patrolSpeed = 3f;
        [SerializeField] private GraphComponent _graphComponent;
        [SerializeField] private float _rotationSpeed = 180f;
        public float PatrolSpeed => _patrolSpeed;
        public bool CanMove { get; private set; } = true;

        [Header("Attack Settings")]
        [SerializeField] private float _attackCooldown = 2f;
        [SerializeField] private float _distanceWeight = 1f;
        [SerializeField] private float _lastTargetPenalty = 2f;
        [SerializeField] private int _maxSameTargetInRow = 2;

        private Transform _lastTarget;
        private int _sameTargetCount = 0;

        private float _cooldownTimer;
        public bool CanAttack => _cooldownTimer <= 0f;
        public bool IsAttacking { get; private set; }

        [Header("Attack Timings")]
        [field: SerializeField, Range(0.1f, 10f)] public float TimeBetweenAttacks { get; private set; }
        [field: SerializeField, Range(0.1f, 5f)] public float PreAttackTime { get; private set; }
        [field: SerializeField, Range(0.1f, 10f)] public float AttackRecoveryTime { get; private set; } = 1f;

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
        [SerializeField, Range(1, 10)] private int _drinkDamage;
        [SerializeField] private GameObject _drinkConeTelegraph;
        [SerializeField, Range(0.5f, 10f)] private float _coneRadius;
        [SerializeField, Range(1f, 180f)] private float _coneAngle;
        [SerializeField] private LayerMask _playerLayerMask;

        [Header("VFX")]
        [SerializeField] private VisualEffect _drinkVfx;
        [SerializeField] private Transform _drinkVfxSpawnPoint;
        [SerializeField, Range(0.1f, 2f)] private float _drinkVfxStopDelay;

        [Header("Attack Helpers")]
        [SerializeField] private GluttonyConeAttack _coneAttack;

        [Header("Visual Debug")]
        [SerializeField] private bool _useVisualDebug = true;
        [SerializeField] private Renderer _renderer;
        [SerializeField] private Color _defaultColor = Color.white;
        [SerializeField] private Color _telegraphColor = Color.yellow;
        [SerializeField] private Color _executeColor = Color.red;

        [Header("Audio Settings")]
        [field: SerializeField] public AudioClip AttackClip {get; private set;}
        [field: SerializeField] public AudioClip HurtClip {get; private set;}

        private BehaviourTree _tree;
        private GluttonyPatrolStrategy _patrolStrategy;
        private GluttonyProjectilePool _boneProjectilePool;
        private Vector3 _lockedSpitDirection;

        private int _lastAttackIndex = -1;
        //private int _repeatCount = 0;

        public enum AttackType
        {
            Eat = 0,
            SpitBones = 1,
            ThrowDrink = 2
        }

        public AttackType CurrentAttack { get; private set; }

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

            BuildTree();
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

            _tree = new BehaviourTree("Gluttony");

            var root = new PrioritySelector("Root");

            root.AddChild(BuildAttackSelector());

            var chooseAttack = new Sequence("ChooseAttack");
            chooseAttack.AddChild(new Leaf("NotAttacking", new ConditionStrategy(() => !IsAttacking)));
            chooseAttack.AddChild(new Leaf("CooldownReady", new ConditionStrategy(() => CanAttack)));
            chooseAttack.AddChild(new Leaf("Choose", new GluttonyChooseAttackStrategy(this)));

            root.AddChild(chooseAttack);

            _patrolStrategy = new GluttonyPatrolStrategy(this, pathFinder);
            root.AddChild(new Leaf("Patrol", _patrolStrategy));

            _tree.AddChild(root);
        }

        private Node BuildAttackSelector()
        {
            var selector = new PrioritySelector("AttackSelector");

            var attackSequence = new Sequence("DoAttack");
            attackSequence.AddChild(new Leaf("IsAttacking", new ConditionStrategy(() => IsAttacking)));

            var attackChoice = new PrioritySelector("AttackChoice");

            var eatSequence = new Sequence("EatAttack", 10);
            eatSequence.AddChild(new Leaf("CheckEat", new ConditionStrategy(() => CurrentAttack == AttackType.Eat)));
            eatSequence.AddChild(new Leaf("Eat", new GluttonyEatStrategy(this)));

            var spitSequence = new Sequence("SpitBonesAttack", 10);
            spitSequence.AddChild(new Leaf("CheckSpit", new ConditionStrategy(() => CurrentAttack == AttackType.SpitBones)));
            spitSequence.AddChild(new Leaf("SpitBones", new GluttonySpitBonesStrategy(this)));

            var drinkSequence = new Sequence("DrinkAttack", 10);
            drinkSequence.AddChild(new Leaf("CheckDrink", new ConditionStrategy(() => CurrentAttack == AttackType.ThrowDrink)));
            drinkSequence.AddChild(new Leaf("Drink", new GluttonyDrinkAttackStrategy(this)));

            attackChoice.AddChild(eatSequence);
            attackChoice.AddChild(spitSequence);
            attackChoice.AddChild(drinkSequence);

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

            if (_coneAttack != null)
            {
                _coneAttack.EndTelegraph(_drinkConeTelegraph);
            }
        }

        public void AllowMovement()
        {
            CanMove = true;
        }

        public void StopMovement()
        {
            CanMove = false;
        }

        public int ChooseNextAttackIndex()
        {
            int next = Random.Range(0, 3);

            if (next == _lastAttackIndex)
            {
                int reroll = Random.Range(0, 3);

                if (reroll == _lastAttackIndex)
                {
                    do
                    {
                        reroll = Random.Range(0, 3);
                    }
                    while (reroll == _lastAttackIndex);
                }

                next = reroll;
            }

            _lastAttackIndex = next;
            return next;
        }

        public void ShowEatTelegraph()
        {
            SetTelegraphVisual();
            Debug.Log($"{TAG} Telegraph: Empanturrar");
        }

        public void ShowSpitBonesTelegraph()
        {
            SetTelegraphVisual();
            Debug.Log($"{TAG} Telegraph: Cuspir Ossos");
        }

        public void ShowDrinkTelegraph()
        {
            SetTelegraphVisual();
            Debug.Log($"{TAG} Telegraph: Arremessar Bebida");
        }
        public void BeginDrinkTelegraph(Vector3 direction)
        {
            if (_coneAttack == null)
                return;

            _coneAttack.BeginTelegraph(
                direction,
                _drinkConeTelegraph
            );
        }

        public void UpdateDrinkTelegraph(float progress)
        {
            if (_coneAttack == null)
                return;

            _coneAttack.UpdateTelegraph(
                progress,
                _coneRadius,
                _coneAngle,
                _drinkConeTelegraph
            );
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

        public void RotateTowardsTarget(Transform target)
        {
            if (target == null)
                return;

            Vector3 direction = target.position - transform.position;
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

        public void LockSpitBonesDirection(Transform target)
        {
            _lockedSpitDirection = GetDirectionToTarget(target, transform.forward);
        }
        public void ExecuteEat()
        {
            SetExecuteVisual();
            Debug.Log($"{TAG} Empanturrou.");
        }

        public void ExecuteSpitBones()
        {
            SetExecuteVisual();
            Debug.Log($"{TAG} Cuspiu ossos.");

            if (_boneProjectilePool == null)
            {
                Debug.LogWarning("[Gluttony] Pool de proj�til n�o foi inicializado.");
                return;
            }

            Transform spawnPoint = _projectileSpawnPoint != null ? _projectileSpawnPoint : transform;
            Vector3 baseDirection = _lockedSpitDirection.sqrMagnitude > 0.0001f
                ? _lockedSpitDirection
                : transform.forward;

            if (_projectileCount <= 1)
            {
                SpawnBoneProjectile(spawnPoint.position, baseDirection);
                return;
            }

            float angleStep = _projectileSpreadAngle / (_projectileCount - 1);
            float startAngle = -_projectileSpreadAngle * 0.5f;

            for (int i = 0; i < _projectileCount; i++)
            {
                float currentAngle = startAngle + angleStep * i;
                Vector3 direction = Quaternion.Euler(0f, currentAngle, 0f) * baseDirection;
                SpawnBoneProjectile(spawnPoint.position, direction.normalized);
            }
        }

        public void ExecuteDrinkAttack()
        {
            SetExecuteVisual();

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

        public void UpdateDrinkTelegraphDirection(Transform target)
        {
            if (_coneAttack == null)
                return;

            Vector3 direction = GetDirectionToTarget(target, transform.forward);
            _coneAttack.SetDirection(direction);
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
            if (!_useVisualDebug) return;
            if (_renderer == null) return;

            _renderer.material.color = _telegraphColor;
        }

        private void SetExecuteVisual()
        {
            if (!_useVisualDebug) return;
            if (_renderer == null) return;

            _renderer.material.color = _executeColor;
        }

        private void ResetVisual()
        {
            if (!_useVisualDebug) return;
            if (_renderer == null) return;

            _renderer.material.color = _defaultColor;
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

        public override void ExecuteAttack()
        {
            
        }
    }
}