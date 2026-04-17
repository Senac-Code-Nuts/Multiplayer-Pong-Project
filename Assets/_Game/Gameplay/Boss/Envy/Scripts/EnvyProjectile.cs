using Pong.Gameplay.Player;
using UnityEngine;

namespace Pong.Gameplay.Boss
{
    public class EnvyProjectile : MonoBehaviour
    {
        [SerializeField] private int _projectileDamage;
        [SerializeField] private float _projectileLifeTime;
        private float _lifetimeTimer = 0;
        private bool _hasHit = false;

        private void Update()
        {
           _lifetimeTimer += Time.deltaTime;
           if(_lifetimeTimer >= _projectileLifeTime)
            {
                Destroy(gameObject);
                _lifetimeTimer = 0;
            } 
        }

        private void OnTriggerEnter(Collider other)
        {
            PlayerActor playerActor = other.GetComponentInParent<PlayerActor>();

            if(playerActor != null)
            {
                if(!_hasHit)
                {
                    playerActor.ApplyDamage(_projectileDamage);

                    _hasHit = true;
                }
                Destroy(gameObject);

            }else if(!other.gameObject.CompareTag("Boss") && !other.gameObject.CompareTag("EnemyProjectile"))
            {
              Destroy(gameObject);  
            }
            
        }



    }
}
