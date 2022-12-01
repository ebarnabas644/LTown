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
                var counter = 0;
                var boundingBox = BoundingBox(plot);
                Vec3 randomPos = new Vec3(0, 0, 0);
                do
                {
                    randomPos = new Vec3(
                        Random.Range(boundingBox.Item1.X, boundingBox.Item2.X), 0,
                        Random.Range(boundingBox.Item1.Z, boundingBox.Item2.Z));
                    counter++;
                } while (!IsPointInPolygon(randomPos, plot) && counter < 10);

                if (counter < 10)
                {
                    builder.BuildBuilding(randomPos, plot.PlotType);
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