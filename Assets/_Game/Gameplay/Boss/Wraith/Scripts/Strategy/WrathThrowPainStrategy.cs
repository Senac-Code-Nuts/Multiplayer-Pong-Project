using Pong.Framework.BehaviourTree;
using Pong.Gameplay.Boss;
using UnityEngine;

namespace Pong.Gameplay.Boss
{
    public class WrathThrowPainStrategy : INodeStrategy
    {
        private WrathBoss _boss;
        private GameObject _currentPain;
        private float _timer;

        private enum ThrowState
        {
            Telegraph,
            Throwing,
            PostThrowDelay,
            MovingToPain,
            PickingUp
            
            
        }

        private ThrowState _state;

        public WrathThrowPainStrategy(WrathBoss boss)
        {
            _boss = boss;
            Reset();
        }

        public Node.Status Process()
        {
            _timer += Time.deltaTime;
            
            switch(_state)
            {
                case ThrowState.Telegraph:
                    return HandleTelegraph();

                case ThrowState.Throwing:
                    return HandleThrow();

                case ThrowState.MovingToPain:
                    return HandleMoveToPain();

                case ThrowState.PickingUp:
                    return HandlePickUp();

                case ThrowState.PostThrowDelay:
                    return HandlePostThrowDelay();

            }

            return Node.Status.Failure;
        }

        private Node.Status HandleTelegraph()
        {
            _boss.StopMovement();
            GameObject target = _boss.GetFarthestPlayer();
            if(target != null)
            {
                _boss.RotateTowards(target.transform.position);
            }

            if(_timer >= _boss.PainTelegraphTime)
            {
                _state = ThrowState.Throwing;
            }
            return Node.Status.Running;
        }

        private Node.Status HandleThrow()
        {
            
            _currentPain = _boss.SpawnPain();
            _timer = 0;
            _state = ThrowState.PostThrowDelay;

            return Node.Status.Running;
        }

        private Node.Status HandlePostThrowDelay()
        {
            if(_timer >= _boss.PostThrowDelay)
            {
                _state = ThrowState.MovingToPain;
            }

            return Node.Status.Running;  
        }
        
        private Node.Status HandleMoveToPain()
        {
            if(_currentPain == null)
            {
                _state = ThrowState.PickingUp;
                return Node.Status.Running;
            }

            _boss.MoveTo(_currentPain.transform.position);
            return Node.Status.Running;
        }

        private Node.Status HandlePickUp()
        {
            _boss.EndAttack();
            _boss.PlayPickupAnimation();
            _boss.AllowMovement();

            Reset();
            return Node.Status.Success;
        }

        public void Reset()
        {
            _state = ThrowState.Telegraph;
            _timer = 0f;
            _currentPain = null;
        }
    }
}
