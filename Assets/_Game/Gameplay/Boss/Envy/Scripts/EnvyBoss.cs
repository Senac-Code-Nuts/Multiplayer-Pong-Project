using Pong.Framework.BehaviourTree;
using Pong.Gameplay.Enemy;
using Pong.Systems.Graph;
using UnityEngine;

namespace Pong.Gameplay.Boss
{
    public class EnvyBoss : BossActor
    {
        private EnvySlotMachine _slotMachine;
        private BehaviourTree _tree;
        [Header("Attack Settings")]
        public bool IsPreparingAttack {get; private set;}
        private float _prepareTimer;

        [SerializeField] private GameObject _projectilePrefab;
        [SerializeField] private Transform _projectileSpawnPoint;
        [SerializeField] private int _projectileCount;
        [SerializeField] private float _projectileSpeed;
        [SerializeField] private float _prepareDuration = 1.5f;
        [SerializeField] private float _attackCooldown = 1.5f;
        private float _cooldownTimer;

        public bool CanAttack => _cooldownTimer <= 0f;
        public bool IsInSlotPhase { get; private set; }

        [Header("Vulnerability settings")]
        [SerializeField] private float _vulnerableDuration = 2f;
        

        [Header("Move Settings")]
        [SerializeField] private  GraphComponent _graphComponent;
        [SerializeField] private float _moveSpeed = 3f;
        public bool CanMove {get; private set;} = true;


        protected override void Awake()
        {
            base.Awake();
            _slotMachine = GetComponent<EnvySlotMachine>();
            BuildTree();
        }

        private void Start()
        {
            _slotMachine.OnResultReady += OnSlotResult;
        }

        protected override void Update()
        {
            base.Update();

            _tree?.Process();
            if(_cooldownTimer >0f)
            {
                _cooldownTimer -= Time.deltaTime;
            }

        }

        private void OnDestroy()
        {
            _slotMachine.OnResultReady -= OnSlotResult;
        }


        private void BuildTree()
        {
            var pathFinder = new EnemyPathFinder(_graphComponent);
            _tree = new BehaviourTree("Envy");

            var root = new PrioritySelector("Root");

            var attackSequence = new Sequence("Attack");

            attackSequence.AddChild(new Leaf("IsPreparingAttack", new ConditionStrategy(() => IsPreparingAttack)));
            attackSequence.AddChild(new Leaf("CanAttack", new ConditionStrategy(() => CanAttack)));
            attackSequence.AddChild(new Leaf("ExecuteAttack", new EnvyAttackStrategy(this)));
            root.AddChild(attackSequence);

            var vulnerableSequence = new Sequence("Vulnerable");
            vulnerableSequence.AddChild(new Leaf("IsVulnerable", new ConditionStrategy(() => IsVulnerable)));
            vulnerableSequence.AddChild(new Leaf("VulnerableTimer", new EnvyVulnerableTimerStrategy(this, _vulnerableDuration)));
            root.AddChild(vulnerableSequence);

            var slotSequence = new Sequence("SlotPhase");

            slotSequence.AddChild(new Leaf("IsSlotPhase", new ConditionStrategy(() => IsInSlotPhase)));

            slotSequence.AddChild(new Leaf("WaitSlot", new EnvyWaitStrategy()));

            root.AddChild(slotSequence);

            root.AddChild(new Leaf("MoveGraph", new EnvyMoveGraphStrategy(this,pathFinder,() => _moveSpeed)));

            _tree.AddChild(root);
        }

        protected override void OnDamageTaken()
        {
            if(!_slotMachine.IsRolling)
            {
                IsInSlotPhase = true;
                _slotMachine.StartRollAll();
                
            }
        }
        public override void ApplyDamage(int damage)
        {
            if (_isDead)
                return;

            OnDamageTaken();

            if (!_isVulnerable)
                return;

            _currentHealth -= damage;

            if (_currentHealth <= 0)
            {
                _currentHealth = 0;
                Die();
            }
        }

        public void OnSlotResult(int yesCount, int noCount)
        {

            IsInSlotPhase = false;
            if(yesCount >= 2)
            {

                EnterVulnerableState();
            } else
            {

                SetVulnerable(false);
                PrepareAttack();
            }
        }

        private void PrepareAttack()
        {
            if(IsPreparingAttack) return;


            IsPreparingAttack = true;
            _prepareTimer = 0f;

            StopMovement();   
        }
        public override void ExecuteAttack()
        {
            if(!IsPreparingAttack) return;

            _prepareTimer += Time.deltaTime;

            if(_prepareTimer < _prepareDuration)
            {
                return;
            }

            FireProjectiles360();

            IsPreparingAttack = false;
            AllowMovement();
            _cooldownTimer = _attackCooldown; 

        }

        private void FireProjectiles360()
        {
            float angleStep = 360f/_projectileCount;

            for(int i = 0; i < _projectileCount; i++)
            {
                float angle = angleStep * i;

                Vector3 direction = Quaternion.Euler(0,angle,0) * Vector3.forward;

                SpawnProjectile(_projectileSpawnPoint.position, direction);
            }
        }

        private void AllowMovement()
        {
            CanMove = true;
        }

        private void StopMovement()
        {
            CanMove = false;
            
        }
        public void EnterVulnerableState()
        {
            SetVulnerable(true);

        }

        public void ExitVulnerableState()
        {
            SetVulnerable(false);
            AllowMovement();
        }

        private void SpawnProjectile(Vector3 position, Vector3 direction)
        {
            GameObject projectile = Instantiate(_projectilePrefab, position, Quaternion.identity);

            Rigidbody rigidbody = projectile.GetComponent<Rigidbody>();

            if(rigidbody != null)
            {
                rigidbody.linearVelocity = direction.normalized * _projectileSpeed;
            }
        }

    }
}