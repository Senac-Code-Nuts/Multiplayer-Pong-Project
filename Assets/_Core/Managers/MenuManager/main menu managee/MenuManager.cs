using MenuManager;
using Unity.VisualScripting;
using UnityEngine;

namespace MenuManager
{
    public class MainMenuManager : MonoBehaviour
    {


        private MenuStateMachine StateMachina;
        // Start is called once before the first execution of Update after the MonoBehaviour is created
        void Start()
        {
            StateMachina = GetComponent<MenuStateMachine>();
            Debug.Log(StateMachina.name);
        }

        public void ChangeToOptions()
        {
            StateMachina.ChangeState(StateMachina.currentState, MenuState.StateType.config);
        }

        public void ChangeToCredits()
        {
            StateMachina.ChangeState(StateMachina.currentState, MenuState.StateType.credit);
        }
        public void ChangeToMain()
        {
            StateMachina.ChangeState(StateMachina.currentState, MenuState.StateType.Padrao);
        }

        public void ChangeToNull()
        {
            StateMachina.ClearState();
        }
        public void Exit()
        {
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#else
            Application.Quit();
#endif
        }
    }
}