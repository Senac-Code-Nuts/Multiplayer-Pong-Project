using System;
using Pong.Framework.BehaviourTree;
using Pong.Gameplay.Enemy;
using UnityEngine;

namespace Pong.Gameplay
{
    public class WrathMoveGraphStrategy : EnemyPathStrategyBase
    {
        public WrathMoveGraphStrategy(EnemyActor enemy, EnemyPathFinder pathFinder, Func<float> speedProvider) : base(enemy, pathFinder, speedProvider) { }
        
        public override Node.Status Process()
        {
           if(!IsReady)
            {
                return Node.Status.Failure;
            }

            if(CurrentTargetNode == null)
            {
                var start = GetClosestNode(_enemy.transform.position);
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
