using UnityEngine;
using UnityEngine.UI;

namespace Pong.Gameplay.Life
{
    public class LifeManagerHud : MonoBehaviour
    {
        [SerializeField] private LifeManager _lifeManager;
        [SerializeField] private Slider _lifeSlider;
        [SerializeField] private GameObject _hudContainer;

        private void Awake()
        {
            SetVisible(false);
        }

        private void Start()
        {
            RefreshHud();
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

        public void SetVisible(bool isVisible)
        {
            if (_hudContainer.activeSelf == isVisible)
            {
                if (isVisible)
                {
                    RefreshHud();
                }

                return;
            }

            _hudContainer.SetActive(isVisible);
        }

        private void RefreshHud()
        {
            if (_lifeManager == null || _lifeSlider == null)
            {
                return;
            }

            _lifeSlider.maxValue = _lifeManager.MaxLife;
            _lifeSlider.value = _lifeManager.Life;
        }
    }
}
