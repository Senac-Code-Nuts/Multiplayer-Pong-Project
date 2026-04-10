using System.Collections;
using UnityEngine;

namespace Pong.Gameplay.Enemy
{
    public class CerberusEnemy : EnemyActor
    {
        [Header("Specific Attributes")]
        [SerializeField] private float _preAttackTime;
        [SerializeField] private float _atackCooldown;
        [SerializeField] private Renderer _renderer;

        [Header("Shots Attributs")]
        [SerializeField] private int _numberOfShots;
        [SerializeField] private GameObject _shotPrefab;
        [SerializeField] private CerberusShot[] _shots;
        [SerializeField] private Transform _targetDirection;
        [SerializeField] private float _angleDiff;

        protected override void Awake()
        {
            base.Awake();
            _shots = new CerberusShot[_numberOfShots];

            for (int i = 0; i < _numberOfShots; i++)
            {
                var shot = Instantiate(_shotPrefab);
                _shots[i] = shot.GetComponent<CerberusShot>();
                shot.SetActive(false);
            }

        }
        private void OnEnable()
        {
            StartCoroutine(ActCoroutine());
        }

        private void OnDisable()
        {
            StopAllCoroutines();
        }

        private void ExecutePreAttack()
        {

            _renderer.material.color = Color.yellow;

        }

        public override void ExecuteAttack()
        {
            _renderer.material.color = Color.grey;

            for (int i = 0; i < _numberOfShots; i++)
            {

                float dirX = _targetDirection.position.x - transform.position.x;
                float dirZ = _targetDirection.position.z - transform.position.z;

                float angleDiff = _angleDiff - (i * _angleDiff);

                Vector3 direction = new Vector3(dirX, 0, dirZ);
                Vector3 finalDirection = Quaternion.AngleAxis(angleDiff, Vector3.up) * direction;

                _shots[i].Initialize(transform.position, finalDirection.normalized, _damage);

            }
        }

        private IEnumerator ActCoroutine()
        {

            ExecutePreAttack();

            yield return new WaitForSecondsRealtime(_preAttackTime);

            ExecuteAttack();

            yield return new WaitForSecondsRealtime(_atackCooldown);

            StopAllCoroutines();
            StartCoroutine(ActCoroutine());

        }
    }
}