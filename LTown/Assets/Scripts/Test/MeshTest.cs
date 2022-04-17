using UnityEngine;

public class MeshTest : MonoBehaviour
{
    Vector2[] vertices2D = new Vector2[] {
			new Vector2(0,0),
			new Vector2(0,50),
			new Vector2(50,50),
			new Vector2(50,100),
			new Vector2(0,100),
			new Vector2(0,150),
			new Vector2(150,150),
			new Vector2(150,100),
			new Vector2(100,100),
			new Vector2(100,50),
			new Vector2(150,50),
			new Vector2(150,0),
		};
    
    // Start is called before the first frame update
    void Start()
    {
        //vertices2D = new [] { new Vector2(0, 0), new Vector2(0, 1), new Vector2(1, 1), new Vector2(1, 0) };
        Triangulator tr = new Triangulator(vertices2D);
        int[] indices = tr.Triangulate();
        Vector3[] vertices = new Vector3[vertices2D.Length];
        for (int i=0; i<vertices.Length; i++) {
            vertices[i] = new Vector3(vertices2D[i].x, 0, vertices2D[i].y);
        }
        
        Mesh mesh = new Mesh();
        mesh.vertices = vertices;
        mesh.triangles = indices;
        mesh.RecalculateNormals();
        mesh.RecalculateBounds();
        
        gameObject.AddComponent(typeof(MeshRenderer));
        MeshFilter filter = gameObject.AddComponent(typeof(MeshFilter)) as MeshFilter;
        filter.mesh = mesh;

    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
