using UnityEngine;

namespace Pong.Systems.MapSelection
{
    public class Uimanager : MonoBehaviour
    {
        #region Singleton
        private static Uimanager _instance;
        void Start() 
        {
            if(_instance == null)
            {
                _instance = this;
            }  
        }
        public static Uimanager Instance => _instance;
        #endregion

        public void Show(CanvasGroup canvasGroup)
        {
            canvasGroup.alpha = 1;
            canvasGroup.interactable = true;
            canvasGroup.blocksRaycasts = true;
        }

        public void Hide(CanvasGroup canvasGroup)
        {
            canvasGroup.alpha = 0;
            canvasGroup.interactable = false;
            canvasGroup.blocksRaycasts = false;
        }
    }
}
