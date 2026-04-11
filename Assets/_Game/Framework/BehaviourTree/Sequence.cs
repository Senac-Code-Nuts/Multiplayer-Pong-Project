namespace Pong.Framework.BehaviourTree
{
    public class Sequence : Node
    {
        public Sequence(string name = "Sequence") : base(name) { }

        public override Status Process()
        {
            if (currentChild < children.Count)
            {
                switch(children[currentChild].Process())
                {
                    case Status.Running:
                        return Status.Running;
                    case Status.Failure:
                        Reset();
                        return Status.Failure;
                    default:
                        currentChild++;
                        return currentChild == children.Count ? Status.Success : Status.Running;
                }
            }
            Reset();
            return Status.Success;
        }

        public override void Reset()
        {
            base.Reset();
        }
    }
}