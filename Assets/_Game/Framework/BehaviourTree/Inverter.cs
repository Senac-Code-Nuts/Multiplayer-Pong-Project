namespace Pong.Framework.BehaviourTree
{
    public class Inverter : Node
    {
        public Inverter(string name = "Inverter") : base(name) { }

        public override Status Process()
        {
            switch(children[0].Process())
            {
                case Status.Running:
                    return Status.Running;
                case Status.Failure:
                    return Status.Success;  
                default:
                    return Status.Failure;
            }
        }
    }
}