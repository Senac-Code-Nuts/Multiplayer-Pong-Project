namespace Pong.Connection
{
    using System;
    using System.Collections.Generic;
    using NUnit.Framework.Interfaces;
    using Unity.VisualScripting;
    using UnityEditor.Experimental.GraphView;
    using UnityEngine;
    using GraphNode;
    

    [Serializable]
    public class Connection
    {
        public GraphNode nodeA;
        public GraphNode nodeB;
        public float weight;

        public GraphNode GetOther(GraphNode current)
        {
            if(current == nodeA) return nodeB;
            if(current == nodeB) return nodeA;

            return null;
        }
    } 
}
