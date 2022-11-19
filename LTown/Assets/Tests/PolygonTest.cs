using System.Collections;
using System.Collections.Generic;
using DataTypes;
using DataTypes.Graph;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

public class PolygonTest
{
    
    
    // A Test behaves as an ordinary method
    [Test]
    public void SamePolygonEqualityShouldBeTrue()
    {
        var polygon1 = new Polygon<Unit>();
        polygon1.AddPoint(new Node<Unit>(new Unit(1, 2, 3, 0)));
        polygon1.AddPoint(new Node<Unit>(new Unit(2, 3, 4, 0)));
        polygon1.AddPoint(new Node<Unit>(new Unit(3, 4, 5, 0)));
        var polygon2 = new Polygon<Unit>();
        polygon2.AddPoint(new Node<Unit>(new Unit(2, 3, 4, 0)));
        polygon2.AddPoint(new Node<Unit>(new Unit(3, 4, 5, 0)));
        polygon2.AddPoint(new Node<Unit>(new Unit(1, 2, 3, 0)));

        var result = polygon1.Equals(polygon2);
        
        Assert.That(result, Is.True);
    }
    
    [Test]
    public void DifferentPolygonEqualityShouldBeFalse()
    {
        var polygon1 = new Polygon<Unit>();
        polygon1.AddPoint(new Node<Unit>(new Unit(1, 2, 3, 0)));
        polygon1.AddPoint(new Node<Unit>(new Unit(2, 3, 4, 0)));
        polygon1.AddPoint(new Node<Unit>(new Unit(3, 4, 5, 0)));
        var polygon2 = new Polygon<Unit>();
        polygon2.AddPoint(new Node<Unit>(new Unit(2, 3, 4, 0)));
        polygon2.AddPoint(new Node<Unit>(new Unit(3, 4, 5, 0)));
        polygon2.AddPoint(new Node<Unit>(new Unit(1, 2, 3, 0)));
        polygon2.AddPoint(new Node<Unit>(new Unit(6, 2, 3, 0)));

        var result = polygon1.Equals(polygon2);
        
        Assert.That(result, Is.False);
    }

    [Test]
    public void AddWhenAlreadyContainsShouldBeFalse()
    {
        var polygon = new Polygon<Unit>();
        polygon.AddPoint(new Node<Unit>(new Unit(1, 2, 3, 0)));
        polygon.AddPoint(new Node<Unit>(new Unit(2, 3, 4, 0)));
        polygon.AddPoint(new Node<Unit>(new Unit(3, 4, 5, 0)));
        
        var result = polygon.AddPoint(new Node<Unit>(new Unit(3, 4, 5, 0)));
        
        Assert.That(result, Is.False);
    }
}
