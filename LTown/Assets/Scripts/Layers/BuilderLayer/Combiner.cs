using ConvertLayer;
using DataTypes;
using RoadLayer.Generators;
using UnityEngine;

namespace BuilderLayer
{
    public class Combiner : MonoBehaviour
    {
        private RoadSystemConverter _roadSystemConverter;
        private LSystem _lSystem;
        private LSystemAssembler _lSystemAssembler;

    [SerializeField]
    public GameObject roadBuilder;
    [SerializeField]
    public string axiom;
    [SerializeField]
    public int numberOfIterations;
    [SerializeField]
    public GameObject start;
    [SerializeField]
    public int RoadMinLenght = 10;
    [SerializeField]
    public int RoadMaxLenght = 10;
    [SerializeField]
    public int randomMultiplier = 4;
    [SerializeField]
    public int randomAngle = 60;
    [SerializeField]
    public int intersectionRoadNumber = 4;
    [SerializeField]
    public float snapRange = 1f;

    // Start is called before the first frame update
    void Start()
    {
        CallBuilder();

        Debug.Log("Unit graph: ");
        GraphTest();
        Debug.Log("Gameobject graph: ");
        GameObjectGraphTest();
    }

    void CallBuilder()
    {
        Unit startPoint = new Unit(start.transform.position.x, start.transform.position.y, start.transform.position.z, start.transform.rotation.eulerAngles.y);
        LSystemConfigurator();
        _lSystemAssembler = new LSystemAssembler(_lSystem, startPoint);
        TurtleConfigurator();
        _lSystemAssembler.Draw(numberOfIterations);
        _roadSystemConverter = new RoadSystemConverter(roadBuilder.GetComponent<RoadBuilder>());
        _roadSystemConverter.ConvertUnitGraphToGameObjectGraph(_lSystemAssembler.GenerateGraph());
        Debug.Log(_lSystem.GetAxiom);
    }

    void LSystemConfigurator()
    {
        _lSystem = new LSystem(axiom);
        _lSystem.AddRule('F', "FF[+F-F-F]-[-F+F+F]");
        //this._lSystem.AddRule('F', "FF[-F++F][+F--F]++F--F");
        //this._lSystem.AddRule('F', "FF");
    }

    void TurtleConfigurator()
    {
        _lSystemAssembler.GetTurtle.SetAngleInc = 0;
        _lSystemAssembler.GetTurtle.roadMinLenght = this.RoadMinLenght;
        _lSystemAssembler.GetTurtle.roadMaxLenght = this.RoadMaxLenght;
        _lSystemAssembler.GetTurtle.intersectionRoadNumber = this.intersectionRoadNumber;
        _lSystemAssembler.GetTurtle.randomAngle = this.randomAngle;
        _lSystemAssembler.GetTurtle.SnapRange = this.snapRange;
        _lSystemAssembler.GetTurtle.randomMultiplier = this.randomMultiplier;
    }
    
    void GraphTest()
    {
        foreach (var vertex in _lSystemAssembler.GetTurtle.GetRoadBlueprint.GetVertexes())
        {
            string test = vertex.Key.ToString() + "(" + vertex.Key.GetContent.X + ", " + vertex.Key.GetContent.Z + ")" + ": ";
            foreach (var edge in _lSystemAssembler.GetTurtle.GetRoadBlueprint.GetVertexEdges(vertex.Key))
            {
                test += edge.ToString() + ", ";
            }
            
            Debug.Log(test);
        }
    }

    private void GameObjectGraphTest()
    {
        foreach (var vertex in _roadSystemConverter.convertedGraph.GetVertexes())
        {
            var position = vertex.Key.GetContent.transform.position;
            string test = vertex.Key.ToString() + "(" + position.x + ", " + position.z + ")" + ": ";
            foreach (var edge in _roadSystemConverter.convertedGraph.GetVertexEdges(vertex.Key))
            {
                test += edge.ToString() + ", ";
            }
            Debug.Log(test);
        }
    }
}
}