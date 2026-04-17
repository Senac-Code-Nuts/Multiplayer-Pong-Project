using UnityEngine;
using Pong.Gameplay.Player;

namespace Pong.Gameplay.Boss
{
    public class PainThrow : MonoBehaviour
    {
        [SerializeField] private int _painDamage;
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

            PlayerActor playerActor = collision.gameObject.GetComponentInParent<PlayerActor>();

            if(collision.gameObject.CompareTag("Wall") || playerActor != null)
            {
                _isStuck = true;
                
                _rigidBody.linearVelocity = Vector3.zero;
                _rigidBody.isKinematic = true;

                if(playerActor != null)
                {
                    playerActor.ApplyDamage(_painDamage);
                }

                transform.SetParent(playerActor != null ? playerActor.transform : collision.transform);

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
