using System.Data;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;


namespace MenuManager
{
    public abstract class MenuState : MonoBehaviour
    {

        public enum StateType
        {
            Padrao,
            config,
            credit
        }

        //reference to which state it is
        [SerializeField] private StateType _stateType;

        //get setter, unshure of its usability, 
        //kept in here while attempting to fix problem with type referencing
        public StateType _StateType
        {
            get {return _stateType;}
            private set {_StateType = value;} 
        }

        //the start func of a state
        public abstract void LocalStart();

        //the update func of a state, ]
        //will run as long as the state is currently running on the machine
        public abstract void LocalUpdate();

        //LocalUpdate, but for the FixedUpdate
        public abstract void LocalFixedUpdate();

        /*To be played whenever the state machine changes a state, 
        will be played BEFORE the UI goes invisible, keep that in mind*/
        public abstract void LocalExit();

        // when entering a new state, the state machine first activates it and THEN runs its ready func
        public void LocalReady()
        {
            gameObject.SetActive(true);
            LocalStart();
        }

        //will do the inverse when changing a state
        public void OnChange()
        {
            LocalExit();
            gameObject.SetActive(false);
        }

    }
}