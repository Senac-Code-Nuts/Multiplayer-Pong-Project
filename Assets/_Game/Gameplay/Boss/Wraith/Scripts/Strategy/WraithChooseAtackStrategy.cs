using Pong.Framework.BehaviourTree;
using Pong.Framework.Strategy;
using UnityEngine;

namespace Pong.Gameplay.Boss
{
    public class WraithChooseAtackStrategy : INodeStrategy
    {
        private WrathBoss _boss;
        private int _lastAttack = -1;
        private int _repeatCount = 0;

        public WraithChooseAtackStrategy(WrathBoss boss)
        {
            _boss = boss;
        }
        public Node.Status Process()
        {
            int choice = Random.Range(0,2);

            if(choice == _lastAttack)
            {
                _repeatCount++;
                if(_repeatCount >= 2)
                {
                    choice = 1- choice;
                    _repeatCount = 0;
                }
            } else
            {
                _repeatCount = 0;
            }

            _lastAttack = choice;
            Debug.Log("ESCOLHEU ATAQUE: " + choice);

            _boss.SetAttack((WrathBoss.AttackType)choice);
            _boss.StartAttack();
            return Node.Status.Success;
        }
    }
}
