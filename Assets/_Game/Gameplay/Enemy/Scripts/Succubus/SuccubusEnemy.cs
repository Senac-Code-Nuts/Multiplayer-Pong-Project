using UnityEngine;
using Pong.Framework.BehaviourTree;
using Pong.Systems.Graph;
using Pong.Gameplay.Player;
using Pong.Core.Gizmo;

namespace Pong.Gameplay.Enemy.Succubus
{
    public class SuccubusEnemy : EnemyActor
    {
        [Header("Movement")]
        [SerializeField] private float _patrolSpeed = 3f;
        public float PatrolSpeed => _patrolSpeed;

        [SerializeField] private float _chaseSpeed = 6f;
        public float ChaseSpeed => _chaseSpeed;

        [Header("Pathfinding")]
        [SerializeField] private GraphComponent _graphComponent;
        [SerializeField] private float _pathfindRecalculateInterval = 0.5f;

        private BehaviourTree _tree;
        private ChaseStrategy _chaseStrategy;
        private PatrolStrategy _patrolStrategy;
        private AttackStrategy _attackStrategy;
        private Material _meshMaterial;
        private float _colorRedIntensity = 0f;

        protected override void Awake()
        {
            base.Awake();

            Renderer renderer = GetComponent<Renderer>();
            _meshMaterial = renderer.material;

            _tree = new BehaviourTree("Succubus");
            var pathFinder = new EnemyPathFinder(_graphComponent);

            _chaseStrategy = new ChaseStrategy(this, pathFinder);
            _patrolStrategy = new PatrolStrategy(this, pathFinder);
            _attackStrategy = new AttackStrategy(this);
            
            var chaseNode = new Leaf("Chase", _chaseStrategy, priority: 10);
            var patrolNode = new Leaf("Patrol", _patrolStrategy, priority: 1);
            var attackNode = new Leaf("Attack", _attackStrategy, priority: 10);

            // Sequence: Passou 5s E tenta Chase
            var chaseSequence = new Sequence("ChaseSequence", priority: 10);
            chaseSequence.AddChild(new Leaf("ChaseInterval5s", new IntervalStrategy(5f)));
            chaseSequence.AddChild(chaseNode);
            chaseSequence.AddChild(attackNode); // Ataca depois de chegar perto


            // PrioritySelector: ordena por prioridade
            var prioritySelector = new PrioritySelector("ChaseOrPatrol");
            prioritySelector.AddChild(chaseSequence);
            prioritySelector.AddChild(patrolNode);

            _tree.AddChild(prioritySelector);
        }

        private void Update()
        {
            _colorRedIntensity = Mathf.Clamp01(_colorRedIntensity + Time.deltaTime / 5f);
            UpdateMeshColor();

            _tree.Process();
            
            if (_attackStrategy != null && _attackStrategy.IsInRecovery)
            {
                ResetMeshColor();
            }
            
            if (_attackStrategy != null && _attackStrategy.JustFinished && _patrolStrategy != null)
            {
                _patrolStrategy.Reset();
                Debug.Log("<color=cyan>[Succubus] Voltando a patrulhar após ataque!</color>");
            }
        }

        private void UpdateMeshColor()
        {
            if (_meshMaterial == null) return;

            Color color = Color.Lerp(Color.white, Color.red, _colorRedIntensity);
            _meshMaterial.color = color;
        }

        public void ResetMeshColor()
        {
            _colorRedIntensity = 0f;
            UpdateMeshColor();
        }

        public override void ExecuteAttack()
        {
            throw new System.NotImplementedException();
        }

        private void OnDrawGizmos()
        {
            if (_attackStrategy == null) return;

            var gizmoData = _attackStrategy.GetGizmoData();
            
            if (!gizmoData.IsInTelegraph) return;

            Color chargeColor = Color.Lerp(Color.yellow, Color.red, gizmoData.ChargeProgress);

            GizmoDrawer.DrawChargingCircle(
                transform.position, 
                gizmoData.CurrentRadius, 
                gizmoData.MaxRadius, 
                chargeColor, 
                Color.white, 
                segments: 32
            );

            // Ponto no centro
            GizmoDrawer.DrawPoint(transform.position, 0.2f, Color.red);
        }
    }
}