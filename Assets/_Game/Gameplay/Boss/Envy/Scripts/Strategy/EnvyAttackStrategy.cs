using Pong.Framework.BehaviourTree;
using Pong.Gameplay.Boss;
using UnityEngine;

namespace Pong.Gameplay
{
    public class EnvyAttackStrategy : INodeStrategy
    {
        private EnvyBoss _boss;

        public EnvyAttackStrategy(EnvyBoss boss)
        {
            _boss = boss;
        }
        public Node.Status Process()
        {
            _boss.ExecuteAttack();
            if(!_boss.IsPreparingAttack)
            {
                return Node.Status.Success;
            }
            return Node.Status.Running;
        }
    }
}
