using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace Pong.Systems.Graph
{
    [ExecuteInEditMode]
    public class GraphComponent : MonoBehaviour
    {
        [field : SerializeField] public List<GraphNode> Nodes {get; private set;}

        [SerializeField] private InfluenceSystem _influenceSystem;

        [Header("Weight Color")]
        [SerializeField] private Gradient _weightGradient;
        [SerializeField] private float _minWeight;
        [SerializeField] private float _maxWeight;

        public float GetDynamicCost(GraphNode from, Connection conn)
        {
           GraphNode toNode = conn.GetOther(from);

            if (toNode == null) return conn.weight;

            float baseCost = conn.weight;

            float influence = _influenceSystem != null ? _influenceSystem.GetMultiplier(toNode.transform.position) : 1f;

            return baseCost * influence;
        }


        private void OnDrawGizmos()
        {
           if (Nodes == null) return;

            float minCost = float.MaxValue;
            float maxCost = float.MinValue;
            List<(GraphNode, GraphNode, float)> debugEdges = new();


            foreach (var node in Nodes)
            {
                if (node == null) continue;
                Gizmos.color = Color.red;
                Gizmos.DrawSphere(node.transform.position, 0.025f);

                foreach (var conn in node.connections)
                {
                    if (conn == null) continue;

                    if (conn.nodeA != node) continue;

                    var other = conn.GetOther(node);

                    if (conn.nodeA == null || conn.nodeB == null) continue;

                    float cost = GetDynamicCost(node, conn);

                    debugEdges.Add((node, other, cost));

                    minCost = Mathf.Min(minCost, cost);
                    maxCost = Mathf.Max(maxCost, cost);
                }
            }

            foreach (var edge in debugEdges)
            {
                Vector3 start = edge.Item1.transform.position;
                Vector3 end = edge.Item2.transform.position;

                Gizmos.color = GetColor(edge.Item3, minCost, maxCost);
                Gizmos.DrawLine(start, end);

#if UNITY_EDITOR
                Vector3 mid = (start + end) * 0.5f;

                Handles.Label(
                    mid,
                    edge.Item3.ToString("F2")
                );
#endif
            }

        }

        private Color GetColor(float weight, float min, float max)
        {
            float normalizedWeight = Mathf.InverseLerp(min, max, weight);

            return _weightGradient.Evaluate(normalizedWeight);
        }

        [ContextMenu("Randomize Weights")]
        private void RandomizeWeights()
        {
            foreach(var node in Nodes)
            {
                foreach(var connection in node.connections)
                {
                    if(connection.nodeA != node) continue;

                    float randomWeight = Random.Range(minWeight, maxWeight);
                    connection.weight = randomWeight;
                }
            }
        }

        [ContextMenu("Reset Weights")]
        private void ResetWeights()
        {
            foreach(var node in Nodes)
            {
                foreach(var connection in node.connections)
                {
                    if(connection.nodeA != node) continue;

                    connection.weight = 1;
                }
            }
        }
    }
}


