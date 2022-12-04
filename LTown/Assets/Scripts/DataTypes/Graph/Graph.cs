using DataTypes.Map;

namespace DataTypes.Graph
{
    using System.Collections.Generic;
    using System.Linq;

    namespace Assets.Scripts.Graph
{
    public class Graph<T> where T : ILocatable
    {
        private Dictionary<Node<T>, HashSet<Edge<T>>> vertexes;

        public Graph()
        {
            this.vertexes = new Dictionary<Node<T>, HashSet<Edge<T>>>();
        }

        public bool AddVertex(Node<T> vertex)
        {
            if (vertexes.TryAdd(vertex, new HashSet<Edge<T>>()))
            {
                return true;
            }

            return false;
        }

        public bool AddEdge(Edge<T> edge)
        {
            if (!EdgeExists(edge) && VertexExists(edge.Start) && VertexExists(edge.End) && !edge.Start.Equals(edge.End))
            {
                this.vertexes[edge.Start].Add(edge);
                this.vertexes[edge.End].Add(edge);
                return true;
            }

            return false;
        }

        public void AddGraph(Graph<T> toAdd)
        {
            toAdd.GetVertexes().ToList().ForEach(x => this.vertexes.Add(x.Key, x.Value));
        }

        public bool RemoveEdge(Node<T> v, Node<T> u, Edge<T> edge)
        {
            if (VertexExists(v) && VertexExists(u))
            {
                this.vertexes[v].Remove(edge);
                this.vertexes[u].Remove(edge);
                return true;
            }

            return false;
        }

        public bool RemoveVertex(Node<T> v)
        {
            if (VertexExists(v))
            {
                foreach (var u in vertexes)
                {
                    foreach (var edge in vertexes[v])
                    {
                        this.RemoveEdge(v, u.Key, edge);
                    }
                }

                vertexes.Remove(v);
                return true;
            }

            return false;
        }

        public List<Edge<T>> GetVertexEdges(Node<T> vertex)
        {
            if (VertexExists(vertex))
            {
                return vertexes[vertex].ToList();
            }

            return Enumerable.Empty<Edge<T>>().ToList();
        }

        public List<Edge<T>> GetEdges()
        {
            HashSet<Edge<T>> edges = new HashSet<Edge<T>>();
            foreach (var vertex in vertexes)
            {
                foreach (var edge in vertex.Value)
                {
                    edges.Add(edge);
                }
            }

            return edges.ToList();
        }

        public Dictionary<Node<T>, HashSet<Edge<T>>> GetVertexes()
        {
            return vertexes;
        }

        private bool VertexExists(Node<T> vertex)
        {
            if (vertex != null && vertexes.ContainsKey(vertex))
            {
                return true;
            }

            return false;
        }

        private bool EdgeExists(Edge<T> edgeForTest)
        {
            foreach (var vertex in vertexes)
            {
                foreach (var edge in vertex.Value)
                {
                    if ((edge.Start.Equals(edgeForTest.Start) && edge.End.Equals(edgeForTest.End)) || (edge.Start.Equals(edgeForTest.End) && edge.End.Equals(edgeForTest.Start)))
                    {
                        return true;
                    }
                }
            }

            return false;
        }
    }
}

}