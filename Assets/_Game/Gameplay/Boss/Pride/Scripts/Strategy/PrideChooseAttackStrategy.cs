using Pong.Framework.BehaviourTree;
using UnityEngine;

namespace Pong.Gameplay.Boss
{
    public class PrideChooseAttackStrategy : INodeStrategy
    {
        private readonly PrideBoss _boss;

        public PrideChooseAttackStrategy(PrideBoss boss)
        {
            _boss = boss;
        }

        public Node.Status Process()
        {
            _boss.SetAttack(PrideBoss.AttackType.Vine);
            _boss.StartAttack();

            Debug.Log("[Pride] Escolheu ataque: Vine");
            return Node.Status.Success;
        }
    }
}