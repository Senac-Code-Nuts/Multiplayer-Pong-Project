namespace Pong.Edge
{
    using System;
    using System.Collections.Generic;
    using NUnit.Framework.Interfaces;
    using Unity.VisualScripting;
    using UnityEditor.Experimental.GraphView;
    using UnityEngine;
    using Pong.GraphNode;


        [Serializable]
        public class Edge
    {
        public GraphNode fromNode;
        public GraphNode toNode;

        public float weight;

        public float GetBaseCost()
        {
            return weight;
        }
    }
}

