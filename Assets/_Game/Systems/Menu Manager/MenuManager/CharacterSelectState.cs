using UnityEngine;
using Pong.Systems.Selection;

namespace MenuManager
{
    public class CharacterSelectState : MenuState
    {
        [SerializeField] private CharacterSelectionManager _selectionManager;
        [SerializeField] private CharacterSelectionInputHandler _inputHandler;
        [SerializeField] private CutsceneController _cutsceneController;
        [SerializeField] private GameObject _selectionPanel;

        private bool _isLoadingScene = false;

        public override void LocalStart()
        {
            _isLoadingScene = false;

            if (_selectionManager != null)
            {
                _selectionManager.ClearSelections();
            }

            if (_inputHandler != null)
            {
                _inputHandler.ResetInputState();
            }

            if (_selectionPanel != null)
            {
                _selectionPanel.SetActive(true);
            }
        }

        public override void LocalUpdate()
        {
            if (_selectionManager == null) return;
            if (_isLoadingScene) return;
            if (!_selectionManager.CanStartMatch()) return;

            _isLoadingScene = true;

            if (_selectionPanel != null)
            {
                _selectionPanel.SetActive(false);
            }

            if (_cutsceneController != null)
            {
                _cutsceneController.PlayCutscene();
                return;
            }

            Debug.LogWarning("[CharacterSelectState] CutsceneController não configurado.");
        }

        public override void LocalFixedUpdate()
        {
        }

        public override void LocalExit()
        {
            _isLoadingScene = false;
        }
    }
}