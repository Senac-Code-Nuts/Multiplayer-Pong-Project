using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem.XR.Haptics;


namespace MenuManager{
public class MenuStateMachine : MonoBehaviour
{

   

    [SerializeField] private List<MenuState> _States = new List<MenuState>(); // list of all states in the machine

    [SerializeField] private MenuState _StarterState = null; // inital state, can be set to null

    private MenuState _CurrenteState; // the machines current state

    public MenuState currentState 
    {
            get {return _CurrenteState;}
            private set {_CurrenteState = value;} 
        }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (_StarterState == null)
            {
                _CurrenteState = null;
                return;   
            }
        _CurrenteState = _StarterState;
        _CurrenteState.LocalReady();
    }

    // Update is called once per frame
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
        if (OldState != _CurrenteState || OldState == null)
            {
                return;
            }
        var _NewState = _States.FirstOrDefault(o => o._StateType == NewState);
        _CurrenteState.OnChange();
        
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