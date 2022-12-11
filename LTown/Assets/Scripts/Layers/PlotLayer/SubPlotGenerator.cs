using System;
using System.Collections.Generic;
using System.Linq;
using DataTypes;
using DataTypes.Graph;
using MathNet.Numerics.LinearAlgebra;
using UnityEngine;
using Random = System.Random;

namespace Layers.PlotLayer
{
    public class SubPlotGenerator
    {
        private HashSet<Polygon<Unit>> plots;
        private HashSet<Polygon<Unit>> subPlots;
        private float targetArea;
        private Random rnd = new Random();

        public SubPlotGenerator(HashSet<Polygon<Unit>> plots, float targetArea)
        {
            this.targetArea = targetArea;
            this.subPlots = new HashSet<Polygon<Unit>>();
            this.plots = plots;
        }

        public HashSet<Polygon<Unit>> Generate()
        {
            this.subPlots = new HashSet<Polygon<Unit>>();
            foreach (var plot in plots)
            {
                SplitPolygonToArea(plot, targetArea);
            }

            
            var e = new Node<Unit>(new Unit(8, 0, 12, 0));
            var d = new Node<Unit>(new Unit(18, 0, 12, 0));
            var b = new Node<Unit>(new Unit(10, 0, 2, 0));
            var c = new Node<Unit>(new Unit(18, 0, 4, 0));
            var result = IsRayIntersecting(e, d, b, c);
            return subPlots;
        }

        public void SplitPolygonToArea(Polygon<Unit> polygon, float area)
        {
            if(polygon == null) return;
            var subPolygon = SplitPolygon(polygon);
            if (subPolygon.Item1.EstimateArea() > area)
            {
                SplitPolygonToArea(subPolygon.Item1, area);
            }
            else
            {
                subPlots.Add(subPolygon.Item1);
            }

            if (subPolygon.Item2.EstimateArea() > area)
            {
                SplitPolygonToArea(subPolygon.Item2, area);
            }
            else
            {
                subPlots.Add(subPolygon.Item2);
            }

        }

        public Tuple<Polygon<Unit>, Polygon<Unit>> SplitPolygon(Polygon<Unit> polygon)
        {
            var longestEdge = polygon.GetLongestEdge();
            var middlePoint = longestEdge.MiddlePoint();
            var furthestEdge = longestEdge;
            var polygonEdges = polygon.GetPolygonEdges();

            foreach (var edge in polygonEdges)
            {
                if (!edge.Equals(longestEdge) && IsRayIntersecting(longestEdge.Start, longestEdge.End, edge.Start, edge.End))
                {
                    furthestEdge = edge;
                } 
            }

            var randomSeed = rnd.NextDouble() * 0.3 + 0.3;
            var furthestMiddlePoint = furthestEdge.Start.GetContent.Position * (float)randomSeed + furthestEdge.End.GetContent.Position * (1 - (float)randomSeed);

            var dividerStartNode = new Node<Unit>(new Unit(middlePoint.X, middlePoint.Y, middlePoint.Z, 0));
            var dividerEndNode =
                new Node<Unit>(new Unit(furthestMiddlePoint.X, furthestMiddlePoint.Y, furthestMiddlePoint.Z, 0));

            var plot1 = new Polygon<Unit>();
            plot1.AddPoint(dividerStartNode);
            plot1.AddPoint(longestEdge.End);
            var plot2 = new Polygon<Unit>();
            plot2.AddPoint(dividerStartNode);
            plot2.AddPoint(longestEdge.Start);
            var currentEdge = longestEdge;
            var currentIdx = polygonEdges.IndexOf(longestEdge);
            while (!currentEdge.Equals(furthestEdge))
            {
                plot1.AddPoint(currentEdge.End);
                currentIdx++;
                currentEdge = polygonEdges[currentIdx % polygonEdges.Count];
            }

            plot1.AddPoint(dividerEndNode);

            currentEdge = longestEdge;
            currentIdx = polygonEdges.IndexOf(longestEdge);
            while (!currentEdge.Equals(furthestEdge))
            {
                plot2.AddPoint(currentEdge.Start);
                currentIdx--;
                currentEdge = polygonEdges[mod(currentIdx, polygonEdges.Count)];
            }

            plot2.AddPoint(dividerEndNode);

            return new Tuple<Polygon<Unit>, Polygon<Unit>>(plot1, plot2);
        }

        private bool IsRayIntersecting(Node<Unit> A, Node<Unit> B, Node<Unit> C, Node<Unit> D)
        {
            var a = A.GetContent.Position;
            var b = B.GetContent.Position;
            var c = C.GetContent.Position;
            var d = D.GetContent.Position;

            Vec3 middlePoint = (a + b) / 2;
            Vec3 vector1 = b - a;
            Vec3 vector2 = d - c;
            Vec3 normal2 = new Vec3(vector2.Z, vector2.Y, -vector2.X);

            var matrix = Matrix<float>.Build.DenseOfArray(new float[,]
            {
                { normal2.X, normal2.Z },
                { vector1.X, vector1.Z }
            });

            var right = Vector<float>.Build.Dense(new float[] 
                { normal2.X * c.X + normal2.Z * c.Z,
                    vector1.X * middlePoint.X + vector1.Z * middlePoint.Z });

            var equationResult = matrix.Solve(right);
            return equationResult[0] >= c.X && equationResult[0] <= d.X || equationResult[0] <= c.X && equationResult[0] >= d.X;
        }
        
        
        private int mod(int x, int m) {
            return (x%m + m)%m;
        }
    }
}