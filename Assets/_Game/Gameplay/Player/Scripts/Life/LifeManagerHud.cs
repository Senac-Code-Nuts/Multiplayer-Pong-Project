using UnityEngine;
using UnityEngine.UI;

namespace Pong.Gameplay.Life
{
    public class LifeManagerHud : MonoBehaviour
    {
        [SerializeField] private LifeManager _lifeManager;
        [SerializeField] private Slider _lifeSlider;

        private void Start()
        {
            _lifeSlider.maxValue = _lifeManager.MaxLife;
            _lifeSlider.value = _lifeManager.Life;
        }

        private void OnEnable()
        {
            LifeManager.OnLifeChangeEvent += OnLifeChangeDelegate;            
        }

        private void OnDisable()
        {
            LifeManager.OnLifeChangeEvent -= OnLifeChangeDelegate;            
        }

        public void OnLifeChangeDelegate(int currentLife)
        {
            _lifeSlider.value = currentLife;
        }
    }
}
