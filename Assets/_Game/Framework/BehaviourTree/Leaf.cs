using Pong.Framework.Strategy;

namespace Pong.Framework.BehaviourTree
{
    public class Leaf : Node
    {
        private readonly INodeStrategy strategy;

        public Leaf(string name, INodeStrategy strategy, int priority = 0) : base(name, priority)
        {
            this.strategy = strategy;
        }

        public override Status Process() => strategy.Process();
        public override void Reset() => strategy.Reset();
    }
}