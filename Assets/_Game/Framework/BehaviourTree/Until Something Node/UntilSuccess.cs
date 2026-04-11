namespace Pong.Framework.BehaviourTree
{
    public class UntilSuccess : Node
    {
        public UntilSuccess(string name = "UntilSuccess") : base(name) { }

        public override Status Process()
        {
            if (children[0].Process() == Status.Success)
            {
                Reset();
                return Status.Success;
            }

            return Status.Running;
        }
    }
}
