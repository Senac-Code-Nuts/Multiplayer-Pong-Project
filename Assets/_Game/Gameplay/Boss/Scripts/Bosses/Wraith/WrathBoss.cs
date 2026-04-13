using Pong.Framework.BehaviourTree;
using Pong.Gameplay.Enemy;
using UnityEngine;

namespace Pong.Gameplay.Boss
{
    public class WrathBoss : BossActor
    {
        [SerializeField] private GameObject _painPrefab;
        private BehaviourTree _tree;
        public AttackType CurrentAttack {get; private set;}
        [SerializeField] private EnemyPathFinder _pathFinder;
        [SerializeField] private float _moveSpeed = 3f;

        public enum AttackType
        {
            Spin,
            Throw
        }
        protected override void Awake()
        {
            base.Awake();
            BuildTree();
        }
        private void Update()
        {
            _tree?.Process();
        }
        public void SetAttack(AttackType type)
        {
            CurrentAttack = type;
        }
        private void BuildTree()
        {
            _tree = new BehaviourTree("Wrath");
            var root = new PrioritySelector("Root");
            var mainLoop = new Sequence("MainLoop");

            mainLoop.AddChild(new Leaf("MoveGraph", new WrathMoveGraphStrategy(this, _pathFinder, () => _moveSpeed)));

            mainLoop.AddChild(new Leaf("ChooseAttack", new WraithChooseAtackStrategy(this)));

            mainLoop.AddChild(BuildAttackSelector());

            root.AddChild(mainLoop);

            _tree.AddChild(root);

        }

        private Node BuildAttackSelector()
        {
            var selector = new PrioritySelector("AttackSelector");

            var spinSequence = new Sequence("SpinAttack", 10);
            spinSequence.AddChild(new Leaf("CheckSpin", new ConditionStrategy(() => CurrentAttack == AttackType.Spin)));

            spinSequence.AddChild(new Leaf("Spin", new WrathSpinAttackStrategy(this)));

            var throwSequence = new Sequence("ThrowAttack", 10);
            throwSequence.AddChild(new Leaf("CheckThrow", new ConditionStrategy(() => CurrentAttack == AttackType.Throw)));
            throwSequence.AddChild(new Leaf("Throw", new WrathThrowPainStrategy(this)));

            selector.AddChild(spinSequence);
            selector.AddChild(throwSequence);

            return selector;
        }
        public void StopMovement()
        {
            
        }
        public void MoveTo(Vector3 pos)
        {
            transform.position = Vector3.MoveTowards(transform.position, pos, _moveSpeed * Time.deltaTime);
        }
        public void ShowAttackArea()
        {
            
        }
        public void ExecuteSpinAttack()
        {
            
        }
        public GameObject SpawnPain()
        {
            return _painPrefab;
        }
        public void PlayPickupAnimation()
        {
            
        }

        public override void ExecuteAttack()
        {
            
        }
    }
}