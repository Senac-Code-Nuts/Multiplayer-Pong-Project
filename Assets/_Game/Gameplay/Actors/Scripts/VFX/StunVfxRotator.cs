using UnityEngine;

public class StunVfxRotator : MonoBehaviour
{
    [SerializeField] private float _rotationSpeed = 120f;

    private void Update()
    {
        transform.Rotate(Vector3.up * _rotationSpeed * Time.deltaTime, Space.World);
    }
}