namespace Pong.Framework.BehaviourTree
{
    public class BehaviourTree : Node
    {
        public BehaviourTree(string name = "BehaviourTree") : base(name) { }

        public override Status Process()
        {
            if (currentChild >= children.Count)
            {
                Reset();
            }

            while (currentChild < children.Count)
            {
                var status = children[currentChild].Process();
                if (status != Status.Success) return status;
                currentChild++;
            }
            Reset();
            return Status.Success;
        }
    }
}