using UnityEngine;
using UnityEngine.UI;

namespace Pong.Gameplay.Life
{
    public class LifeManagerHud : MonoBehaviour
    {
        [SerializeField] private LifeManager _lifeManager;
        [SerializeField] private Image _fillImage;

        private void OnEnable()
        {
            LifeManager.OnLifeChangeEvent += UptadeLifeHud;            
        }

        private void OnDisable()
        {
            LifeManager.OnLifeChangeEvent -= UptadeLifeHud;            
        }

        private void UptadeLifeHud(int newLife)
        {           
            float fill = (float)newLife / (float)_lifeManager.MaxLife;            

            _fillImage.fillAmount = fill;
        }
    }
}
