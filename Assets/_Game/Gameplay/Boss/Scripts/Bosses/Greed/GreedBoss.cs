using Pong.Framework.BehaviourTree;
using Pong.Gameplay.Enemy;
using Pong.Gameplay.Enemy.Succubus;
using Pong.Systems.Graph;
using UnityEngine;

namespace Pong.Gameplay.Boss.Greed
{
    public class GreedBoss : BossActor
    {
        [Header("Moviment Settings")]
        [SerializeField] private GraphComponent _graphComponent;
        [SerializeField] private float _patrolSpeed = 3f;
        public float PatrolSpeed => _patrolSpeed;

        [SerializeField] private float _chaseSpeed = 6f;
        public float ChaseSpeed => _chaseSpeed;

        [Header("Time Settings")]
        [SerializeField] private float _intervalTime;
        [SerializeField] private float _vulnerableTime = 3f;
        private float _vulnerableTimer;

        [Header("References")]
        [SerializeField] private Treasure _treasure;
        [SerializeField] private GreedCircle _circle;

        [Header("Treasure Settings")]
        public bool IsTouched { get; set; }

        private IntervalStrategy _intervalStrategy;
        private GreedAttackStrategy _attackStrategy;
        private GreedChaseStrategy _chaseStrategy;
        private GreedPatrolStrategy _patrolStrategy;
        private BehaviourTree _tree;

        protected override void Awake()
        {
            base.Awake();

            if (_treasure == null)
                _treasure = GetComponentInChildren<Treasure>();

            if (_circle == null)
                _circle = GetComponentInChildren<GreedCircle>();

            if (_treasure != null)
                _treasure.SetBoss(this);

            if (_circle != null)
                _circle.SetBoss(this);

            _isVulnerable = false;
        }

        protected override void OnAIInitialized()
        {
            if (!TryResolveGraphComponent(ref _graphComponent))
            {
                FailAIInitialization("[Greed] GraphComponent não configurado.");
                return;
            }

            _tree = new BehaviourTree("Greed tree");

            var pathFinder = new EnemyPathFinder(_graphComponent);

            _attackStrategy = new GreedAttackStrategy(this);
            _chaseStrategy = new GreedChaseStrategy(this, pathFinder);
            _chaseStrategy.SetActivePlayers(_activePlayers);

            _patrolStrategy = new GreedPatrolStrategy(this, pathFinder);
            _intervalStrategy = new IntervalStrategy(_intervalTime);

            var anger = new Sequence("AngerMode", 100);
            anger.AddChild(new Leaf("CheckTouched", new ConditionStrategy(() => IsTouched)));
            anger.AddChild(new Leaf("Chase", _chaseStrategy));
            anger.AddChild(new Leaf("Attack", _attackStrategy));

            var normal = new Sequence("NormalMode", 10);
            normal.AddChild(new Leaf("Wait", _intervalStrategy));
            normal.AddChild(new Leaf("Patrol", _patrolStrategy));

            var root = new PrioritySelector("Root");
            root.AddChild(anger);
            root.AddChild(normal);

            _tree.AddChild(root);
        }

        protected override void Update()
        {
            if (!IsInitialized || _tree == null)
                return;

            _tree.Process();
            HandleVulnerability();
        }

        public override void ApplyDamage(int damage)
        {
            if (!_isVulnerable)
            {
                Debug.Log("[Greed] Invulnerável");
                return;
            }

            base.ApplyDamage(damage);
        }

        public override void ExecuteAttack()
        {
            Debug.Log("[Greed] Executou ataque");
            PlayAttackSfx();
        }

        public void SetVulnerability(bool value)
        {
            _isVulnerable = value;
        }

        private void HandleVulnerability()
        {
            if (!IsTouched) return;

            _vulnerableTimer += Time.deltaTime;

            if (_vulnerableTimer >= _vulnerableTime)
            {
                ResetState();
            }
        }

        private void ResetState()
        {
            Debug.Log("[Greed] Reset");

            IsTouched = false;
            _isVulnerable = false;
            _vulnerableTimer = 0f;

            _chaseStrategy.Reset();
            _attackStrategy.Reset();
            _intervalStrategy.Reset();
        }
    }
}