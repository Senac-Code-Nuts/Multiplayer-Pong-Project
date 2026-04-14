using System;
using Pong.Framework.BehaviourTree;
using Pong.Gameplay.Actors;
using Pong.Gameplay.Boss;
using Pong.Gameplay.Enemy;
using UnityEngine;

namespace Pong.Gameplay.Boss
{
    public class WrathMoveGraphStrategy : EnemyPathStrategyBase
    {
        private WrathBoss _boss;
        public WrathMoveGraphStrategy(Actor actor, EnemyPathFinder pathFinder, Func<float> speedProvider) : base(actor, pathFinder, speedProvider) 
        {
            _boss = (WrathBoss)actor;
        }
        
        public override Node.Status Process()
        {
            if(!IsReady || !_boss._canMove || _boss.IsAttacking)
            {
                return Node.Status.Failure; 
            }


            if(CurrentTargetNode == null)
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
