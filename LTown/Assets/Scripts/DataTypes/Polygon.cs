using System.Collections.Generic;
using System.Text;
using DataTypes.Graph;
using DataTypes.Map;

namespace DataTypes
{
    public class Polygon<T> where T : ILocatable
    {
        public List<Node<T>> Points { get; }

        public int Lenght => Points.Count;

        public Polygon()
        {
            this.Points = new List<Node<T>>();
        }

        public Polygon(Polygon<T> polygon)
        {
            this.Points = new List<Node<T>>(polygon.Points);
        }

        public bool AddPoint(Node<T> Point)
        {
            if (!Points.Contains(Point))
            {
                this.Points.Add(Point);
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
                text.Append(counter + ": " + point + ", ");
                counter++;
            }
            
            return text.ToString();
        }
    }
}