using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;
using DataTypes.Graph;
using DataTypes.Map;

namespace DataTypes
{
    public class Polygon<T> where T : ILocatable
    {
        public List<Node<T>> Points { get; }

        public Vec3 CenterPoint { get; private set; }

        public HashSet<Polygon<T>> SubPolygons;

        public int Lenght => Points.Count;

        public Polygon()
        {
            this.CenterPoint = new Vec3(0, 0, 0);
            this.Points = new List<Node<T>>();
            this.SubPolygons = new HashSet<Polygon<T>>();
        }

        public Polygon(Polygon<T> polygon)
        {
            this.Points = new List<Node<T>>(polygon.Points);
            CalculateCenterPoint();
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
            Edge<T> longest = null;
            foreach (var edge in GetPolygonEdges())
            {
                if (longest == null || edge.Distance() > longest.Distance())
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

            return new HashSet<Node<T>>(Points).SetEquals(item.Points);
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
    }
}