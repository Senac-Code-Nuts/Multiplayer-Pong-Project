using UnityEngine;
using UnityEngine.InputSystem;

public class Player_Controller : MonoBehaviour
{
    [SerializeField] float _speed = 10f;
    private Vector2 _moveInput;

    public void OnMove(InputValue value)
    {
        _moveInput = value.Get<Vector2>();
    }

    void FixedUpdate()
    {
        Vector3 movimentacao = new Vector3(_moveInput.x, 0 , _moveInput.y);
        
        transform.Translate(movimentacao * _speed * Time.deltaTime);
    }
}
