using UnityEngine;

namespace Pong.Systems.MapSelection
{
    public class Uimanager : MonoBehaviour
    {
        public static Uimanager Instance { get; private set; }
        private void Awake() 
        {
            if(Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;
        }

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
