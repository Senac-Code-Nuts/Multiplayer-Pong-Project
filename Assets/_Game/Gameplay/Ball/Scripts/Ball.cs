using UnityEngine;

namespace Pong.Gameplay.Ball
{
    public class Ball : MonoBehaviour
    {
        [SerializeField, Range(0f,10f)] private float _speed;
        private Vector3 _direction;
        private Rigidbody _rigidBody;


        private void Start()
        {
            _rigidBody = GetComponent<Rigidbody>();
            Launch();
        }

        private void Launch()
        {
            float dirX = Random.Range(-1f,1f);
            float dirZ = Random.Range(-1f, 1f);

            _direction = new Vector3(dirX, 0, dirZ).normalized;
            _rigidBody.linearVelocity = _direction * _speed;
        }

        private void FixedUpdate()
        {
            _rigidBody.linearVelocity = _rigidBody.linearVelocity.normalized * _speed;
        }

        private void OnCollisionEnter(Collision collision)
        {
            Vector3 normal = collision.contacts[0].normal;
            _direction = Vector3.Reflect(_direction,normal);


            _rigidBody.linearVelocity = _direction * _speed; 
        }
    }

}
