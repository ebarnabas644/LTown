using System;
using DataTypes.Map;

namespace DataTypes
{
    public class Unit : ILocatable
    {
        public Vec3 Position { get; set; }
        public float Angle { get; set; }

        public Unit(float x, float y, float z, float angle)
        {
            this.Position = new Vec3(x, y, z);
            this.Angle = angle;
        }

        public float X
        {
            get => Position.X;
        }

        public float Y
        {
            get => Position.Y;
        }

        public float Z
        {
            get => Position.Z;
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
                if (unit.Angle == this.Angle && unit.Position.Equals(this.Position))
                {
                    return true;
                }

                return false;
            }
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(this.Position.X, this.Position.Y, this.Position.Z, this.Angle);
        }

        public Vec3 GetPosition()
        {
            return this.Position;
        }
    }
}