using System;
using System.Collections.Generic;
using System.Linq;
using DataTypes;
using DataTypes.Graph;

namespace Layers.PlotLayer
{
    public class SubPlotGenerator
    {
        private HashSet<Polygon<Unit>> plots;

        public SubPlotGenerator(HashSet<Polygon<Unit>> plots)
        {
            this.plots = plots;
        }

        public HashSet<Polygon<Unit>> Generate()
        {
            var result = SplitPolygon(plots.First(x => x.Lenght > 0));
            return new HashSet<Polygon<Unit>>();
        }

        public Tuple<Polygon<Unit>, Polygon<Unit>> SplitPolygon(Polygon<Unit> polygon)
        {
            var longestEdge = polygon.GetLongestEdge();
            var middlePoint = longestEdge.MiddlePoint();
            var furthestEdge = longestEdge;
            var polygonEdges = polygon.GetPolygonEdges();

            foreach (var edge in polygonEdges)
            {
                if (middlePoint.DistanceFrom(edge.MiddlePoint()) > middlePoint.DistanceFrom(furthestEdge.MiddlePoint()))
                {
                    furthestEdge = edge;
                }
            }

            var furthestMiddlePoint = furthestEdge.MiddlePoint();

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
        
        private int mod(int x, int m) {
            return (x%m + m)%m;
        }
    }
}