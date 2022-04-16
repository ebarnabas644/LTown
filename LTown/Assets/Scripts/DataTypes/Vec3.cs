using System;

namespace DataTypes
{
    public class Vec3
    {
        private static int _incr;
        private readonly int _id;
        public float X { get; }
        public float Y { get; }
        public float Z { get; }
        
        public float Tolerance { get; set; }

        public Vec3(float x, float y, float z)
        {
            this.X = x;
            this.Y = y;
            this.Z = z;
            this.Tolerance = 0.001f;
            this._id = _incr++;
        }

        public override bool Equals(object obj)
        {
            return Equals(obj as Vec3);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(this.X, this.Y, this.Z);
        }

        private bool Equals(Vec3 other)
        {
            return Math.Abs(this.X - other.X) < Tolerance &&
                   Math.Abs(this.Y - other.Y) < Tolerance &&
                   Math.Abs(this.Z - other.Z) < Tolerance;
        }
    }
}