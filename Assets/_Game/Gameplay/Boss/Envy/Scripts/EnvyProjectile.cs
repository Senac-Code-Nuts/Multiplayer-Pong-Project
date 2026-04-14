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
            if(_hasHit) return;
            _hasHit = true;

            if(other.gameObject.CompareTag("Player"))
            {
                var playerActor = other.gameObject.GetComponent<PlayerActor>();
                if(playerActor != null)
                {
                    playerActor.ApplyDamage(_projectileDamage);
                } 
            }else if(!other.gameObject.CompareTag("Boss"))
            {
              Destroy(gameObject);  
            }
            
        }



    }
}
