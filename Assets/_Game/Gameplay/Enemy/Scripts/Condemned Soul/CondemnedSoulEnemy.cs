using UnityEngine;
using Pong.Framework.BehaviourTree;
using Pong.Systems.Graph;

namespace Pong.Gameplay.Enemy
{
    public class CondemnedSoulEnemy : EnemyActor
    {
        [Header("Specific Attributes")]
        [SerializeField] private float _movementSpeed = 2.25f;
        [SerializeField] private float _pathDecisionDelay = 0.75f;
        [SerializeField] private float _pauseTime = 0.2f;
        [SerializeField] private float _invulnerableTime = 3f;

        public float MovementSpeed => _movementSpeed;
        public float PathDecisionDelay => _pathDecisionDelay;
        public float PauseTime => _pauseTime;
        public float InvulnerableTime => _invulnerableTime;

        [Header("Components")]
        [SerializeField] private GraphComponent _graphComponent;
        [SerializeField] private Renderer _renderer;

        private BehaviourTree _tree;
        private AlmaMoveStrategy _moveStrategy;
        private AlmaPauseStrategy _pauseStrategy;
        private AlmaInvulnerabilityStrategy _invulnerabilityStrategy;
        private Material _material;

        protected override void Awake()
        {
            base.Awake();

            if (_renderer == null)
            {
                _renderer = GetComponent<Renderer>();
            }

            if (_renderer != null)
            {
                _material = _renderer.material;
                _material.color = Color.gray;
            }

            _tree = new BehaviourTree("Alma");

            var pathFinder = new EnemyPathFinder(_graphComponent);
            _moveStrategy = new AlmaMoveStrategy(this, pathFinder);
            _pauseStrategy = new AlmaPauseStrategy(_pauseTime);
            _invulnerabilityStrategy = new AlmaInvulnerabilityStrategy(this, _invulnerableTime);

            var cycleSequence = new Sequence("AlmaCycle");
            cycleSequence.AddChild(new Leaf("Move", _moveStrategy));
            cycleSequence.AddChild(new Leaf("Pause", _pauseStrategy));
            cycleSequence.AddChild(new Leaf("Invulnerable", _invulnerabilityStrategy));

            _tree.AddChild(cycleSequence);
        }

        private void OnEnable()
        {
            ExecuteAttack();
        }

        private void Update()
        {
            _tree?.Process();
        }

        public override void ExecuteAttack()
        {
            ResetCycle();
        }

        private void OnDisable()
        {
            ResetCycle();
        }

        public void SetCycleColor(Color color)
        {
            if (_material == null) return;

            _material.color = color;
        }

        public void ResetCycle()
        {
            _isVulnerable = true;
            SetCycleColor(Color.gray);

            _moveStrategy?.Reset();
            _pauseStrategy?.Reset();
            _invulnerabilityStrategy?.Reset();
            _tree?.Reset();
        }
    }
}