using System.Collections.Generic;
using DataTypes;
using DataTypes.Graph;
using Layers.PlotLayer;
using NUnit.Framework;
using UnityEditor.Experimental.GraphView;

public class SubPlotGeneratorTest
{
    private SubPlotGenerator _generator;
    
    [SetUp]
    public void Setup()
    {
        _generator = new SubPlotGenerator(new HashSet<Polygon<Unit>>());
    }
    
    [Test]
    public void IntersectionOfLinesTest()
    {
        var A = new Node<Unit>(new Unit(5, 0, 9, 0));
        var B = new Node<Unit>(new Unit(4, 0, 2, 0));
        var C = new Node<Unit>(new Unit(4, 0, 5.5f, 0));
        var D = new Node<Unit>(new Unit(10, 0, 5.5f, 0));

        //var result = _generator.IntersectionOfTwoLines(A, B, C, D);
        
        //Assert.That(result, Is.EqualTo(new Node<Unit>(new Unit(4.5f, 0, 5.5f, 0))));

    }

    [Test]
    public void BoundingBoxCalculatorTest()
    {
        var testPolygon = new Polygon<Unit>();
        testPolygon.AddPoint(new Node<Unit>(new Unit(5, 0, 9, 0)));
        testPolygon.AddPoint(new Node<Unit>(new Unit(10, 0, 7, 0)));
        testPolygon.AddPoint(new Node<Unit>(new Unit(10, 0, 3, 0)));
        testPolygon.AddPoint(new Node<Unit>(new Unit(4, 0, 2, 0)));
        var expected = new Polygon<Unit>();
        expected.AddPoint(new Node<Unit>(new Unit(4, 0, 9, 0)));
        expected.AddPoint(new Node<Unit>(new Unit(10, 0, 9, 0)));
        expected.AddPoint(new Node<Unit>(new Unit(10, 0, 2, 0)));
        expected.AddPoint(new Node<Unit>(new Unit(4, 0, 2, 0)));

        //var result = _generator.CalculateBoundingBox(testPolygon);
        
        //Assert.That(result, Is.EqualTo(expected));
    }
}
