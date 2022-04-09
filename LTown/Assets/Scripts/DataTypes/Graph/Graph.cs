namespace DataTypes.Graph
{
    using System.Collections.Generic;
    using System.Linq;

    namespace Assets.Scripts.Graph
{
    public class Graph<T>
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
            if (!EdgeExists(edge) && VertexExists(edge.Start) && VertexExists(edge.End))
            {
                this.vertexes[edge.Start].Add(edge);
                this.vertexes[edge.End].Add(edge);
                return true;
            }

            return false;
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

        public Dictionary<Node<T>, HashSet<Edge<T>>> GetVertexes()
        {
            Dictionary<Node<T>, HashSet<Edge<T>>> vertexesCopy = new Dictionary<Node<T>, HashSet<Edge<T>>>(vertexes);

            return vertexesCopy;
        }

        public bool VertexExists(Node<T> vertex)
        {
            if (vertex != null && vertexes.ContainsKey(vertex))
            {
                return true;
            }

            return false;
        }

        public bool EdgeExists(Edge<T> edgeForTest)
        {
            foreach (var vertex in vertexes)
            {
                foreach (var edge in vertex.Value)
                {
                    if ((edge.Start == edgeForTest.Start && edge.End == edgeForTest.End) || (edge.Start == edgeForTest.End && edge.End == edgeForTest.Start))
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