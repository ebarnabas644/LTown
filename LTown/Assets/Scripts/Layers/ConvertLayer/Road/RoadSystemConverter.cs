using System.Collections.Generic;
using DataTypes;
using DataTypes.Graph;
using DataTypes.Graph.Assets.Scripts.Graph;
using DataTypes.Map;
using RoadLayer.Generators;
using UnityEngine;

namespace ConvertLayer
{
    public class RoadSystemConverter
    {
        private Map<CityObject> roadSystem;
        private RoadBuilder _roadBuilder;
        private Map<Unit> originalGraph;
        public Map<CityObject> convertedGraph { get; private set; }

        public RoadSystemConverter(RoadBuilder roadBuilder)
        {
            _roadBuilder = roadBuilder;
            this.originalGraph = new Map<Unit>();
        }

        public Map<CityObject> ConvertUnitGraphToGameObjectGraph(Map<Unit> roadSystem)
        {
            this.originalGraph = roadSystem;
            this.convertedGraph = new Map<CityObject>();
            this.ConvertVertexes();
            this.ConvertEdges();

            return convertedGraph;
        }

        private Node<CityObject> SearchGameObject(Node<Unit> node)
        {
            foreach (var vertex in this.convertedGraph.GetChunk(node.GetContent.GetPosition()).GetVertexes())
            {
                if (UnitEqualToGameObject(vertex.Key, node))
                {
                    return vertex.Key;
                }
            }

            return null;
        }

        private bool UnitEqualToGameObject(Node<CityObject> u, Node<Unit> v)
        {
            return u.GetContent.GetGameObject().transform.position.x == v.GetContent.X && u.GetContent.GetGameObject().transform.position.y == v.GetContent.Y && u.GetContent.GetGameObject().transform.position.z == v.GetContent.Z;
        }

        private void ConvertVertexes()
        {
            foreach (var vertex in this.originalGraph.GetVertexes())
            {
                Node<CityObject> nodeToAdd = new Node<CityObject>(_roadBuilder.BuildIntersection(vertex.Key, 0));
                nodeToAdd.GetContent.GetGameObject().name = "Intersection " + nodeToAdd.GetId;
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
                        Edge<CityObject> goEdge = new Edge<CityObject>(SearchGameObject(start), SearchGameObject(end));
                        goEdge.Content = _roadBuilder.BuildRoad(edge, 0);
                        goEdge.Content.GetGameObject().name = "Edge " + goEdge.GetId;
                        this.convertedGraph.AddEdge(goEdge);
                    }
                }
            }
        }
    }
}