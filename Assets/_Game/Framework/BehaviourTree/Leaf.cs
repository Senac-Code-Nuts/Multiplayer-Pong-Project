using Pong.Framework.Strategy;

namespace Pong.Framework.BehaviourTree
{
    public class Leaf : Node
    {
        private readonly INodeStrategy strategy;

        public Leaf(string name, INodeStrategy strategy) : base(name)
        {
            this.strategy = strategy;
        }

        public override Status Process() => strategy.Process();
        public override void Reset() => strategy.Reset();
    }
}