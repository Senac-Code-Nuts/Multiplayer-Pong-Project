using UnityEngine;

namespace Pong.Gameplay.Boss
{
    public class PainThrow : MonoBehaviour
    {
        private Rigidbody _rigidBody;
        private bool _isStuck = false;

        public void Launch(Vector3 direction, float force)
        {
            _rigidBody = GetComponent<Rigidbody>();
            _rigidBody.isKinematic = false;
            _rigidBody.AddForce(direction * force, ForceMode.Impulse);

        }

        private void OnCollisionEnter(Collision collision)
        {
            if(_isStuck) return;

            if(collision.gameObject.CompareTag("Wall") || collision.gameObject.CompareTag("Player"))
            {
                _isStuck = true;
                
                _rigidBody.linearVelocity = Vector3.zero;
                _rigidBody.isKinematic = true;

                transform.SetParent(collision.transform);

            }
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Boss") && _isStuck)
            {
                Destroy(gameObject);
            }
        }
    }
}
