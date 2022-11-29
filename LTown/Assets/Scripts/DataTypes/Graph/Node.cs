using System;
using DataTypes.Map;

namespace DataTypes.Graph
{
    public class Node<T> where T : ILocatable
    {
        private static int counter = 0;
        private int _id;
        private T _node;

        public Node(T node)
        {
            this._id = counter++;
            this._node = node;
        }

        public T GetContent
        {
            get
            {
                return this._node;
            }
            set
            {
                this._node = value;
            }
        }

        public int GetId
        {
            get
            {
                return this._id;
            }
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(this._node.GetPosition().X, this._node.GetPosition().Y, this._node.GetPosition().Z);
        }

        public override bool Equals(object obj)
        {
            if ((obj == null))
            {
                return false;
            }
            else
            {
                Node<T> p = (Node<T>)obj;
                return this._node.Equals(p.GetContent);
            }
        }

        public override string ToString()
        {
            return "|NodeId: " + this.GetId + "|";
        }
    }

}