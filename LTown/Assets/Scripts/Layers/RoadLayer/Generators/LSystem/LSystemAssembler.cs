﻿using DataTypes;
using DataTypes.Map;

namespace RoadLayer.Generators
{
    public class LSystemAssembler
    {
        private Turtle worker;
        private LSystem lSystem;

        public LSystemAssembler(LSystem lSystem, Unit startPoint, int chunkSize)
        {
            this.worker = new Turtle(startPoint, chunkSize);
            this.lSystem = lSystem;
        }

        public Turtle GetTurtle => worker;

        private void RunLsystem(int iterate)
        {
            for (int i = 0; i < iterate; i++)
            {
                this.lSystem.Iterate();
            }
        }

        public void Draw(int iterate, float lineLenghtIncrement = 1)
        {
            RunLsystem(iterate);
            string commands = this.lSystem.GetAxiom;
            foreach (var command in commands)
            {
                switch (command)
                {
                    case 'F':
                        this.worker.PlaceRoad();
                        break;
                    case '+':
                        this.worker.RotateRight();
                        break;
                    case '-':
                        this.worker.RotateLeft();
                        break;
                    case '[':
                        this.worker.SavePosition();
                        break;
                    case ']':
                        this.worker.LoadPosition();
                        break;
                }
            }
            this.worker.SetLineLength = this.worker.GetLineLenght * lineLenghtIncrement;
        }

        public Map<Unit> GenerateGraph()
        {
            return worker.GetRoadBlueprint;
        }
    }
}