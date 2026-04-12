using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Pong.Systems.MapSelection
{
    public class MapButton : MonoBehaviour
    {
        [Header("Proxima Cena")]
        [Tooltip("Coloque o nome CORRETO da cena que será chamada")]
        [SerializeField] private string _scene;
        [SerializeField] private int _numberScene;

        [Header("Componentes")]
        [SerializeField] private Button _Button;
        [SerializeField] private Image _ButtonImage;

        [Header("Cores")]
        [SerializeField] private Color _colorActualphase = Color.white;
        [SerializeField] private Color _colorUnlockedPhase = Color.green;
        [SerializeField] private Color _colorlockedPhase = Color.black;
        /// <summary> 
        /// (PlayerPrefs)
        /// GetInt vai retornar um valor pra mim da etiqueta
        /// Setint vai salvar um novo valor na etiqueta
        /// PlayerPrefs.Save(); salvar a info
        /// </summary>
        private void Start()
        {
            ButtonSetting();    
        }
        public void ButtonSetting()
        {
            int Phase = PlayerPrefs.GetInt("NewPhase", 0);
            if(_numberScene == Phase)
            {
                activeButton(false, _colorActualphase);
            }
            else if(_numberScene == Phase + 1)
            {
                activeButton(true, _colorUnlockedPhase);
            }
            else
            {
                activeButton(false, _colorlockedPhase);
            }

        }
        private void activeButton(bool Isinteractable, Color color)
        {
            if (_Button != null) 
            {
                _Button.interactable = Isinteractable;
            }
            if (_ButtonImage != null) 
            {
                _ButtonImage.color = color;
            }
        }
        
        public void ToNextScene()
        {
            if (!string.IsNullOrEmpty(_scene)) 
            {
                PlayerPrefs.SetInt("NewPhase",_numberScene);
                PlayerPrefs.Save(); 
                SceneManager.LoadScene(_scene);
            }
            else
            {
                Debug.LogError("Voce colocou o nome de uma cena que não existe ou voce só não escreveu mesmo!");
            }
        }

        public void DeleteAllScenes()
        {
            if (!string.IsNullOrEmpty(_scene))
            {
                PlayerPrefs.DeleteAll();
                SceneManager.LoadScene(_scene);
            }
        }
    }
}
