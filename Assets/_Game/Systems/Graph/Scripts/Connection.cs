using System;

namespace Pong.Systems.Graph
{
    [Serializable]
    public class Connection
    {
        public GraphNode nodeA;
        public GraphNode nodeB;
        public float weight;

        public GraphNode GetOther(GraphNode current)
        {
            if (current == nodeA) return nodeB;
            if (current == nodeB) return nodeA;

            return null;
        }
    }
}
