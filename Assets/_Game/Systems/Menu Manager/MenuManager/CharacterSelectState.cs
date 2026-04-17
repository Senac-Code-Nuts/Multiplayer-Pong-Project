using MenuManager;
using Pong.Systems.Selection;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace MenuManager
{
    public class CharacterSelectState : MenuState
    {
        [SerializeField] private CharacterSelectionManager _selectionManager;
        [SerializeField] private CharacterSelectionInputHandler _inputHandler;
        [SerializeField] private string _combatSceneName;

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
        }

        public override void LocalUpdate()
        {
            if (_selectionManager == null) return;
            if (_isLoadingScene) return;

            Debug.Log("[CharacterSelectState] Updating");

            if (!_selectionManager.CanStartMatch()) return;

            Debug.Log("[CharacterSelectState] Starting match");

            _isLoadingScene = true;
            SceneManager.LoadScene(_combatSceneName);
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