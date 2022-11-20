﻿using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using ConvertLayer;
using DataTypes;
using DataTypes.Map;
using Layers.PlotLayer;
using Layers.RoadLayer.PostProcessing;
using RoadLayer.Generators;
using UnityEngine;
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
    public int chunkSize = 100;

    private Unit startPoint;

    private RoadBuilder roadBuilderScript;
    // Start is called before the first frame update
    void Start()
    {
        startPoint = new Unit(start.transform.position.x, start.transform.position.y, start.transform.position.z, start.transform.rotation.eulerAngles.y);
        roadBuilderScript = roadBuilder.GetComponent<RoadBuilder>();
        CallBuilder();
        ChunkColorizer();
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
        IntersectionFilter<Unit> filter = new IntersectionFilter<Unit>(unitMap);
        unitMap = filter.Filtering();
        test.Stop();
        Debug.Log("Filtering time: "+test.Elapsed.ToString(@"m\:ss\.ff"));
        Debug.Log("Number of vertexes: " + unitMap.GetVertexes().Count);
        test.Reset();

        test.Start();
        var cityMap = _roadSystemConverter.ConvertUnitGraphToGameObjectGraph(unitMap);
        test.Stop();
        Debug.Log("Gameobject conversion time: "+test.Elapsed.ToString(@"m\:ss\.ff"));
        test.Reset();
        
        Debug.Log("Generating plots:");
        
        test.Start();
        var plots = GeneratePlots(cityMap);
        test.Stop();
        Debug.Log("Plot detection time: "+test.Elapsed.ToString(@"m\:ss\.ff"));
        test.Reset();
        
        Debug.Log("Drawing plots:");
        test.Start();
        DrawPlots(plots);
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

    private List<Polygon<CityObject>> GeneratePlots(Map<CityObject> map)
    {
        PlotGenerator<CityObject> plotGenerator = new PlotGenerator<CityObject>(map, RoadMaxLenght);
        return plotGenerator.GenerateFromMap();
    }

    private void DrawPlots(List<Polygon<CityObject>> plots)
    {
        List<Vector2> vertices2D = new List<Vector2>();
        List<Vector3> vertices3D = new List<Vector3>();
        int counter = 0;
        foreach (var plot in plots)
        {
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

            var plotGameObject = new GameObject();
            var plotPivotPoint = new GameObject();
            plotPivotPoint.transform.position = CenterVectorOfPolygon(vertices3D);
            plotPivotPoint.name = "Plot " + counter;
            plotGameObject.name = "Plot plane";
            MeshFilter filter = plotGameObject.AddComponent<MeshFilter>();
            var renderer = plotGameObject.AddComponent<MeshRenderer>();
            renderer.material.color = Color.gray;
            filter.mesh = mesh;
            plotGameObject.transform.SetParent(plotPivotPoint.transform);
            
            plotPivotPoint.transform.localScale = new Vector3(0.9f, 0.1f * 0.9f, 0.9f);
            counter++;
            vertices2D.Clear();
            vertices3D.Clear();
        }
    }

    private Vector3 CenterVectorOfPolygon(List<Vector3> vectors)
    {
        var avg = new Vector3(0, 0, 0);
        foreach (var vector in vectors)
        {
            avg += vector;
        }

        avg /= vectors.Count;

        return avg;
    }
    
    private bool IsClockwisePolygon(Polygon<CityObject> polygon)
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