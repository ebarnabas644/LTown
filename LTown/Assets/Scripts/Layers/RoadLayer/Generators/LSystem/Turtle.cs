﻿using System;
using System.Collections.Generic;
using DataTypes;
using DataTypes.Graph;
using DataTypes.Graph.Assets.Scripts.Graph;

namespace RoadLayer.Generators
{
        public struct SavedUnit
    {
        public Node<Unit> current;
        public Node<Unit> previous;
    }
    public class Turtle
    {
        private Node<Unit> currentUnit;
        private Node<Unit> previousUnit;
        private float lineLenght;
        private float angleInc;
        private Stack<SavedUnit> transforms;
        private Graph<Unit> roadBlueprint;
        public float randomAngle { get; set; }
        public int randomMultiplier { get; set; }
        public float SnapRange { get; set; }
        public int intersectionRoadNumber { get; set; }
        public int roadMinLenght { get; set; }
        public int roadMaxLenght { get; set; }
        public Turtle(Unit startPoint)
        {
            this.currentUnit = new Node<Unit>(startPoint);
            this.angleInc = 0;
            this.lineLenght = 0;
            this.SnapRange = 1f;
            this.roadMinLenght = 10;
            this.roadMaxLenght = 10;
            this.intersectionRoadNumber = 4;
            this.transforms = new Stack<SavedUnit>();
            this.roadBlueprint = new Graph<Unit>();
            this.previousUnit = this.currentUnit;
            this.roadBlueprint.AddVertex(this.previousUnit);
        }

        public float GetX
        {
            get
            {
                return this.currentUnit.GetContent.X;
            }
        }

        public float GetY
        {
            get
            {
                return this.currentUnit.GetContent.Y;
            }
        }

        public float GetZ
        {
            get
            {
                return this.currentUnit.GetContent.Z;
            }
        }

        public float GetAngle
        {
            get
            {
                return this.currentUnit.GetContent.Angle;
            }
        }

        public float GetLineLenght
        {
            get
            {
                return this.lineLenght;
            }
        }

        public Graph<Unit> GetRoadBlueprint
        {
            get
            {
                return this.roadBlueprint;
            }
        }

        public void PlaceRoad()
        {
            Random rnd = new Random();
            float newAngle = rnd.Next(1,this.randomMultiplier + 1) * this.randomAngle;
            this.lineLenght = rnd.Next(this.roadMinLenght, this.roadMaxLenght + 1);
            double radian = newAngle * Math.PI / 180;
            float newX = (float)(GetX + Math.Sin(radian) * GetLineLenght);
            float newZ = (float)(GetZ + Math.Cos(radian) * GetLineLenght);
            Unit newUnit = new Unit(newX, 0, newZ, newAngle);
            this.currentUnit = new Node<Unit>(newUnit);
            Node<Unit> snappedNode = this.SnapToIntersection(this.currentUnit);
            if (snappedNode.Equals(this.currentUnit))
            {
                this.roadBlueprint.AddVertex(currentUnit);
            }
            else
            {
                this.currentUnit = snappedNode;
            }
            Edge<Unit> edge = new Edge<Unit>(previousUnit, currentUnit);
            if (roadBlueprint.GetVertexEdges(snappedNode).Count < this.intersectionRoadNumber)
            {
                this.roadBlueprint.AddEdge(edge);
            }
            this.previousUnit = currentUnit;
        }

        private Node<Unit> SnapToIntersection(Node<Unit> nodeToCheck)
        {
            float minDistance = Int32.MaxValue;
            Node<Unit> closestToTarget = nodeToCheck;
            foreach (var vertex in this.roadBlueprint.GetVertexes())
            {
                float distance = CalculateNodeDistance(nodeToCheck, vertex.Key);
                if (distance < minDistance)
                {
                    closestToTarget = vertex.Key;
                    minDistance = distance;
                }
            }

            if (minDistance > SnapRange)
            {
                return nodeToCheck;
            }

            return closestToTarget;
        }

        private float CalculateNodeDistance(Node<Unit> u, Node<Unit> v)
        {
            float xDistance = u.GetContent.X - v.GetContent.X;
            float zDistance = u.GetContent.Z - v.GetContent.Z;
            return (float)Math.Sqrt(xDistance * xDistance + zDistance * zDistance);
        }

        public void RotateRight()
        {
            this.currentUnit.GetContent.Angle += angleInc;
        }

        public void RotateLeft()
        {
            this.currentUnit.GetContent.Angle -= angleInc;
        }

        public float SetAngleInc
        {
            set
            {
                this.angleInc = value;
            }
        }

        public float SetLineLength
        {
            set
            {
                this.lineLenght = value;
            }
        }

        public void SavePosition()
        {
            SavedUnit transform = new SavedUnit();
            transform.current = this.currentUnit;
            transform.previous = this.previousUnit;
            this.transforms.Push(transform);
        }

        public void LoadPosition()
        {
            SavedUnit transform = this.transforms.Pop();
            this.previousUnit = transform.previous;
            this.currentUnit = transform.current;
        }
    }
}