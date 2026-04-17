using Pong.Systems.Selection;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Pong.Systems.MapSelection
{
    public class MapButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, ISelectHandler, IDeselectHandler
    {
        private const string PHASE_KEY = "NewPhase";

        [Header("Next Scenes")]
        [Tooltip("Cena da sala para 2 jogadores")]
        [SerializeField] private string _scene2Players;

        [Tooltip("Cena da sala para 4 jogadores")]
        [SerializeField] private string _scene4Players;

        [Tooltip("Para ordenar qual será a fase de forma sequencial")]
        [SerializeField] private int _numberScene;

        [Header("Selection Session")]
        [SerializeField] private CharacterSelectionSession _selectionSession;

        [Header("UI Components")]
        [SerializeField] private Button _button;
        [SerializeField] private Image _buttonImage;

        [Header("Colors")]
        [SerializeField] private Color _colorActualphase = Color.white;
        [SerializeField] private Color _colorUnlockedPhase = Color.green;
        [SerializeField] private Color _colorCompletedPhase = Color.gray;
        [SerializeField] private Color _colorLockedPhase = Color.black;

        [Header("Selection")]
        [SerializeField] private float _selectScale = 1.2f;

        private Vector3 _defaultScale;

        /// <summary>
        /// (PlayerPrefs)
        /// GetInt vai retornar um valor pra mim da etiqueta
        /// SetInt vai salvar um novo valor na etiqueta
        /// PlayerPrefs.Save(); salva a info
        /// </summary>
        private void Awake()
        {
            _defaultScale = transform.localScale;

            if (_button == null)
                _button = GetComponent<Button>();

            if (_buttonImage == null)
                _buttonImage = GetComponent<Image>();
        }

        /// <summary>
        /// Estado BLOQUEADO:
        /// pode ficar invisível ou apenas desativado
        /// </summary>
        public void SetLocked(bool shouldHide)
        {
            SetVisualState(false, _colorLockedPhase, !shouldHide);
        }

        /// <summary>
        /// Estado LIBERADO:
        /// jogador pode clicar
        /// </summary>
        public void SetUnlocked()
        {
            SetVisualState(true, _colorUnlockedPhase, true);
        }

        /// <summary>
        /// Estado CONCLUÍDO:
        /// fica visível, mas não clicável
        /// </summary>
        public void SetCompleted()
        {
            SetVisualState(false, _colorCompletedPhase, true);
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            if (!CanHighlight())
                return;

            transform.localScale = _defaultScale * _selectScale;
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            ResetScale();
        }

        public void OnSelect(BaseEventData data)
        {
            if (!CanHighlight())
                return;

            transform.localScale = _defaultScale * _selectScale;
        }

        public void OnDeselect(BaseEventData data)
        {
            ResetScale();
        }

        public void ToNextScene()
        {
            string targetScene = GetSceneByPlayerCount();

            if (string.IsNullOrEmpty(targetScene))
            {
                Debug.LogError($"Nao foi possivel definir a cena da fase {name} para a quantidade atual de jogadores.");
                return;
            }

            if (MapProgressManager.Instance != null)
            {
                MapProgressManager.Instance.SetCurrentPhase(_numberScene);
            }

            SceneManager.LoadScene(targetScene);
        }

        public void DeleteAllSaves()
        {
            PlayerPrefs.DeleteKey(PHASE_KEY);
            PlayerPrefs.Save();
        }

        private string GetSceneByPlayerCount()
        {
            int registeredPlayers = GetRegisteredPlayerCount();

            if (registeredPlayers == 2)
                return _scene2Players;

            if (registeredPlayers == 4)
                return _scene4Players;

            Debug.LogWarning($"Quantidade de jogadores invalida para carregar sala: {registeredPlayers}");
            return string.Empty;
        }

        private int GetRegisteredPlayerCount()
        {
            if (_selectionSession == null)
                return 0;

            int registeredPlayers = 0;

            for (int i = 0; i < _selectionSession.PlayerSelections.Length; i++)
            {
                if (_selectionSession.PlayerSelections[i].IsRegistered)
                    registeredPlayers++;
            }

            return registeredPlayers;
        }

        private void SetVisualState(bool interactable, Color color, bool isVisible)
        {
            if (_button != null)
                _button.interactable = interactable;

            if (_buttonImage != null)
                _buttonImage.color = color;

            gameObject.SetActive(isVisible);

            if (!interactable)
                ResetScale();
        }

        private bool CanHighlight()
        {
            if (_button == null)
                return false;

            return _button.interactable;
        }

        private void ResetScale()
        {
            transform.localScale = _defaultScale;
        }
    }
}