using System;
using Pong.Framework.BehaviourTree;
using Pong.Gameplay.Actors;
using Pong.Gameplay.Enemy;
using UnityEngine;

namespace Pong.Gameplay.Boss
{
    public class EnvyMoveGraphStrategy : EnemyPathStrategyBase
    {
        private EnvyBoss _boss;
        public EnvyMoveGraphStrategy(Actor actor, EnemyPathFinder pathFinder, Func<float> speedProvider) : base(actor, pathFinder, speedProvider) 
        {
            _boss = (EnvyBoss)actor;
        }
        public override Node.Status Process()
        {

            if(!IsReady)
            {
                return Node.Status.Failure; 
            }

            if(!_boss.CanMove || _boss.IsPreparingAttack)
            {
                ResetPath();
                return Node.Status.Failure;
            }


            if(CurrentTargetNode == null|| HasReachedTarget(CurrentTargetNode))
            {
                var start = GetClosestNode(_actor.transform.position);
                var target = GetRandomNode();

                if(!TryBuildPath(start, target))
                {
                    return Node.Status.Failure;
                }
            }

            MoveTowardNode(CurrentTargetNode);

            if(HasReachedTarget(CurrentTargetNode))
            {
                AdvancePath();
            }

            return Node.Status.Running;
        }

    }
}
