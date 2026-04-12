using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Pong.Framework.BehaviourTree
{
    public class RandomSelector : PrioritySelector
    {
        protected override List<Node> SortChildren()
        {
            return children.OrderBy(_ => Random.value).ToList();
        }

        public RandomSelector(string name) : base(name) { }
    }
}