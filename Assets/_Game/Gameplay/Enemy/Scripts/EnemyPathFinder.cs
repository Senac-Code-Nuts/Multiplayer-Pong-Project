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
        public List<GraphNode> FindPath(GraphNode start, GraphNode target)
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
                    float newDistance = distance[current] + cost;

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

    /// <summary>
    /// Retorna todos os nodes do grafo.
    /// </summary>
    public List<GraphNode> GetAllNodes()
    {
        return _graph?.Nodes;
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

            path.Add(start); // Adiciona o nó de início
            path.Reverse();
            return path;
        }
    }
}
