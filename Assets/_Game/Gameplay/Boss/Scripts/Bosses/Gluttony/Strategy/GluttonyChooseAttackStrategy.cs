using Pong.Framework.BehaviourTree;
using UnityEngine;

namespace Pong.Gameplay.Boss
{
    public class GluttonyChooseAttackStrategy : INodeStrategy
    {
        private readonly GluttonyBoss _boss;

        public GluttonyChooseAttackStrategy(GluttonyBoss boss)
        {
            _boss = boss;
        }

        public Node.Status Process()
        {
            int choice = _boss.ChooseNextAttackIndex();

            _boss.SetAttack((GluttonyBoss.AttackType)choice);
            _boss.StartAttack();

            Debug.Log("[Gluttony] Escolheu ataque: " + choice);
            return Node.Status.Success;
        }
    }
}