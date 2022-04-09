using System;

namespace DataTypes
{
    public class Unit
    {
        public float X { get; set; }
        public float Y { get; set; }
        public float Z { get; set; }
        public float Angle { get; set; }

        public Unit(float x, float y, float z, float angle)
        {
            this.X = x;
            this.Y = y;
            this.Z = z;
            this.Angle = angle;
        }

        public override bool Equals(object obj)
        {
            if (obj == null || !this.GetType().Equals(obj.GetType()))
            {
                return false;
            }
            else
            {
                Unit unit = (Unit)obj;
                if (unit.Angle == this.Angle && unit.X == this.X && unit.Y == this.Y && unit.Z == this.Z)
                {
                    return true;
                }

                return false;
            }
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(this.X, this.Y, this.Z, this.Angle);
        }
    }
}