using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Pong.Systems.Graph
{
    public class GraphNode : MonoBehaviour
    {
        public List<Connection> connections = new List<Connection>();

        [Header("Auto Connect")]
        [SerializeField] private GraphNode _autoTarget;
        [SerializeField] private float _autoWeight = 1f;
        public List<GraphNode> GetNeighbours()
        {
            List<GraphNode> neighbours = new();

            foreach (var conn in connections)
            {
                var other = conn.GetOther(this);

                if (other != null)
                {
                    neighbours.Add(other);
                }

            }

            return neighbours;
        }

        public void ConnectTo(GraphNode other, float weight = 1f)
        {
            if (other == null || other == this) return;

            foreach (var conn in connections)
            {
                if (conn.GetOther(this) == other)
                {
                    conn.weight = weight;
                    return;
                }
            }

            Connection newConn = new Connection
            {
                nodeA = this,
                nodeB = other,
                weight = weight
            };

            connections.Add(newConn);
            other.connections.Add(newConn);
        }

        [ContextMenu("Connect To Target")]
        private void ConnectToTarget()
        {
            if (_autoTarget != null)
            {
                ConnectTo(_autoTarget, _autoWeight);
            }
        }

        [CustomEditor(typeof(GraphNode))]
        private class GraphNodeEditor : Editor
        {
            public override void OnInspectorGUI()
            {
                base.OnInspectorGUI();

                GUILayout.Space(10);

                GraphNode node = (GraphNode)target;

                if (GUILayout.Button("Connect To Target"))
                {
                    node.ConnectToTarget();
                }
            }
        }
    }

}
