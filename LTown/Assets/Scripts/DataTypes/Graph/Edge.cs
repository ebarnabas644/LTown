using DataTypes.Map;

namespace DataTypes.Graph
{
    public class Edge<T> where T : ILocatable
    {
        private static int counter = 0;
        private int id;
        private T edge;
        public Node<T> Start { get; private set; }
        public Node<T> End { get; private set; }

        public Edge(Node<T> start, Node<T> end)
        {
            this.Start = start;
            this.End = end;
            this.id = counter++;
        }

        public int GetId
        {
            get
            {
                return this.id;
            }
        }

        public override int GetHashCode()
        {
            return id;
        }

        public T Content
        {
            get
            {
                return edge;
            }

            set
            {
                this.edge = value;
            }
        }

        public override bool Equals(object obj)
        {
            if ((obj == null) || !this.GetType().Equals(obj.GetType()))
            {
                return false;
            }
            else
            {
                Edge<T> p = (Edge<T>)obj;
                return this.Start.GetContent.Equals(p.Start.GetContent) && this.End.GetContent.Equals(p.End.GetContent);
            }
        }

        public override string ToString()
        {
            return "|EdgeId: " + this.GetId + "|";
        }
    }

}