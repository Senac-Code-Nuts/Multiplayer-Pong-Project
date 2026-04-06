using MenuManager;
using Unity.VisualScripting;
using UnityEngine;

namespace MenuManager{
public class MainMenuManager : MonoBehaviour
{


    private MenuStateMachine StateMachina;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        StateMachina = GetComponentInChildren<MenuStateMachine>();
        Debug.Log(StateMachina.name);
    }

    public void change_to_options()
    {
        StateMachina.ChangeState(StateMachina.currentState,MenuState.StateType.config);
    }

    public void change_to_credits()
    {
        StateMachina.ChangeState(StateMachina.currentState,MenuState.StateType.credit);
    }
    public void change_to_main()
    {
        StateMachina.ChangeState(StateMachina.currentState,MenuState.StateType.Padrao);
    }

    public void change_to_null()
        {
            StateMachina.ClearState();
        }
    // Update is called once per frame
    void Update()
    {
        
    }
}
}