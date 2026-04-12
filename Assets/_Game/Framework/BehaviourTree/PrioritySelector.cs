using System.Collections.Generic;
using System.Linq;

namespace Pong.Framework.BehaviourTree
{
    public class PrioritySelector : Selector
    {
        List<Node> sortedChildren;
        List<Node> SortedChildren => sortedChildren ??= SortChildren();

        protected virtual List<Node> SortChildren() => children.OrderByDescending(child => child.priority).ToList();

        public PrioritySelector(string name = "PrioritySelector") : base(name) { }

        public override Status Process()
        {
            foreach (var child in SortedChildren)
            {
                switch (child.Process())
                {
                    case Status.Running:
                        return Status.Running;
                    case Status.Success:
                        return Status.Success;
                    default:
                        continue;
                }
            }
            return Status.Failure;
        }

        public override void Reset()
        {
            base.Reset();
            sortedChildren = null; 
        }

    }
}