using Pong.Framework.BehaviourTree;
using UnityEngine;

namespace Pong.Gameplay
{
    public class EnvyWaitStrategy : INodeStrategy
    {
        public Node.Status Process()
        {
            return Node.Status.Running;
        }
    }
}
