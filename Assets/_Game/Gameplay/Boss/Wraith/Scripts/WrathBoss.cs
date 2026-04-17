using Pong.Framework.BehaviourTree;
using Pong.Gameplay.Enemy;
using Pong.Gameplay.Player;
using Pong.Systems.Graph;
using UnityEngine;

namespace Pong.Gameplay.Boss
{
    public class WrathBoss : BossActor
    {
        [Header("Attack Settings")]
        [SerializeField] private float _attackCooldown = 3f;
        private float _cooldownTimer = 0f;
        public bool CanAttack => _cooldownTimer <= 0f;

        [Header("Pain Settings")]
        [SerializeField] private GameObject _painPrefab;
        [SerializeField] private Transform _painSpawnPoint;
        [field : SerializeField] public float PainTelegraphTime {get;private set;}
        [field : SerializeField] public float PostThrowDelay {get; private set;} = 0.5f;
        [SerializeField] private float _throwForce;
        [SerializeField] private float _painRotateSpeed;

        [Header("Meelee Settings")]
        [SerializeField] private int _melleeDamage;
        [field : SerializeField] public float TelegraphTime {get; private set;} = 1.0f;
        [field : SerializeField] public float ChaseDistance {get; private set;} = 2.5f;
        [field : SerializeField]  public float RecoveryTime {get; private set;} = 1.5f;
        [field : SerializeField]  public float AttackRadius {get; private set;} = 3.5f;
        private BehaviourTree _tree;
        public AttackType CurrentAttack {get; private set;}
        public bool IsAttacking { get; private set; }
        [Header("Move Settings")]
        [SerializeField] private  GraphComponent _graphComponent;
        [SerializeField] private float _moveSpeed = 3f;
        public bool _canMove {get; private set;} = true;

        public enum AttackType
        {
            Spin,
            Throw
        }
        protected override void Awake()
        {
            base.Awake();
        }

        protected override bool ValidateAISetup()
        {
            if (_painPrefab == null)
            {
                Debug.LogWarning("[Wrath] Pain prefab não foi configurado. O ataque Throw ficará desativado.");
            }

            if (_painSpawnPoint == null)
            {
                Debug.LogWarning("[Wrath] Pain spawn point não foi configurado. O boss vai usar a própria posição.");
            }

            return true;
        }

        protected override void OnAIInitialized()
        {
            if (!TryResolveGraphComponent(ref _graphComponent))
            {
                FailAIInitialization("[Wrath] GraphComponent não foi configurado.");
                return;
            }

            BuildTree();
        }
        protected override void Update()
        {
            if (!IsInitialized)
            {
                return;
            }

            base.Update();

            _tree?.Process();
            if (_cooldownTimer > 0f)
            {
                _cooldownTimer -= Time.deltaTime;
            }  
        }


        public void SetAttack(AttackType type)
        {
            CurrentAttack = type;
        }
        public void StartAttack()
        {
            IsAttacking = true;
        }

        public void EndAttack()
        {
            IsAttacking = false;
            _cooldownTimer = _attackCooldown;
            AllowMovement();
        }
        private void BuildTree()
        {
            var pathFinder = new EnemyPathFinder(_graphComponent);

            _tree = new BehaviourTree("Wrath");

            var root = new PrioritySelector("Root");

            root.AddChild(BuildAttackSelector());

            var chooseAttack = new Sequence("ChooseAttack");
            chooseAttack.AddChild(new Leaf("NotAttacking", new ConditionStrategy(() => !IsAttacking)));

            chooseAttack.AddChild(new Leaf("CooldownReady", new ConditionStrategy(() => CanAttack)));

            chooseAttack.AddChild(new Leaf("Choose", new WraithChooseAtackStrategy(this)));

            root.AddChild(chooseAttack);

            root.AddChild(new Leaf("MoveGraph", new WrathMoveGraphStrategy(this, pathFinder, () => _moveSpeed)));

            _tree.AddChild(root);

        }

        private Node BuildAttackSelector()
        {
            var selector = new PrioritySelector("AttackSelector");

            var attackSequence = new Sequence("DoAttack");
            attackSequence.AddChild(new Leaf("IsAttacking", new ConditionStrategy(() => IsAttacking)));

            var attackChoice = new PrioritySelector("AttackChoice");

            var spinSequence = new Sequence("SpinAttack", 10);
            spinSequence.AddChild(new Leaf("CheckSpin", new ConditionStrategy(() => CurrentAttack == AttackType.Spin)));

            spinSequence.AddChild(new Leaf("Spin", new WrathSpinAttackStrategy(this)));

            var throwSequence = new Sequence("ThrowAttack", 10);
            throwSequence.AddChild(new Leaf("CheckThrow", new ConditionStrategy(() => CurrentAttack == AttackType.Throw)));
            throwSequence.AddChild(new Leaf("Throw", new WrathThrowPainStrategy(this)));

            attackChoice.AddChild(spinSequence);
            attackChoice.AddChild(throwSequence);

            attackSequence.AddChild(attackChoice);

            selector.AddChild(attackSequence);

            return selector;
        }

        public void MoveTo(Vector3 pos)
        {
            transform.position = Vector3.MoveTowards(transform.position, pos, _moveSpeed * Time.deltaTime);
        }
        public void RotateTowards(Vector3 targetPosition)
        {
            Vector3 direction = (targetPosition - transform.position);
            direction.y = 0f;

            if(direction == Vector3.zero)
            {
                return;
            } 

            Quaternion targetRotation = Quaternion.LookRotation(direction);

            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, _painRotateSpeed * Time.deltaTime);
        }

        public void AllowMovement()
        {
            _canMove = true;
        }

        public void StopMovement()
        {
            _canMove = false;
        }
        public void ShowAttackArea()
        {
            //Método para VFX da área de ataque
        }
        public override void ExecuteAttack()
        {
            Debug.Log("Spin attack");
            Collider[] hits = Physics.OverlapSphere(transform.position, AttackRadius);

            foreach(var hit in hits)
            {
                if (hit.GetComponentInParent<PlayerActor>() is PlayerActor playerActor)
                {
                    Debug.Log($"<color=orange>Hit player: {playerActor.name}</color>");
                    playerActor.ApplyDamage(_melleeDamage);
                }
            }
        }
        public GameObject SpawnPain()
        {
            if (_painPrefab == null)
            {
                Debug.LogWarning("[Wrath] Pain prefab não foi configurado.");
                return null;
            }

            GameObject target = GetFarthestPlayer();
            if (target == null)
            {
                Debug.LogWarning("[Wrath] Nenhum player encontrado para o ataque Throw.");
                return null;
            }

            Transform spawnPoint = _painSpawnPoint != null ? _painSpawnPoint : transform;
            GameObject pain = Instantiate(_painPrefab, spawnPoint.position, Quaternion.identity);
            var projectile = pain.GetComponent<PainThrow>();

            if (projectile == null)
            {
                Debug.LogWarning("[Wrath] PainThrow não foi encontrado no prefab.");
                Destroy(pain);
                return null;
            }

            RotateTowards(target.transform.position);

            Vector3 direction = -(target.transform.position - transform.position).normalized;

            projectile.Launch(direction, _throwForce);
            return pain;
        }

        public GameObject GetFarthestPlayer()
        {
            PlayerActor[] players = FindObjectsByType<PlayerActor>(FindObjectsSortMode.None);

            GameObject farthest = null;
            float maxDistance = 0f;

            foreach(var player in players)
            {
                float distance = Vector3.Distance(transform.position, player.transform.position);
                if(distance > maxDistance)
                {
                    maxDistance = distance;
                    farthest = player.gameObject;
                }
            }

            return farthest;
        }
        public void PlayPickupAnimation()
        {
            //método para animação de pegar o machado Dor
        }



        private void OnDrawGizmos()
        {
            
            if(IsAttacking && CurrentAttack == AttackType.Spin)
            {
                Gizmos.color = Color.red;
                Gizmos.DrawWireSphere(transform.position, 3.5f);
            }

        }
    }

    
}