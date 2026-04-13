namespace Pong.Framework.BehaviourTree
{
    public class UntilFail : Node
    {
        public UntilFail(string name = "UntilFail") : base(name) { }

        public override Status Process()
        {
            if (children[0].Process() == Status.Failure)
            {
                Reset();
                return Status.Failure;
            }

            return Status.Running;
        }
    }
}