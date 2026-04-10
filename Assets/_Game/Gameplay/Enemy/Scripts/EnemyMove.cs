using System.Collections;
using System.Collections.Generic;
using Pong.Systems.Graph;
using UnityEditor.Graphs;
using UnityEngine;

namespace Pong.Gameplay.Enemy
{
    public class EnemyMove : MonoBehaviour
    {
        [SerializeField] private GraphComponent _graph;
        [SerializeField] private float _speed;
        [SerializeField] private bool _canMove = true;

        private GraphNode _currentNode;
        private List<GraphNode> _path = new List<GraphNode>();

        private void Start()
        {
            _currentNode = GetClosestNode();
            StartCoroutine(MoveRoutine());
        }

        private GraphNode GetClosestNode()
        {
            GraphNode closest = null;
            float minDistance = float.MaxValue;

            foreach(var node in _graph.Nodes)
            {
                float distance = Vector3.Distance(transform.position, node.transform.position);

                if(distance < minDistance)
                {
                    minDistance = distance;
                    closest = node;
                }
            }

            return closest;
        }

        private IEnumerator MoveRoutine()
        {
            
            while(true)
            {
                if(!_canMove)
                {
                    yield return null;
                    continue;
                }

                GraphNode target = GetRandomNode();
                _path = FindPath(_currentNode, target);

                                
                if(_path.Count == 0)
                {
                    
                    yield return null;
                    continue;
                }


                foreach(var node in _path)
                {
                    yield return MoveTo(node);
                    _currentNode = node;
                }

                yield return new WaitForSeconds(0.2f);
            }
        }

        private GraphNode GetRandomNode()
        {
            GraphNode targetNode = _graph.Nodes[Random.Range(0, _graph.Nodes.Count)];
            return targetNode;
        }

        private IEnumerator MoveTo(GraphNode node)
        {
            while(Vector3.Distance(transform.position, node.transform.position) > 0.05f)
            {
                transform.position = Vector3.MoveTowards(transform.position, node.transform.position, _speed * Time.deltaTime);

                 yield return null;
            }
        }

        private List<GraphNode> FindPath(GraphNode start, GraphNode target)
        {
            var distance = new Dictionary<GraphNode, float>();
            var previous = new Dictionary<GraphNode, GraphNode>();
            var unvisited = new List<GraphNode>(_graph.Nodes);

            foreach(var node in _graph.Nodes)
            {
                distance[node] = float.MaxValue;
            }

            distance[start] = 0;

            while (unvisited.Count > 0)
            {
                GraphNode current = GetLowest(unvisited, distance);
                if(current == target)
                {
                    break;
                }

                unvisited.Remove(current);

                foreach(var connection in current.connections)
                {
                    var neighbor = connection.GetOther(current);

                    if(neighbor == null)
                    {
                        continue;
                    } 

                    float cost = _graph.GetDynamicCost(current, connection);

                    float newDistance = distance[current] + cost;

                    if(newDistance < distance[neighbor])
                    {
                        distance[neighbor] = newDistance;
                        previous[neighbor] = current;
                    }
                }
            }
            return ReconstructPath(previous,start,target);
        }

        private GraphNode GetLowest(List<GraphNode> nodes, Dictionary<GraphNode, float> distance)
        {
            GraphNode nextBest = null;

            float min = float.MaxValue;

            foreach(var node in nodes)
            {
                if(distance[node] < min)
                {
                    min = distance[node];
                    nextBest = node;
                }
            }

            return nextBest;
        }

        private List<GraphNode> ReconstructPath(Dictionary<GraphNode,GraphNode> previous, GraphNode start, GraphNode target )
        {
            List<GraphNode> path = new List<GraphNode>();

            var current = target;

            while(current != start)
            {
                path.Add(current);
                if(!previous.ContainsKey(current))
                {
                    break;
                }

                current = previous[current];
            }

            path.Reverse();

            return path;
        }



    }
}
