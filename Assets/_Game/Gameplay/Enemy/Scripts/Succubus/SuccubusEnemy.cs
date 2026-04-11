using UnityEngine;
using Pong.Framework.BehaviourTree;
using Pong.Systems.Graph;

namespace Pong.Gameplay.Enemy.Succubus
{
    public class SuccubusEnemy : EnemyActor
    {
        [Header("Movement")]
        [SerializeField] private float _patrolSpeed = 3f;
        public float PatrolSpeed => _patrolSpeed;
        [Header("Pathfinding")]
        [SerializeField] private GraphComponent _graphComponent;
        [SerializeField] private float _pathfindRecalculateInterval = 0.5f;

        private BehaviourTree _tree;

        protected override void Awake()
        {
            base.Awake();
            _tree = new BehaviourTree("Succubus");
            var pathFinder = new EnemyPathFinder(_graphComponent);
            _tree.AddChild(new Leaf("Patrol", new PatrolStrategy(this, pathFinder)));
        }

        private void Update()
        {
            _tree.Process();
        }

        public override void ExecuteAttack()
        {
            throw new System.NotImplementedException();
        }
    }
}