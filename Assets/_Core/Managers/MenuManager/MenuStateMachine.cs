using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;


namespace MenuManager{
public class MenuStateMachine : MonoBehaviour
{

    public enum StateType
        {
            Padrao,
            config,
            credit
        }

    [SerializeField] private List<MenuState> _States = new List<MenuState>(); // list of all states in the machine

    [SerializeField] private MenuState _StarterState = null; // inital state, can be set to null

    private MenuState _CurrenteState; // the machines current state

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (_StarterState == null)
            {
                _CurrenteState = null;
                return;   
            }
        _CurrenteState = _StarterState;
        _CurrenteState.LocalStart();
    }

    // Update is called once per frame
    void Update()
    {
        if (_CurrenteState != null)
            {
                _CurrenteState.LocalUpdate();
            }
    }

    void ChangeState(MenuState OldState, StateType NewState)
    {
        if (OldState != _CurrenteState)
            {
                return;
            }

        var _NewState = _States.FirstOrDefault(o => o._StateType == NewState);

        _CurrenteState.OnChange();

        _NewState.LocalStart();

        _CurrenteState = _NewState;

    }

}
}