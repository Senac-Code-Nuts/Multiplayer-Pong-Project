using System.Data;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UIElements;


namespace MenuManager
{
    public abstract class MenuState : MonoBehaviour
    {

        public enum StateType
        {
            Null,
            Padrao,
            config,
            credit
        }

        /// <summary>
        /// reference to which state it is
        /// </summary>
        [SerializeField] private StateType _stateType;
        [SerializeField] private GameObject _FirstSelectedScreenButton;

        /// <summary>
        /// get setter, unsure of its usability, 
        /// kept in here while attempting to fix problem with type referencing
        /// </summary>
        public StateType _StateType
        {
            get { return _stateType; }
            private set { _StateType = value; }
        }

        public GameObject _firstButton
        {
            get {return _FirstSelectedScreenButton;}
            private set {_FirstSelectedScreenButton = value;}
        }

        /// <summary>
        /// the start func of a state
        /// </summary>
        public abstract void LocalStart();

        /// <summary>
        /// the update func of a state, will run as long as the state is currently running on the machine
        /// </summary>
        public abstract void LocalUpdate();

        /// <summary>
        /// LocalUpdate, but for the FixedUpdate
        /// </summary>
        public abstract void LocalFixedUpdate();

        /// <summary>
        /// To be played whenever the state machine changes a state, will be played BEFORE the UI goes invisible, keep that in mind
        /// </summary>
        public abstract void LocalExit();

        /// <summary>
        /// when entering a new state, the state machine first activates it and THEN runs its ready func
        /// </summary>
        public void LocalReady()
        {
            gameObject.SetActive(true);
            LocalStart();
        }

        /// <summary>
        /// will do the inverse when changing a state
        /// </summary>
        public void OnChange()
        {
            LocalExit();
            gameObject.SetActive(false);
        }

        public void SelectThat(GameObject butt)
        {
            EventSystem.current.SetSelectedGameObject(butt);
        }

    }
}