using UnityEngine;

namespace Pong.Gameplay.Life
{
    public class LifeManager : MonoBehaviour
    {
        private static int _life;
        [SerializeField] private int _maxLife;
        public static int Life => _life;
        public int MaxLife => _maxLife;

        public delegate void OnLifeChangeDelegate(int changeValue);
        public static event OnLifeChangeDelegate OnLifeChangeEvent;

        public delegate void OnDieDelegate();
        public static event OnDieDelegate OnDieEvent;

        private void Awake()
        {
            _life = _maxLife;
        }

        public static void ApplyDamage(int damage)
        {
            if (damage == 0) return;

            int newLife = _life - damage;

            if (newLife <= 0)
            {
                _life = 0;
                Die();
                return;
            }

            _life = newLife;            

            // checar se o evento tem algum listener
            if (OnLifeChangeEvent != null)
            {
                OnLifeChangeEvent.Invoke(newLife);
            }           
        }        

        private static void Die()
        {
            // checa se o evento tem algum listener
            if (OnDieEvent != null) { 
            
                OnDieEvent.Invoke();
            
            }

            if (OnLifeChangeEvent != null) {

                OnLifeChangeEvent(0);

            }
        }
    }
}
