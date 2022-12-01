using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using ConvertLayer;
using DataTypes;
using DataTypes.Map;
using Layers.BuildingLayer;
using Layers.PlotLayer;
using Layers.RoadLayer.PostProcessing;
using RoadLayer.Generators;
using UnityEngine;
using UnityEngine.Rendering;
using Debug = UnityEngine.Debug;
using Random = UnityEngine.Random;

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
    public GameObject buildingBuilder;
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
    [SerializeField]
    public float targetPlotArea = 15f;
    [SerializeField] 
    public int chunkSize = 100;

    private Unit startPoint;

    private RoadBuilder roadBuilderScript;
    // Start is called before the first frame update
    void Start()
    {
        startPoint = new Unit(start.transform.position.x, start.transform.position.y, start.transform.position.z, start.transform.rotation.eulerAngles.y);
        roadBuilderScript = roadBuilder.GetComponent<RoadBuilder>();
        CallBuilder();
        //ChunkColorizer();
        //GraphTest();
        //GameObjectGraphTest();
        //ChunkColorizer();
        //MapTester();
        
    }

    void CallBuilder()
    {
        LSystemConfigurator();
        Stopwatch test = new Stopwatch();
        _lSystemAssembler = new LSystemAssembler(_lSystem, startPoint, chunkSize);
        TurtleConfigurator();
        
        test.Start();
        _lSystemAssembler.Draw(numberOfIterations);
        test.Stop();
        Debug.Log("Graph generation time: "+test.Elapsed.ToString(@"m\:ss\.ff"));
        test.Reset();

        _roadSystemConverter = new RoadSystemConverter(roadBuilderScript);
        Map<Unit> unitMap = _lSystemAssembler.GenerateGraph();
        test.Start();
        IntersectionFilter<Unit> filter = new IntersectionFilter<Unit>();
        unitMap = filter.Filtering(unitMap);
        unitMap = filter.FilterIntersectionWithoutEdge(unitMap);
        test.Stop();
        Debug.Log("Filtering time: "+test.Elapsed.ToString(@"m\:ss\.ff"));
        Debug.Log("Number of vertexes: " + unitMap.GetVertexes().Count);
        test.Reset();
        
        Debug.Log("Generating plots:");
        
        test.Start();
        var plots = GeneratePlots(unitMap);
        test.Stop();
        Debug.Log("Plot detection time: "+test.Elapsed.ToString(@"m\:ss\.ff"));
        test.Reset();
        
        test.Start();
        var subPlotGenerator = new SubPlotGenerator(plots, targetPlotArea);
        var subPlots = subPlotGenerator.Generate();
        test.Stop();
        Debug.Log("Plot detection time: "+test.Elapsed.ToString(@"m\:ss\.ff"));
        test.Reset();

        test.Start();
        var cityMap = _roadSystemConverter.ConvertUnitGraphToGameObjectGraph(unitMap);
        test.Stop();
        Debug.Log("Gameobject conversion time: "+test.Elapsed.ToString(@"m\:ss\.ff"));
        test.Reset();

        GenerateBuildings(subPlots);
        
        Debug.Log("Drawing plots:");
        test.Start();
        DrawPlots(plots, 0.09f);
        DrawPlots(subPlots, 0.1f);
        test.Stop();
        Debug.Log("Plot drawing time: "+test.Elapsed.ToString(@"m\:ss\.ff"));
        test.Reset();
        
        //PlotTester(plots);
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
    
    private void GraphTest()
    {
        Debug.Log("Unit graph: ");
        foreach (var vertex in _lSystemAssembler.GetTurtle.GetRoadBlueprint.GetVertexes())
        {
            string test = vertex.Key + "(" + vertex.Key.GetContent.X + ", " + vertex.Key.GetContent.Z + ")" + ": ";
            foreach (var edge in _lSystemAssembler.GetTurtle.GetRoadBlueprint.GetVertexEdges(vertex.Key))
            {
                test += edge.ToString() + ", ";
            }
            
            Debug.Log(test);
        }
    }

    private void ChunkColorizer()
    {
        Dictionary<Vec3, Color> colorPalette = new Dictionary<Vec3, Color>();
        Map<CityObject> map = _roadSystemConverter.convertedGraph;
        foreach (var chunk in map.GetChunks())
        {
            colorPalette.Add(chunk.Key, Random.ColorHSV(0f, 1f, 1f, 1f, 0.5f, 1f));
        }
        foreach (var vertex in map.GetVertexes())
        {
            GameObject vertexGo = vertex.Key.GetContent.GetGameObject().transform.Find("IntersectionPlane").gameObject;
            Renderer vertexRenderer = vertexGo.GetComponent<Renderer>();
            vertexRenderer.material.color =
                colorPalette[map.ConvertPositionToChunkPosition(vertex.Key.GetContent.GetPosition())];
            foreach (var road in vertex.Value)
            {
                GameObject roadGo = road.Content.GetGameObject().transform.Find("RoadPlane").gameObject;
                Renderer roadRenderer = roadGo.GetComponent<Renderer>();
                roadRenderer.material.color =
                    colorPalette[map.ConvertPositionToChunkPosition(road.Start.GetContent.GetPosition())] * 0.5f;
            }
        }
    }

    private void GameObjectGraphTest()
    {
        Debug.Log("Gameobject graph: ");
        foreach (var vertex in _roadSystemConverter.convertedGraph.GetVertexes())
        {
            var position = vertex.Key.GetContent.GetGameObject().transform.position;
            string test = vertex.Key + "(" + position.x + ", " + position.z + ")" + ": ";
            foreach (var edge in _roadSystemConverter.convertedGraph.GetVertexEdges(vertex.Key))
            {
                test += edge + ", ";
            }
            Debug.Log(test);
        }
        
        Debug.Log("Number of chunks: "+_roadSystemConverter.convertedGraph.NumberOfChunks());
    }

    private HashSet<Polygon<T>> GeneratePlots<T>(Map<T> map) where T : ILocatable
    {
        PlotGenerator<T> plotGenerator = new PlotGenerator<T>(map, RoadMaxLenght);
        var plots = plotGenerator.GenerateFromMap();
        Debug.Log("Number of search recursive invoke: " + plotGenerator.NumberOfSearchIterations);
        return plots;
    }

    private void DrawPlots<T>(HashSet<Polygon<T>> plots, float yOffset) where T : ILocatable
    {
        List<Vector2> vertices2D = new List<Vector2>();
        List<Vector3> vertices3D = new List<Vector3>();
        int counter = 0;
        foreach (var plot in plots)
        {
            if(plot == null) continue;
            if (IsClockwisePolygon(plot))
            {
                foreach (var point in plot.Points)
                {
                    vertices2D.Add(new Vector2(point.GetContent.GetPosition().X, point.GetContent.GetPosition().Z));
                    vertices3D.Add(new Vector3(point.GetContent.GetPosition().X, 0.1f, point.GetContent.GetPosition().Z));
                }
            }
            
            else
            {
                for (int i = plot.Points.Count - 1; i >= 0; i--)
                {
                    vertices2D.Add(new Vector2(plot.Points[i].GetContent.GetPosition().X, plot.Points[i].GetContent.GetPosition().Z));
                    vertices3D.Add(new Vector3(plot.Points[i].GetContent.GetPosition().X, 0.1f, plot.Points[i].GetContent.GetPosition().Z));
                }
            }

            Triangulator triangulator = new Triangulator(vertices2D.ToArray());
            int[] indices = triangulator.Triangulate();

            Mesh mesh = new Mesh();
            mesh.vertices = vertices3D.ToArray();
            mesh.triangles = indices;
            mesh.RecalculateNormals();
            mesh.RecalculateBounds();
            
            if (vertices3D.Count > 0)
            {
                var plotGameObject = new GameObject();
                var plotPivotPoint = new GameObject();
                var centerPoint = plot.CenterPoint;
                var pivotPointPosition = new Vector3(centerPoint.X, centerPoint.Y, centerPoint.Z);
                plotPivotPoint.transform.position = pivotPointPosition;
                plotPivotPoint.name = "Plot " + plot.Id;
                plotGameObject.name = "Plot plane";
                MeshFilter filter = plotGameObject.AddComponent<MeshFilter>();
                var renderer = plotGameObject.AddComponent<MeshRenderer>();
                renderer.shadowCastingMode = ShadowCastingMode.Off;
                renderer.receiveShadows = false;
                plotGameObject.transform.SetParent(plotPivotPoint.transform);
                //plotPivotPoint.transform.Rotate(Vector3.up, -120);
                
                plotPivotPoint.transform.localScale = new Vector3(0.9f, yOffset * 0.9f, 0.9f);
                switch (plot.PlotType)
                {
                    case PlotType.Housing:
                        renderer.material.color = new Color(0, 0.31f, 0);
                        break;
                    case PlotType.Park:
                        renderer.material.color = new Color(0, 0.45f, 0);
                        break;
                    case PlotType.Market:
                        renderer.material.color = new Color(0.23f, 0.23f, 0.23f);
                        break;
                    case PlotType.Default:
                        renderer.material.color = Color.black;
                        plotPivotPoint.transform.localScale = new Vector3(1f, yOffset * 0.9f, 1f);
                        break;
                }
                filter.mesh = mesh;
                
            }
            counter++;
            vertices2D.Clear();
            vertices3D.Clear();
        }
    }

    public void GenerateBuildings(HashSet<Polygon<Unit>> subPlots)
    {
        BuildingGenerator generator = new BuildingGenerator(subPlots, buildingBuilder);
        generator.Start();
    }

    private bool IsClockwisePolygon<T>(Polygon<T> polygon) where T : ILocatable
    {
        float edgeSum = 0;
        for (int i = 0; i < polygon.Points.Count - 1; i++)
        {
            Vec3 point1 = polygon.Points[i].GetContent.GetPosition();
            Vec3 point2 = polygon.Points[i + 1 % polygon.Points.Count].GetContent.GetPosition();
            edgeSum += (point2.X - point1.X) * (point2.Z + point1.Z);
        }

        return edgeSum < 0;
    }

    private void PlotTester(List<Polygon<CityObject>> plots)
    {
        Debug.Log("Number of plots: "+ plots.Count);
        foreach (var plot in plots)
        {
            Debug.Log(plot);
        }
    }

    private void MapTester()
    {
        Map<Unit> testMap = new Map<Unit>();
        testMap.CreateChunk(new Vec3(0, 0, 0));
        testMap.CreateChunk(new Vec3(20, 0, 0));
        Debug.Log("Number of chunks: "+testMap.NumberOfChunks());
    }
}
}