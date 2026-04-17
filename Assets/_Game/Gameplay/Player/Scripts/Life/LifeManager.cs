using UnityEngine;

namespace Pong.Gameplay.Life
{
    public class LifeManager : MonoBehaviour
    {
        public static LifeManager Instance { get; private set; }

        private int _life;
        private int _maxLife = 3;
        public int Life => _life;
        public int MaxLife => _maxLife;

        public delegate void OnLifeChangeDelegate(int changeValue);
        public static event OnLifeChangeDelegate OnLifeChangeEvent;

        public delegate void OnDieDelegate();
        public static event OnDieDelegate OnDieEvent;

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;

            _life = _maxLife;
        }

        public void ApplyDamage(int damage)
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
            OnLifeChangeEvent?.Invoke(newLife);
        }

        private void Die()
        {
            // checa se o evento tem algum listener
            OnDieEvent?.Invoke();

            OnLifeChangeEvent?.Invoke(0);
        }
    }
}
