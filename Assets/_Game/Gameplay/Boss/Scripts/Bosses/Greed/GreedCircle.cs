using UnityEngine;

namespace Pong.Gameplay.Boss.Greed
{
    public class GreedCircle : MonoBehaviour
    {
        private GreedBoss _boss;

        [SerializeField] private Transform _target;
        [SerializeField] private float _radius = 3f;
        [SerializeField] private float _followSpeed = 5f;

        [Header("Visual")]
        [SerializeField] private Renderer _renderer;
        [SerializeField] private Color _protected = Color.blue;
        [SerializeField] private Color _vulnerable = Color.red;

        public void SetBoss(GreedBoss boss)
        {
            _boss = boss;
        }

        private void Update()
        {
            Follow();
            HandleProtection();
            UpdateVisual();
        }

        private void Follow()
        {
            if (_target == null)
                return;

            transform.position = Vector3.Lerp(
                transform.position,
                _target.position,
                _followSpeed * Time.deltaTime
            );
        }

        private void HandleProtection()
        {
            if (_boss == null)
                return;

            if (_boss.IsTouched)
                return;

            float dist = Vector3.Distance(transform.position, _boss.transform.position);

            _boss.SetVulnerability(dist > _radius);
        }

        private void UpdateVisual()
        {
            if (_renderer == null || _boss == null)
                return;

            _renderer.material.color = _boss.IsTouched ? _vulnerable : _protected;
        }

        private void OnDrawGizmos()
        {
            Gizmos.color = Color.cyan;
            Gizmos.DrawWireSphere(transform.position, _radius);
        }
    }
}