using UnityEngine;

public class Bola : MonoBehaviour
{
    [SerializeField] private float _speed;
    private Vector3 _direction;
    private Rigidbody _rb;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _rb = GetComponent<Rigidbody>();
        Launch();
    }

    // Update is called once per frame
    void Update()
    {

    }

    void Launch()
    {
        float dirX = Random.Range(-1f,1f);
        float dirZ = Random.Range(-1f, 1f);

        _direction = new Vector3(dirX, 0, dirZ).normalized;
        _rb.linearVelocity = _direction * _speed;
    }

    void FixedUpdate()
    {
        _rb.linearVelocity = _rb.linearVelocity.normalized * _speed;
    }

    void OnCollisionEnter(Collision collision)
    {
       Vector3 normal = collision.contacts[0].normal;
       _direction = Vector3.Reflect(_direction,normal);


       _rb.linearVelocity = _direction * _speed; 
    }
}
