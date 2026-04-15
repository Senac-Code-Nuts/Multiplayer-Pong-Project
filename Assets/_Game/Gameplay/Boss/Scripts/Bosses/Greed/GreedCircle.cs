using UnityEngine;

namespace Pong.Gameplay.Boss.Greed
{
    public class GreedCircle : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private GreedBoss _greedBoss;
        [SerializeField] private Treasure _treasure;


        [Header("Movement Settings")]
        [SerializeField] private float _radius;
        [SerializeField] private float _followSpeed;
        [SerializeField] private Vector3 _offset = new Vector3(0, 0.1f, 0);

        //private SphereCollider _sphereCollider;
        private void Start()
        {
            //_sphereCollider = GetComponent<SphereCollider>();
            _followSpeed = _greedBoss.PatrolSpeed + (_greedBoss.PatrolSpeed * 0.1f);
        }
        private void LateUpdate()
        {
            if (_greedBoss == null || _treasure == null) return;

            HandleMovement();
            HandleBossProtection();
        }

        private void HandleMovement()
        {
            if (_greedBoss.IsTouched)
            {
                Vector3 targetPos = _treasure.transform.position + _offset;
                transform.position = Vector3.Lerp(transform.position, targetPos, _followSpeed * Time.deltaTime);
            }
            else
            {
                Vector3 targetPos = _greedBoss.transform.position + _offset;
                transform.position = Vector3.Lerp(transform.position, targetPos, _followSpeed * Time.deltaTime);
            }
        }

        private void HandleBossProtection()
        {
            float distanceToBoss = Vector3.Distance(transform.position, _greedBoss.transform.position);
            if (distanceToBoss <= _radius)
            {
                _greedBoss.SetVulnerability(false);
                //_sphereCollider.enabled = true;
            }
            else
            {
                //_sphereCollider.enabled = false;
                _greedBoss.SetVulnerability(true);
            }
        }
    }
}