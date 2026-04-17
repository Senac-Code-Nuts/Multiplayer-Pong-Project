using UnityEngine;

namespace Pong.Gameplay.Boss.Greed
{
    public class Treasure : MonoBehaviour
    {
        private GreedBoss _boss;

        [Header("Follow")]
        [SerializeField] private Vector3 _localOffset = new Vector3(0.8f, 0.5f, 0f);

        [Header("Visual")]
        [SerializeField] private Renderer _renderer;
        [SerializeField] private Color _normal = Color.yellow;
        [SerializeField] private Color _hit = Color.red;

        public void SetBoss(GreedBoss boss)
        {
            _boss = boss;
        }

        private void LateUpdate()
        {
            if (_boss == null)
                return;

            transform.position = _boss.transform.position + _localOffset;

            if (_renderer != null)
            {
                _renderer.material.color = _boss.IsTouched ? _hit : _normal;
            }
        }

        private void OnCollisionEnter(Collision collision)
        {
            if (!collision.gameObject.CompareTag("Relic"))
                return;

            Debug.Log("[Treasure] Hit!");

            _boss.IsTouched = true;
            _boss.SetVulnerability(true);
        }
    }
}