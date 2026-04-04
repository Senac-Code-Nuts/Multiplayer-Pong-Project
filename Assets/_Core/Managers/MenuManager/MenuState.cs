using System.Data;
using Unity.VisualScripting;
using UnityEngine;


namespace MenuManager
{
    public abstract class MenuState : MonoBehaviour
    {
        [SerializeField] private MenuStateMachine.StateType _stateType;

        public MenuStateMachine.StateType _StateType
        {
            get {return _stateType;}
            private set {_StateType = value;} 
        }
        public abstract void LocalStart();

        public abstract void LocalUpdate();

        public abstract void OnChange();


    }
}