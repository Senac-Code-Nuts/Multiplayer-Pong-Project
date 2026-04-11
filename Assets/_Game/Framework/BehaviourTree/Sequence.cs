namespace Pong.Framework.BehaviourTree
{
    public class Sequence : Node
    {
        public Sequence(string name = "Sequence", int priority = 0) : base(name, priority) { }

        public override Status Process()
        {
            if (currentChild < children.Count)
            {
                switch (children[currentChild].Process())
                {
                    case Status.Running:
                        return Status.Running;
                    case Status.Failure:
                        Reset();
                        return Status.Failure;
                    default: 
                        currentChild++;
                        if (currentChild == children.Count)
                        {
                            Reset(); 
                            return Status.Success;
                        }
                        return Status.Running;
                }
            }

            Reset();
            return Status.Success;
        }
    }
}