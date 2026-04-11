using UnityEngine;
using Pong.Framework.BehaviourTree;
using Pong.Systems.Graph;
using Pong.Gameplay.Player;

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
        private Material _meshMaterial;
        private float _colorRedIntensity = 0f;
        
        protected override void Awake()
        {
            base.Awake();

            Renderer renderer = GetComponent<Renderer>();
            if (renderer != null)
                _meshMaterial = renderer.material;

            _tree = new BehaviourTree("Succubus");
            var pathFinder = new EnemyPathFinder(_graphComponent);

            _chaseStrategy = new ChaseStrategy(this, pathFinder);
            var chaseNode = new Leaf("Chase", _chaseStrategy, priority: 10);
            var patrolNode = new Leaf("Patrol", new PatrolStrategy(this, pathFinder), priority: 1);

            // Sequence: Passou 5s E tenta Chase
            var chaseSequence = new Sequence("ChaseSequence", priority: 10);
            chaseSequence.AddChild(new Leaf("ChaseInterval5s", new IntervalStrategy(5f)));
            chaseSequence.AddChild(chaseNode);

            // PrioritySelector: ordena por prioridade
            var prioritySelector = new PrioritySelector("ChaseOrPatrol");
            prioritySelector.AddChild(chaseSequence);
            prioritySelector.AddChild(patrolNode);

            _tree.AddChild(prioritySelector);
        }

        private void Update()
        {
            // Aumenta intensidade vermelha gradualmente (0 a 1 em 5 segundos)
            _colorRedIntensity = Mathf.Clamp01(_colorRedIntensity + Time.deltaTime / 5f);
            UpdateMeshColor();

            _tree.Process();
        }

        private void UpdateMeshColor()
        {
            if (_meshMaterial == null) return;

            // Interpola de branco (1,1,1) para vermelho (1,0,0)
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
    }
}