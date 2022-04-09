using System.Collections.Generic;
using DataTypes;
using DataTypes.Graph;
using DataTypes.Graph.Assets.Scripts.Graph;
using RoadLayer.Generators;
using UnityEngine;

namespace ConvertLayer
{
    public class RoadSystemConverter
    {
        private Graph<GameObject> roadSystem;
        private RoadBuilder _roadBuilder;
        private Graph<Unit> originalGraph;
        public Graph<GameObject> convertedGraph { get; private set; }

        public RoadSystemConverter(RoadBuilder roadBuilder)
        {
            _roadBuilder = roadBuilder;
            this.originalGraph = new Graph<Unit>();
        }

        public Graph<GameObject> ConvertUnitGraphToGameObjectGraph(Graph<Unit> roadSystem)
        {
            this.originalGraph = roadSystem;
            this.convertedGraph = new Graph<GameObject>();
            this.ConvertVertexes();
            this.ConvertEdges();

            return convertedGraph;
        }

        private Node<GameObject> SearchGameObject(Node<Unit> node)
        {
            foreach (var vertex in this.convertedGraph.GetVertexes())
            {
                if (UnitEqualToGameObject(vertex.Key, node))
                {
                    return vertex.Key;
                }
            }

            return null;
        }

        private bool UnitEqualToGameObject(Node<GameObject> u, Node<Unit> v)
        {
            return u.GetContent.transform.position.x == v.GetContent.X && u.GetContent.transform.position.y == v.GetContent.Y && u.GetContent.transform.position.z == v.GetContent.Z;
        }

        private void ConvertVertexes()
        {
            foreach (var vertex in this.originalGraph.GetVertexes())
            {
                Node<GameObject> nodeToAdd = new Node<GameObject>(_roadBuilder.BuildIntersection(vertex.Key, 0));
                nodeToAdd.GetContent.name = "Intersection " + nodeToAdd.GetId;
                convertedGraph.AddVertex(nodeToAdd);
            }
        }

        private void ConvertEdges()
        {
            List<int> alreadyConverted = new List<int>();
            foreach (var vertex in this.originalGraph.GetVertexes())
            {
                foreach (var edge in vertex.Value)
                {
                    if (!alreadyConverted.Contains(edge.GetId))
                    {
                        alreadyConverted.Add(edge.GetId);
                        Node<Unit> start = edge.Start;
                        Node<Unit> end = edge.End;
                        Edge<GameObject> goEdge = new Edge<GameObject>(SearchGameObject(start), SearchGameObject(end));
                        goEdge.Content = _roadBuilder.BuildRoad(edge, 0);
                        goEdge.Content.name = "Edge " + goEdge.GetId;
                        this.convertedGraph.AddEdge(goEdge);
                    }
                }
            }
        }
    }
}