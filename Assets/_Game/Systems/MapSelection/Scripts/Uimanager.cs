using UnityEngine;

namespace Pong.Systems.MapSelection
{
    public class Uimanager : MonoBehaviour
    {
        #region Singleton
        public static Uimanager Instance {  get; private set; }
        private static Uimanager _instance;
        void Awake() 
        {
            if(_instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;
        }
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
