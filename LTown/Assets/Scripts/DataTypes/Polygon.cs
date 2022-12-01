using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;
using DataTypes.Graph;
using DataTypes.Map;

namespace DataTypes
{
    public enum PlotType
    {
        Housing = 0, Park, Market, Default
    }
    public class Polygon<T> where T : ILocatable
    {
        private static int counter = 0;
        public int Id;
        public List<Node<T>> Points { get; }

        public Vec3 CenterPoint { get; private set; }

        public PlotType PlotType = PlotType.Default;

        public int Length => Points.Count;

        public Polygon()
        {
            this.CenterPoint = new Vec3(0, 0, 0);
            this.Points = new List<Node<T>>();
            this.Id = counter;
            counter++;
        }

        public Polygon(Polygon<T> polygon)
        {
            this.Points = new List<Node<T>>(polygon.Points);
            CalculateCenterPoint();
            this.Id = counter;
            counter++;
        }

        public void CalculateCenterPoint()
        {
            var avg = new Vec3(0, 0, 0);
            foreach (var point in Points)
            {
                avg += point.GetContent.GetPosition();
            }

            avg /= Points.Count;
            CenterPoint = new Vec3(avg.X, avg.Y, avg.Z);
        }

        public Edge<T> GetLongestEdge()
        {
            var edges = GetPolygonEdges();
            Edge<T> longest = edges[0];
            foreach (var edge in edges)
            {
                if (edge.Distance() > longest.Distance())
                {
                    longest = edge;
                }
            }

            return longest;
        }


        public List<Edge<T>> GetPolygonEdges()
        {
            var edges = new List<Edge<T>>();
            for (int i = 0; i < Points.Count; i++)
            {
                edges.Add(new Edge<T>(Points[i], Points[(i+1) % Points.Count]));
            }

            return edges;
        }

        public bool AddPoint(Node<T> Point)
        {
            if (!Points.Contains(Point))
            {
                this.Points.Add(Point);
                CalculateCenterPoint();
                return true;
            }

            return false;
        }

        public void RemovePoint(Node<T> Point)
        {
            this.Points.Remove(Point);
        }

        public override bool Equals(object obj)
        {
            var item = obj as Polygon<T>;

            if (item == null)
            {
                return false;
            }

            return CenterPoint.Equals(item.CenterPoint);
        }

        protected bool Equals(Polygon<T> other)
        {
            return Equals(CenterPoint, other.CenterPoint);
        }

        public override int GetHashCode()
        {
            return CenterPoint.GetHashCode();
        }

        public override string ToString()
        {
            StringBuilder text = new StringBuilder();
            text.Append("Polygons of the plot: ");
            int counter = 1;
            foreach (var point in Points)
            {
                text.Append(counter + ": " + point + ": |" + point.GetContent.GetPosition().X + ", " + point.GetContent.GetPosition().Z + "|, ");
                counter++;
            }
            
            return text.ToString();
        }

        public float EstimateArea()
        {
            float sum = 0;
            foreach (var point in Points)
            {
                sum += CenterPoint.DistanceFrom(point.GetContent.GetPosition());
            }

            return sum / Points.Count;
        }
    }
}