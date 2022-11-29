﻿using System;
using System.Collections.Generic;
using System.Linq;
using DataTypes;
using DataTypes.Graph;
using DataTypes.Graph.Assets.Scripts.Graph;
using DataTypes.Map;

namespace Layers.PlotLayer
{
    public class PlotGenerator<T> where T : ILocatable
    {
        private Map<T> map;
        private Dictionary<Node<T>, HashSet<Edge<T>>> vertexes;
        private int maxRoadLenght;
        private HashSet<Polygon<T>> plots = new HashSet<Polygon<T>>();
        private List<Polygon<T>> foundedPlots = new List<Polygon<T>>();
        public int NumberOfSearchIterations = 0;

        public PlotGenerator(Map<T> map, int maxRoadLenght)
        {
            this.map = map;
            this.maxRoadLenght = maxRoadLenght;
            vertexes = map.GetVertexes();
        }
        public HashSet<Polygon<T>> GenerateFromMap()
        {
            foreach (var vertex in vertexes)
            {
                var searchResult = new Polygon<T>();
                searchResult.AddPoint(vertex.Key);
                Search(vertex.Key, vertex.Key, searchResult, 0, 5);
                var smallestPlot = new Polygon<T>();
                foreach (var plot in foundedPlots)
                {
                    if (smallestPlot.Lenght == 0 || (smallestPlot.Lenght > plot.Lenght))
                    {
                        smallestPlot = plot;
                    }
                }
                plots.Add(smallestPlot);
                foundedPlots.Clear();
            }

            return plots;
        }

        private void Search(Node<T> startPoint, Node<T> currentPoint, Polygon<T> pathInProgress, int depth, int maxDepth = 5)
        {
            if (!pathInProgress.AddPoint(currentPoint) && depth > 0)
            {
                if (startPoint.Equals(currentPoint) && pathInProgress.Lenght > 2)
                {
                    foundedPlots.Add(pathInProgress);
                }
                return;
            }
            if (depth >= maxDepth) return;
            foreach (var edge in vertexes[currentPoint])
            {
                var endPoint = edge.Start.Equals(currentPoint) ? edge.End : edge.Start;
                if (DistanceFromTwoPoint(startPoint.GetContent, endPoint.GetContent) <= Math.Pow(maxRoadLenght * (maxDepth - depth), 2))
                {
                    NumberOfSearchIterations++;
                    Search(startPoint, endPoint, new Polygon<T>(pathInProgress), depth + 1, maxDepth);
                }
            }
        }

        private float DistanceFromTwoPoint(T point1, T point2)
        {
            var dx = point1.GetPosition().X - point2.GetPosition().X;
            var dz = point1.GetPosition().Z - point2.GetPosition().Z;
            return dx * dx + dz * dz;
        }
    }
}