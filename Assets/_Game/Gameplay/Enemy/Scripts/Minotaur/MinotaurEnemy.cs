using System.Collections.Generic;
using UnityEngine;
using Pong.Framework.BehaviourTree;
using Pong.Gameplay.Player;
using Pong.Systems.Graph;
using Pong.Gameplay.Relics;

namespace Pong.Gameplay.Enemy
{
    public class MinotaurEnemy : EnemyActor
    {
        [Header("Movement")]
        [SerializeField] private float _movementSpeed = 2.75f;
        [SerializeField] private float _pathDecisionDelay = 1.25f;
        [SerializeField] private float _parryModeSpeedMultiplier = 1.5f;

        [Header("Specific Attributes")]
        [SerializeField] private float _preAttackTime = 0.75f;
        [SerializeField] private float _counterWindowTimeout = 2f;
        [SerializeField] private float _attackBoostMultiplier = 1.6f;

        [Header("Components")]
        [SerializeField] private GraphComponent _graphComponent;
        [SerializeField] private Relic _relic;
        [SerializeField] private Renderer _renderer;

        private BehaviourTree _tree;
        private MinotaurMoveStrategy _moveStrategy;
        private MinotaurCounterWindowStrategy _counterWindowStrategy;
        private List<PlayerController> _activePlayers;
        private InfluenceSystem _influenceSystem;
        private bool _isAIActive;
        private Material _material;

        private bool _isCounterAttackReady;
        private bool _hasCounterAttackTriggered;
        private bool _hasCounterAttackExecuted;

        public float MovementSpeed => _movementSpeed;
        public float PathDecisionDelay => _pathDecisionDelay;

        public float CurrentMovementSpeed => IsInParryMode ? _movementSpeed * _parryModeSpeedMultiplier : _movementSpeed;
        public float PreAttackTime => _preAttackTime;
        public float CounterWindowTimeout => _counterWindowTimeout;
        public float AttackBoostMultiplier => _attackBoostMultiplier;
        public GraphComponent GraphComponent => _graphComponent;
        public Relic TargetRelic => _relic;
        public bool IsCounterAttackReady => _isCounterAttackReady;
        

        public bool IsInParryMode => _counterWindowStrategy != null && _counterWindowStrategy.IsArmed;

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
        }

        public override void InitializeAI(List<PlayerController> activePlayers, InfluenceSystem influenceSystem)
        {
            _activePlayers = activePlayers ?? new List<PlayerController>();
            _influenceSystem = influenceSystem;

            _graphComponent = _influenceSystem.GraphComponent;

            _tree = new BehaviourTree("Minotaur");

            var pathFinder = new EnemyPathFinder(_graphComponent);
            _moveStrategy = new MinotaurMoveStrategy(this, pathFinder);
            _counterWindowStrategy = new MinotaurCounterWindowStrategy(this);

            var cycleSelector = new PrioritySelector("MinotaurCycle");
            cycleSelector.AddChild(new Leaf("CounterWindow", _counterWindowStrategy, priority: 10));
            cycleSelector.AddChild(new Leaf("Move", _moveStrategy, priority: 1));

            _tree.AddChild(cycleSelector);
            ResetCycle();
            _isAIActive = true;
        }

        private void OnEnable()
        {
            ResetCycle();
        }

        private void Update()
        {
            if (!_isAIActive)
            {
                return;
            }

            _tree?.Process();
        }

        private void OnDisable()
        {
            ResetCycle();
        }

        public bool ConsumeCounterAttackTriggered()
        {
            bool triggered = _hasCounterAttackTriggered;
            _hasCounterAttackTriggered = false;
            return triggered;
        }

        public bool ConsumeCounterAttackExecuted()
        {
            bool executed = _hasCounterAttackExecuted;
            _hasCounterAttackExecuted = false;
            return executed;
        }

        public void FaceRelic()
        {
            if (_relic == null)
            {
                return;
            }

            Vector3 lookDirection = _relic.transform.position - transform.position;
            lookDirection.y = 0f;

            if (lookDirection.sqrMagnitude < 0.0001f)
            {
                return;
            }

            transform.rotation = Quaternion.LookRotation(lookDirection.normalized, Vector3.up);
        }

        public void BeginPreparation()
        {
            _isCounterAttackReady = false;
            SetVulnerable(false);
            SetCycleColor(Color.gray);
        }

        public void BeginCounterWindow()
        {
            _isCounterAttackReady = true;
            SetVulnerable(true);
            SetCycleColor(Color.yellow);
        }

        public void ContinueRelicMovement()
        {
            _moveStrategy?.TickMovement();
        }

        public void SetCounterAttackReady(bool value)
        {
            _isCounterAttackReady = value;
        }

        public void SetCycleColor(Color color)
        {
            if (_material == null)
            {
                return;
            }

            _material.color = color;
        }

        public void ResetCycle()
        {
            _isCounterAttackReady = false;
            _hasCounterAttackTriggered = false;
            _hasCounterAttackExecuted = false;
            SetVulnerable(true);
            SetCycleColor(Color.gray);

            _moveStrategy?.Reset();
            _counterWindowStrategy?.Reset();
            _tree?.Reset();
        }

        public override void ApplyDamage(int damage)
        {
            if (_isDead)
            {
                return;
            }

            if (_isCounterAttackReady && _isVulnerable)
            {
                ExecuteAttack();
            }

            base.ApplyDamage(damage);
        }

        public override void ExecuteAttack()
        {
            if (_relic != null)
            {
                Vector3 attackAxis = _relic.transform.position - transform.position;
                attackAxis.y = 0f;

                if (attackAxis.sqrMagnitude < 0.0001f)
                {
                    attackAxis = transform.forward;
                    attackAxis.y = 0f;
                }

                Debug.Log("<color=cyan>[Minotaur] Parry hit. Reflecting relic.</color>");
                _relic.InvertDirection(attackAxis);
            }

            _hasCounterAttackTriggered = true;
            _hasCounterAttackExecuted = true;
            _isCounterAttackReady = false;
            SetVulnerable(true);
            SetCycleColor(Color.gray);
        }
    }
}