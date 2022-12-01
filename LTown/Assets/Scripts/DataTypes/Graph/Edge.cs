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

        public float[] LineEquation;

        public Edge(Node<T> start, Node<T> end)
        {
            this.Start = start;
            this.End = end;
            this.id = counter++;
            CalculateLineEquation();
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

        public float Distance()
        {
            var startPos = Start.GetContent.GetPosition();
            var endPos = End.GetContent.GetPosition();
            var distX = endPos.X - startPos.X;
            var distZ = endPos.Z - startPos.Z;
            return distX * distX + distZ * distZ;
        }

        public Vec3 MiddlePoint()
        {
            var startPos = Start.GetContent.GetPosition();
            var endPos = End.GetContent.GetPosition();
            return (startPos + endPos) / 2;
        }

        private void CalculateLineEquation()
        {
            var a = Start.GetContent.GetPosition();
            var b = End.GetContent.GetPosition();

            Vec3 vector = b - a;

            LineEquation = new float[] { vector.X, vector.Z, vector.X * a.X + vector.Z * a.Z };
        }

        public Tuple<Vec3, Vec3> CalculateNormal()
        {
            var startPos = Start.GetContent.GetPosition();
            var endPos = End.GetContent.GetPosition();
            var sub = endPos - startPos;
            var normal1 = new Vec3(-sub.Z, sub.Y, sub.X);
            var normal2 = new Vec3(sub.Z, sub.Y, -sub.X);

            return new Tuple<Vec3, Vec3>(normal1, normal2);
        }
        
        public override string ToString()
        {
            return "|EdgeId: " + this.GetId + "|Start: " + this.Start + "End: " + this.End;
        }
    }

}