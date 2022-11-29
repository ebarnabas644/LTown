using DataTypes;
using DataTypes.Graph;
using DataTypes.Map;
using NUnit.Framework;

namespace Tests
{
    public class EdgeTest
    {
        [Test]
        public void SameEdgeShouldBeEqual()
        {
            var point1 = new Node<Unit>(new Unit(1, 0, 2, 0));
            var point2 = new Node<Unit>(new Unit(3, 0, 4, 0));
            var edge1 = new Edge<Unit>(point1, point2);
            var edge2 = new Edge<Unit>(point2, point1);

            var result = edge1.Equals(edge2);

            Assert.That(result, Is.True);
        }
        
        [Test]
        public void DifferentEdgeShouldBeNotEqual()
        {
            var point1 = new Node<Unit>(new Unit(1, 0, 2, 0));
            var point2 = new Node<Unit>(new Unit(3, 0, 4, 0));
            var point3 = new Node<Unit>(new Unit(5, 0, 2, 0));
            var edge1 = new Edge<Unit>(point1, point2);
            var edge2 = new Edge<Unit>(point3, point1);

            var result = edge1.Equals(edge2);

            Assert.That(result, Is.False);
        }
    }
}