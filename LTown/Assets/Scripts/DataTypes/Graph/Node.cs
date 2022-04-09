namespace DataTypes.Graph
{
    public class Node<T>
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
            return _id;
        }

        public override bool Equals(object obj)
        {
            if ((obj == null) || !this.GetType().Equals(obj.GetType()))
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