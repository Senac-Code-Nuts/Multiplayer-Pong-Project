using UnityEngine;
using Pong.Gameplay.Player;

public class ShieldTestDamage : MonoBehaviour
{
    [SerializeField] private PlayerActor _targetPlayer;
    [SerializeField] private int _damage = 1;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.T))
        {
            if (_targetPlayer != null)
            {
                _targetPlayer.ApplyDamage(_damage);
                Debug.Log($"Applied {_damage} damage to {_targetPlayer.gameObject.name}");
            }
        }
    }
}