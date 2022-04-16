using System.Collections.Generic;

namespace DataTypes
{
    public class Polygon
    {
        private List<Vec3> Points;

        public Polygon()
        {
            this.Points = new List<Vec3>();
        }

        public void AddPoint(Vec3 Point)
        {
            this.Points.Add(Point);
        }

        public void RemovePoint(Vec3 Point)
        {
            this.Points.Remove(Point);
        }
    }
}