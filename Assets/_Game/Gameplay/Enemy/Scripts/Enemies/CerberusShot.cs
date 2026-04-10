using Pong.Gameplay.Player;
using UnityEngine;

namespace Pong.Gameplay.Enemy
{
    public class CerberusShot : MonoBehaviour
    {
        private Vector3 _direction;
        private int _damage;
        [SerializeField] private float _speed;

        public void Initialize(Vector3 postion, Vector3 direction, int damage)
        {
            gameObject.SetActive(true);
            transform.position = postion;
            _direction = direction;
            _damage = damage;

        }

        void FixedUpdate()
        {
            transform.Translate(_direction * _speed * Time.fixedDeltaTime);
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.TryGetComponent<PlayerController>(out PlayerController player))
            {

                // causar dano aos jogadores

            }

            gameObject.SetActive(false);
        }


    }
}
