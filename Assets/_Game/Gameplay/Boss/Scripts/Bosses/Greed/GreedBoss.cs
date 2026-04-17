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
        [SerializeField] private float _vulnerableTime;
        [SerializeField] private float _vulnerableTimer;

        [Header("Treasure Settings")]
        public bool IsTouched { get; set; }

        [Header("Audio Settings")]
        [field: SerializeField] public AudioClip HurtClip {get; private set;}
        [field: SerializeField] public AudioClip AttackClip {get; private set;}

        private IntervalStrategy _greedIntervalStrategy;
        private GreedAttackStrategy _greedAttackStrategy;
        private GreedChaseStrategy _greedChaseStrategy;
        private GreedPatrolStrategy _greedPatrolStrategy;
        private BehaviourTree _tree;

        protected override void Awake()
        {
            base.Awake();
            _isVulnerable = false;
        }

        protected override void OnAIInitialized()
        {
            if (!TryResolveGraphComponent(ref _graphComponent))
            {
                FailAIInitialization("[Greed] GraphComponent não foi configurado.");
                return;
            }

            _tree = new BehaviourTree("Greed tree");

            var pathFinder = new EnemyPathFinder(_graphComponent);
            _greedAttackStrategy = new GreedAttackStrategy(this);
            _greedChaseStrategy = new GreedChaseStrategy(this, pathFinder);
            _greedChaseStrategy.SetActivePlayers(_activePlayers);
            _greedPatrolStrategy = new GreedPatrolStrategy(this, pathFinder);
            _greedIntervalStrategy = new IntervalStrategy(_intervalTime);

            var angerSequence = new Sequence("AngerMode", priority: 100);
            angerSequence.AddChild(new Leaf("CheckIsTouched", new ConditionStrategy(() => IsTouched)));
            angerSequence.AddChild(new Leaf("Chase", _greedChaseStrategy));
            angerSequence.AddChild(new Leaf("Attack", _greedAttackStrategy));

            var normalSequence = new Sequence("NormalMode", priority: 10);
            normalSequence.AddChild(new Leaf("Wait", _greedIntervalStrategy));
            normalSequence.AddChild(new Leaf("Patrol", _greedPatrolStrategy));

            var rootSelector = new PrioritySelector("NormalOrAnger");
            rootSelector.AddChild(angerSequence);
            rootSelector.AddChild(normalSequence);

            _tree.AddChild(rootSelector);
            _isVulnerable = false;
        }

        protected override void Update()
        {
            if (!IsInitialized || _tree == null)
            {
                return;
            }

            _tree.Process();
            HandleVulnerability();
        }

        public override void ApplyDamage(int damage)
        {
            PlayHurtSfx();
            base.ApplyDamage(damage);
        }
        protected override void OnDeath()
        {
            if (_isDead)
            {
                Debug.Log($"[Boss] {gameObject.name} morreu.");
                gameObject.SetActive(false);
            }
        }
        public void SetVulnerability(bool cantakeDamege)
        {
            _isVulnerable = cantakeDamege;
        }
        private void HandleVulnerability()
        {
            if (IsTouched)
            {
                _vulnerableTimer += Time.deltaTime;
                _isVulnerable = true;

                if (_vulnerableTimer >= _vulnerableTime)
                {
                    Debug.Log("[Boss] Tempo de vulnerabilidade acabou. Resetando.");
                    ResetStatesBoss();
                }
            }
        }
        private void ResetStatesBoss()
        {
            IsTouched = false;
            _isVulnerable = false;
            _vulnerableTimer = 0f;
            _greedChaseStrategy.Reset();
            _greedAttackStrategy.Reset();
            _greedIntervalStrategy.Reset();
        }
        public override void ExecuteAttack()
        {
            Debug.Log($"<color=red>[Boss] {gameObject.name} executou o hit!</color>");
            PlayAttackSfx();
        }
        private void OnDrawGizmos()
        {
            _greedAttackStrategy?.DrawGizmos(this.transform.position);
        }
    }
}