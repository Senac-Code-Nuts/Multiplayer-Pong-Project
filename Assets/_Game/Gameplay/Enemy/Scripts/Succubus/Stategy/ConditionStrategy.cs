using System;
using Pong.Framework.BehaviourTree;
using UnityEngine;

namespace Pong.Gameplay.Enemy.Succubus
{
    public class ConditionStrategy : INodeStrategy
    {
        private readonly Func<bool> _predicate;

        public ConditionStrategy(Func<bool> predicate)
        {
            _predicate = predicate;
        }
        public Node.Status Process()
        {
            return _predicate() ? Node.Status.Success : Node.Status.Failure;
        }
    }
}