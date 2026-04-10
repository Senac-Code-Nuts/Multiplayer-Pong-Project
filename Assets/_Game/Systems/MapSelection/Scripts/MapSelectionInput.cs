using UnityEngine;
using UnityEngine.InputSystem;

namespace Pong.Systems.MapSelection
{
    public class MapSelectionInput : MonoBehaviour
    {
        [SerializeField] private InputActionReference _mapInput;
        [SerializeField] private CanvasGroup _MapSelectionScreen;
        private bool _isMapOpen = false;
        private void OnEnable()
        {
            if (_mapInput != null)
            {
                _mapInput.action.performed += OnMapPressed;
            }
        }
        private void OnDisable()
        {
            if (_mapInput != null) 
            {
                _mapInput.action.performed -= OnMapPressed;
            }
        }
        private void OnMapPressed(InputAction.CallbackContext context)
        {
            _isMapOpen = !_isMapOpen;
            if (_isMapOpen)
            {
                Uimanager.Instance.Show(_MapSelectionScreen);
            }
            else
            {
                Uimanager.Instance.Hide(_MapSelectionScreen);
            }
        }
    }
}