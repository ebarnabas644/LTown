using System;
using DataTypes.Map;

namespace DataTypes.Graph
{
    public class Edge<T> where T : ILocatable
    {
        private static int counter = 0;
        private int id;
        private T _edge;
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
            return HashCode.Combine(Start, End);
        }

        public T Content
        {
            get
            {
                return _edge;
            }

            set
            {
                this._edge = value;
            }
        }

        public override bool Equals(object obj)
        {
            if ((obj == null))
            {
                return false;
            }
            else
            {
                Edge<T> p = (Edge<T>)obj;
                return this.Start.GetContent.Equals(p.Start.GetContent) && this.End.GetContent.Equals(p.End.GetContent) ||
                       this.Start.GetContent.Equals(p.End.GetContent) && this.End.GetContent.Equals(p.Start.GetContent);
            }
        }

        public override string ToString()
        {
            return "|EdgeId: " + this.GetId + "|Start: " + this.Start + "End: " + this.End;
        }
    }

}