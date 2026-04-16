using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem.XR.Haptics;


namespace MenuManager
{
    public class MenuStateMachine : MonoBehaviour
    {


        /// <summary>
        /// list of all states in the machine
        /// </summary>
        [SerializeField] private List<MenuState> _States = new List<MenuState>();

        /// <summary>
        /// inital state, can be set to null
        /// </summary>
        [SerializeField] private MenuState.StateType _StarterState = MenuState.StateType.Null;

        /// <summary>
        /// the machines current state
        /// </summary>
        private MenuState _CurrenteState; 

        public MenuState currentState
        {
            get { return _CurrenteState; }
            private set { _CurrenteState = value; }
        }

        void Start()
        {
            if (_StarterState == MenuState.StateType.Null)
            {
                _CurrenteState = null;
                return;
            }
            else
            {
                ChangeState(null,_StarterState);
            }
            
        }


        void Update()
        {
            if (_CurrenteState != null)
            {
                _CurrenteState.LocalUpdate();
            }
        }

        void FixedUpdate()
        {
            if (_CurrenteState != null)
            {
                _CurrenteState.LocalFixedUpdate();
            }
        }
        public void ChangeState(MenuState OldState, MenuState.StateType NewState)
        {
            if (OldState != _CurrenteState)
            {
                return;
            }
            
            var _NewState = _States.FirstOrDefault(o => o._StateType == NewState);
            if (OldState != null)
            {
                _CurrenteState.OnChange();
            }

            _NewState.LocalReady();

            _CurrenteState = _NewState;

        }

        public void ClearState()
        {

            _CurrenteState.OnChange();
            _CurrenteState = null;
        }

    }
}