using System;
using System.Collections.Generic;
using Codice.Client.GameUI.Update;
using DataTypes;
using DataTypes.Graph;
using MathNet.Numerics.LinearAlgebra;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Layers.BuildingLayer
{
    public class BuildingGenerator
    {
        private HashSet<Polygon<Unit>> subPlots;
        private BuildingBuilder builder;
        
        public BuildingGenerator(HashSet<Polygon<Unit>> subPlots, GameObject buildingBuilder)
        {
            this.builder = buildingBuilder.GetComponent<BuildingBuilder>();
            this.subPlots = subPlots;
        }

        private void SetPlotType()
        {
            foreach (var plot in subPlots)
            {
                var chance = Random.Range(0f, 1f);
                if (chance < builder.houseChance)
                {
                    plot.PlotType = PlotType.Housing;
                }
                else if (chance < builder.houseChance + builder.marketChance)
                {
                    plot.PlotType = PlotType.Market;
                }
                else
                {
                    plot.PlotType = PlotType.Park;
                }
            }
        }

        private void GenerateBuildings()
        {
            foreach (var plot in subPlots)
            {
                switch (plot.PlotType)
                {
                    case PlotType.Housing:
                        FillHousingPlot(plot);
                        break;
                    case PlotType.Market:
                        FillMarketPlot(plot);
                        break;
                    case PlotType.Park:
                        FillParkPlot(plot, Random.Range(5, 15));
                        break;
                }
            }
        }

        private void FillHousingPlot(Polygon<Unit> plot)
        {
            var counter = 0;
            var boundingBox = BoundingBox(plot);
            var success = false;
            Vec3 randomPos = new Vec3(0, 0, 0);
            do
            {
                randomPos = new Vec3(
                    Random.Range(boundingBox.Item1.X, boundingBox.Item2.X), 0,
                    Random.Range(boundingBox.Item1.Z, boundingBox.Item2.Z));
                if (counter == 9) randomPos = plot.CenterPoint;
                counter++;
                var maxRadius = DistanceOfClosestEdgeFromPointInPolygon(randomPos, plot);
                success = false;
                if (IsPointInPolygon(randomPos, plot))
                {
                    success = builder.BuildBuilding(randomPos, plot.PlotType,
                        DistanceOfClosestEdgeFromPointInPolygon(randomPos, plot), "Building on plot: " + plot.Id);
                }
            } while (!success && counter < 10);
        }
        
        private void FillMarketPlot(Polygon<Unit> plot)
        {
            var counter = 0;
            var boundingBox = BoundingBox(plot);
            var success = false;
            Vec3 randomPos = new Vec3(0, 0, 0);
            do
            {
                randomPos = new Vec3(
                    Random.Range(boundingBox.Item1.X, boundingBox.Item2.X), 0,
                    Random.Range(boundingBox.Item1.Z, boundingBox.Item2.Z));
                if (counter == 9) randomPos = plot.CenterPoint;
                counter++;
                var maxRadius = DistanceOfClosestEdgeFromPointInPolygon(randomPos, plot);
                success = false;
                if (IsPointInPolygon(randomPos, plot))
                {
                    success = builder.BuildBuilding(randomPos, plot.PlotType,
                        DistanceOfClosestEdgeFromPointInPolygon(randomPos, plot), "Building on plot: " + plot.Id);
                }
            } while (!success && counter < 10);
        }
        
        private void FillParkPlot(Polygon<Unit> plot, int numberOfBuilding)
        {
            var successfulBuild = 0;
            var numberOfFails = 0;
            while (successfulBuild < numberOfBuilding && numberOfFails < 5)
            {
                var counter = 0;
                var boundingBox = BoundingBox(plot);
                var success = false;
                Vec3 randomPos = new Vec3(0, 0, 0);
                do
                {
                    randomPos = new Vec3(
                        Random.Range(boundingBox.Item1.X, boundingBox.Item2.X), 0,
                        Random.Range(boundingBox.Item1.Z, boundingBox.Item2.Z));
                    counter++;
                    var maxRadius = DistanceOfClosestEdgeFromPointInPolygon(randomPos, plot);
                    success = false;
                    if (IsPointInPolygon(randomPos, plot))
                    {
                        success = builder.BuildBuilding(randomPos, plot.PlotType,
                            DistanceOfClosestEdgeFromPointInPolygon(randomPos, plot), "Building on plot: " + plot.Id);
                    }
                } while (!success && counter < 10);

                if (counter >= 10)
                {
                    numberOfFails++;
                }
                else
                {
                    successfulBuild++;
                }
            }
        }

        /*private float CalculateMinimumDistanceFromEdges(Vec3 target, Polygon<Unit> plot)
        {
            
        }*/

        /*private float CalculateDistanceFromEdge(Vec3 target, Edge<Unit> edge)
        {
            var edgeNormal = new Vec3(edge.LineEquation[1], 0, -edge.LineEquation[0]);

            var matrix = Matrix<float>.Build.DenseOfArray(new float[,]
            {
                { edge.LineEquation[0], edge.LineEquation[1] },
                { edgeNormal.X, edgeNormal.Z }
            });

            var right = Vector<float>.Build.Dense(new float[]
            {
                { edge.LineEquation[2], }
            });
        }*/

        private bool IsPointInPolygon(Vec3 target, Polygon<Unit> plot)
        {
            var counter = 0;
            foreach (var edge in plot.GetPolygonEdges())
            {
                if (IsIntersecting(target, edge))
                {
                    counter++;
                }
            }

            return counter % 2 == 0;
        }

        private Tuple<Vec3, Vec3> BoundingBox(Polygon<Unit> plot)
        {
            var minX = float.MaxValue;
            var minZ = float.MaxValue;
            var maxX = float.MinValue;
            var maxZ = float.MinValue;

            foreach (var point in plot.Points)
            {
                var pos = point.GetContent.Position;
                if (pos.X < minX)
                {
                    minX = pos.X;
                }
                else if (pos.X > maxX)
                {
                    maxX = pos.X;
                }

                if (pos.Z < minZ)
                {
                    minZ = pos.Z;
                }
                else if (pos.Z > maxZ)
                {
                    maxZ = pos.Z;
                }
            }

            return new Tuple<Vec3, Vec3>(new Vec3(minX, 0, minZ), new Vec3(maxX, 0, maxZ));
        }

        private float DistanceOfClosestEdgeFromPointInPolygon(Vec3 target, Polygon<Unit> polygon)
        {
            float min = float.MaxValue;
            foreach (var edge in polygon.GetPolygonEdges())
            {
                var distance = DistanceFromPointToEdge(target, edge);
                if (min > distance)
                {
                    min = distance;
                }
            }
            return min;
        }

        private float DistanceFromPointToEdge(Vec3 a, Edge<Unit> edge)
        {
            var p1 = edge.Start.GetContent.Position;
            var p2 = edge.End.GetContent.Position;
            return (float)(Math.Abs((p2.X - p1.X) * (p1.Z - a.Z) - (p1.X - a.X) * (p2.Z - p1.Z)) /
                           Math.Sqrt(Math.Pow((p2.X - p1.X), 2) + Math.Pow((p2.Z - p1.Z), 2)));
        }
        
        private bool IsIntersecting(Vec3 a, Edge<Unit> edge)
        {
            var c = edge.Start.GetContent.Position;
            var d = edge.End.GetContent.Position;
            var resultX = (edge.LineEquation[1] * a.Z - edge.LineEquation[2]) / edge.LineEquation[0];
            return a.X < resultX && (resultX >= c.X && resultX <= d.X || resultX <= c.X && resultX >= d.X);
        }

        public void Start()
        {
            SetPlotType();
            GenerateBuildings();
        }
        
    }
}