using System.Collections.Generic;
using UnityEngine;
using Pong.Systems.Graph;

namespace Pong.Gameplay.Enemy
{
    /// <summary>
    /// Classe utilitária de pathfinding - NÃO herda MonoBehaviour.
    /// Responsável por calcular caminhos no grafo usando Dijkstra.
    /// Pode ser usada por qualquer estratégia de movimento.
    /// </summary>
    public class EnemyPathFinder
    {
        private GraphComponent _graph;

        public EnemyPathFinder(GraphComponent graph)
        {
            _graph = graph;
        }

        /// <summary>
        /// Encontra o node mais próximo de uma posição.
        /// </summary>
        public GraphNode GetClosestNode(Vector3 position)
        {
            if (_graph?.Nodes == null || _graph.Nodes.Count == 0)
                return null;

            GraphNode closest = null;
            float minDistance = float.MaxValue;

            foreach (var node in _graph.Nodes)
            {
                if (node == null) continue;

                float distance = Vector3.Distance(position, node.transform.position);
                if (distance < minDistance)
                {
                    minDistance = distance;
                    closest = node;
                }
            }

            return closest;
        }

        /// <summary>
        /// Calcula o caminho entre dois nodes usando Dijkstra.
        /// </summary>
        public List<GraphNode> FindPath(GraphNode start, GraphNode target, bool preferHighWeight = false)
        {
            if (start == null || target == null || _graph == null)
                return new List<GraphNode>();

            var distance = new Dictionary<GraphNode, float>();
            var previous = new Dictionary<GraphNode, GraphNode>();
            var unvisited = new List<GraphNode>(_graph.Nodes);

            // Inicializar distâncias
            foreach (var node in _graph.Nodes)
            {
                if (node != null)
                    distance[node] = float.MaxValue;
            }

            distance[start] = 0;

            // Dijkstra
            while (unvisited.Count > 0)
            {
                GraphNode current = GetLowest(unvisited, distance);
                if (current == null || current == target)
                    break;

                unvisited.Remove(current);

                foreach (var connection in current.connections)
                {
                    var neighbor = connection.GetOther(current);
                    if (neighbor == null) continue;

                    float cost = _graph.GetDynamicCost(current, connection);
                    float traversalCost = preferHighWeight ? 1f / Mathf.Max(cost, 0.001f) : cost;
                    float newDistance = distance[current] + traversalCost;

                    if (newDistance < distance[neighbor])
                    {
                        distance[neighbor] = newDistance;
                        previous[neighbor] = current;
                    }
                }
            }

            return ReconstructPath(previous, start, target);
        }

        private GraphNode GetLowest(List<GraphNode> nodes, Dictionary<GraphNode, float> distance)
        {
            GraphNode nextBest = null;
            float min = float.MaxValue;

            foreach (var node in nodes)
            {
                if (distance.ContainsKey(node) && distance[node] < min)
                {
                    min = distance[node];
                    nextBest = node;
                }
            }

            return nextBest;
        }

        public List<GraphNode> GetAllNodes()
        {
            return _graph?.Nodes;
        }

        /// <summary>
        /// Calcula a média de peso das conexões de um nó (influência dinâmica).
        /// </summary>
        private float GetNodeAverageWeight(GraphNode node)
        {
            if (node == null || node.connections == null || node.connections.Count == 0)
                return 1f;

            float totalCost = 0f;
            foreach (var conn in node.connections)
            {
                if (conn == null) continue;
                float cost = _graph.GetDynamicCost(node, conn);
                totalCost += cost;
            }

            return totalCost / node.connections.Count;
        }

        /// <summary>
        /// Retorna apenas nós com peso médio acima de 1.0 (áreas influenciadas positivamente).
        /// </summary>
        public List<GraphNode> GetHighWeightNodes()
        {
            var result = new List<GraphNode>();
            if (_graph?.Nodes == null) return result;

            foreach (var node in _graph.Nodes)
            {
                if (node != null && GetNodeAverageWeight(node) > 1f)
                {
                    result.Add(node);
                }
            }

            return result;
        }

        /// <summary>
        /// Encontra o node de alto peso mais próximo de uma posição.
        /// Usado para pathfinding restrito em parry mode.
        /// </summary>
        public GraphNode GetClosestHighWeightNode(Vector3 position)
        {
            var highWeightNodes = GetHighWeightNodes();
            if (highWeightNodes.Count == 0)
                return GetClosestNode(position); // Fallback: qualquer node

            GraphNode closest = null;
            float minDistance = float.MaxValue;

            foreach (var node in highWeightNodes)
            {
                if (node == null) continue;

                float distance = Vector3.Distance(position, node.transform.position);
                if (distance < minDistance)
                {
                    minDistance = distance;
                    closest = node;
                }
            }

            return closest ?? GetClosestNode(position);
        }

        /// <summary>
        /// Retorna apenas nós com peso médio abaixo de 1.0 (áreas leves, sem influência).
        /// </summary>
        public List<GraphNode> GetLowWeightNodes()
        {
            var result = new List<GraphNode>();
            if (_graph?.Nodes == null) return result;

            foreach (var node in _graph.Nodes)
            {
                if (node != null && GetNodeAverageWeight(node) < 1f)
                {
                    result.Add(node);
                }
            }

            return result;
        }

        /// <summary>
        /// Encontra o node de baixo peso mais próximo de uma posição.
        /// Usado quando Minotaur NÃO está em parry mode (evita player).
        /// </summary>
        public GraphNode GetClosestLowWeightNode(Vector3 position)
        {
            var lowWeightNodes = GetLowWeightNodes();
            if (lowWeightNodes.Count == 0)
                return GetClosestNode(position); // Fallback: qualquer node

            GraphNode closest = null;
            float minDistance = float.MaxValue;

            foreach (var node in lowWeightNodes)
            {
                if (node == null) continue;

                float distance = Vector3.Distance(position, node.transform.position);
                if (distance < minDistance)
                {
                    minDistance = distance;
                    closest = node;
                }
            }

            return closest ?? GetClosestNode(position);
        }

        private List<GraphNode> ReconstructPath(Dictionary<GraphNode, GraphNode> previous, GraphNode start, GraphNode target)
        {
            List<GraphNode> path = new List<GraphNode>();
            var current = target;

            while (current != start)
            {
                path.Add(current);
                if (!previous.ContainsKey(current))
                    break;
                current = previous[current];
            }

            path.Add(start);
            path.Reverse();
            return path;
        }
    }
}
