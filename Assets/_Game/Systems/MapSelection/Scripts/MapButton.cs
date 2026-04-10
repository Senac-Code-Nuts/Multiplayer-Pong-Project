using UnityEngine;
using UnityEngine.Experimental.Playables;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Pong.Systems.MapSelection
{
    public class MapButton : MonoBehaviour
    {
        [Header("Proxima Cena")]
        [Tooltip("Coloque o nome CORRETO da cena que será chamada")]
        [SerializeField] private string _scene;
        [SerializeField] private Button _button;

        public void ToNextScene()
        {
            if (!string.IsNullOrEmpty(_scene)) 
            {
                SceneManager.LoadScene(_scene);
            }
            else
            {
                Debug.LogError("Voce colocou o nome de uma cena que não existe ou voce só não escreveu mesmo!");
            }
        }

    }
}
