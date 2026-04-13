using System;

namespace Pong.Framework.BehaviourTree
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
