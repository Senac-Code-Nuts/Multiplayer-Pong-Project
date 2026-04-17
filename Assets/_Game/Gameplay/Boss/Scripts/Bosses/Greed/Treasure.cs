using Codice.CM.Common;
using Pong.Gameplay.Relics;
using UnityEngine;

namespace Pong.Gameplay.Boss.Greed
{
    public class Treasure : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private GreedBoss _greedBoss;
        [Tooltip("O ponto que o tesouro vai seguir")]
        [SerializeField] private Transform _newPoint; 

        [Header("Follow Settings")]
        [SerializeField] private float _smoothSpeed = 5f;
        [Tooltip("Ajustar a posição do tesouro caso não tenha")]
        [SerializeField] private Vector3 _offset = new Vector3(1f, -1f, 0);

        [Header("Arena Limits")]
        [SerializeField] private Transform _arenaCenter;
        [SerializeField] private float _arenaWidth;
        [SerializeField] private float _arenaLength;
        public bool teste;

        private void Update()
        {
            if (!_greedBoss.IsTouched)
            {
                Vector3 targetPos = _newPoint != null ? _newPoint.position : _greedBoss.transform.position + _offset;
                Vector3 clampedPos = RestrictToArena(targetPos);
                FollowTarget(clampedPos);
            }

            if (teste)
                _greedBoss.IsTouched = true;
        }

        private Vector3 RestrictToArena(Vector3 target)
        {
            if (_arenaCenter == null) return target;

            float minX = _arenaCenter.position.x - (_arenaWidth / 2);
            float maxX = _arenaCenter.position.x + (_arenaWidth / 2);
            float minZ = _arenaCenter.position.z - (_arenaLength / 2);
            float maxZ = _arenaCenter.position.z + (_arenaLength / 2);

            float clampedX = Mathf.Clamp(target.x, minX, maxX);
            float clampedZ = Mathf.Clamp(target.z, minZ, maxZ);
            return new Vector3(clampedX, target.y, clampedZ);
        }
        private void OnDrawGizmos()
        {
            if (_arenaCenter == null) return;
            Gizmos.color = Color.yellow;
            Vector3 size = new Vector3(_arenaWidth, 1f, _arenaLength);
            Gizmos.DrawWireCube(_arenaCenter.position, size);
        }

        private void FollowTarget(Vector3 targetPos)
        {
            transform.position = Vector3.Lerp(transform.position, targetPos, _smoothSpeed * Time.deltaTime);
        }

        private void OnCollisionEnter(Collision other)
        {
            if (other.gameObject.TryGetComponent<Relic>(out Relic relic)) 
            {
                if (_greedBoss != null)
                {
                        _greedBoss.IsTouched = true;
                        Debug.Log($"{relic.name} atingiu o tesouro");
                }
            }
        }
    }
}