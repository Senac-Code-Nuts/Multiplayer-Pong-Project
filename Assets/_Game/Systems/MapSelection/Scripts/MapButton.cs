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
        [Tooltip("Para ordenar qual será a fase de forma sequencial")]
        [SerializeField] private int _numberScene;

        [Header("Componentes da UI")]
        [SerializeField] private Button _button;
        [SerializeField] private Image _buttonImage;

        [Header("Cores")]
        [SerializeField] private Color _colorActualphase = Color.white;
        [SerializeField] private Color _colorUnlockedPhase = Color.green;
        [SerializeField] private Color _colorLockedPhase = Color.black;
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
        private void ButtonSetting()
        {
            int _phase = PlayerPrefs.GetInt("NewPhase", 0);
            if(_numberScene == _phase)
            {
                ActiveButton(false, _colorActualphase);
            }
            else if(_numberScene == _phase + 1)
            {
                ActiveButton(true, _colorUnlockedPhase);
            }
            else
            {
                ActiveButton(false, _colorLockedPhase);
            }
        }
        private void ActiveButton(bool interactable, Color color)
        {
            if (_button != null) 
            {
                _button.interactable = interactable;
            }
            if (_buttonImage != null) 
            {
                _buttonImage.color = color;
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

        public void DeleteAllSaves()
        {
            if (!string.IsNullOrEmpty(_scene))
            {
                PlayerPrefs.DeleteAll();
                SceneManager.LoadScene(_scene);
            }
        }
    }
}
