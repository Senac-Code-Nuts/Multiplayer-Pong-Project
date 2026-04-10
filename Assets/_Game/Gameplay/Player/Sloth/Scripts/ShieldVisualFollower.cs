using UnityEngine;

namespace Pong.Gameplay.Player
{
    public class ShieldVisualFollower : MonoBehaviour
    {
        private Transform _target;
        private Vector3 _offset;

        public void Initialize(Transform target, Vector3 offset)
        {
            _target = target;
            _offset = offset;
        }

        private void LateUpdate()
        {
            if (_target == null)
            {
                Destroy(gameObject);
                return;
            }

            transform.position = _target.position + _offset;
        }
    }
}