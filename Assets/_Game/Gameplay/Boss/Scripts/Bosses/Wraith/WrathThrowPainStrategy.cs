using Pong.Framework.BehaviourTree;
using Pong.Gameplay.Boss;
using UnityEngine;

namespace Pong.Gameplay
{
    public class WrathThrowPainStrategy : INodeStrategy
    {
        private WrathBoss _boss;
        private GameObject _currentPain;

        public WrathThrowPainStrategy(WrathBoss boss)
        {
            _boss = boss;
        }

        public Node.Status Process()
        {
            if(_currentPain == null)
            {
                _boss.StopMovement();

                _currentPain = _boss.SpawnPain();

                return Node.Status.Running;
            }

            if(_currentPain != null)
            {
                _boss.MoveTo(_currentPain.transform.position);

                return Node.Status.Running;
            }

            _boss.PlayPickupAnimation();
            _currentPain = null;

            return Node.Status.Success;
        }

        public void Reset()
        {
            _currentPain = null;
        }
    }
}
