using DataTypes;
using DataTypes.Graph;
using DataTypes.Graph.Assets.Scripts.Graph;

namespace RoadLayer.Generators
{
    public class LSystemAssembler
    {
        private Turtle worker;
        private LSystem lSystem;

        public LSystemAssembler(LSystem lSystem, Unit startPoint)
        {
            this.worker = new Turtle(startPoint);
            this.lSystem = lSystem;
        }

        public Turtle GetTurtle => worker;

        public void RunLsystem(int iterate)
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

        public Graph<Unit> GenerateGraph()
        {
            return worker.GetRoadBlueprint;
        }
    }
}