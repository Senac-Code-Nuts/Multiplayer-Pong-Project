using MenuManager;
using UnityEngine;


namespace MenuManager
{

    public class MainState : MenuState
    {
        public override void LocalExit()
        {

        }

        public override void LocalFixedUpdate()
        {

        }

        public override void LocalStart()
        {
            SelectThat(_firstButton);
        }

        public override void LocalUpdate()
        {

        }
    }
}