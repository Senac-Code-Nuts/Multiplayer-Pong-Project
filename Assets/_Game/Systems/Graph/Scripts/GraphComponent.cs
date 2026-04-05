using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace Pong.Systems.Graph
{
    [ExecuteInEditMode]
    public class GraphComponent : MonoBehaviour
    {
        [SerializeField] private List<GraphNode> _nodes;

        [SerializeField] private InfluenceSystem _influenceSystem;

        [Header("Cor de peso")]
        [SerializeField] private Gradient _weightGradient;

        private float GetDynamicCost(GraphNode from, Connection conn)
        {
            var to = conn.GetOther(from);

            if (to == null) return conn.weight;

            float baseCost = conn.weight;

            float influence = _influenceSystem != null ? _influenceSystem.GetMultiplier(to.transform.position) : 1f;

            return baseCost * influence;
        }


        private void OnDrawGizmos()
        {
            if (_nodes == null) return;

            float minCost = float.MaxValue;
            float maxCost = float.MinValue;
            List<(GraphNode, GraphNode, float)> debugEdges = new();


            foreach (var node in _nodes)
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
            float t = Mathf.InverseLerp(min, max, weight);

            return _weightGradient.Evaluate(t);
        }
    }
}


