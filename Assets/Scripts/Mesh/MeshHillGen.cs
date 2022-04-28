using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Create a hill shaped mesh based on inspector values
[RequireComponent(typeof(MeshFilter))]
public class MeshHillGen : MonoBehaviour
{

    Mesh mesh;
    Vector3[] vertices;
    int[] triangles;

    public int xSize = 20;
    public int zSize = 20;

    private Vector3 midPoint;

    // Awake is called before Start()
    void Awake()
    {
        midPoint = new Vector3(xSize / 2, 0, zSize / 2);
        mesh = new Mesh();
        GetComponent<MeshFilter>().mesh = mesh;

        CreateShape();
        UpdateMesh();
    }

    /// <summary>
    /// Create the mesh
    /// </summary>
    void CreateShape()
    {
        vertices = new Vector3[(xSize + 1) * (zSize + 1)];

        int i = 0;

        for (int z = 0; z <= zSize; z++)
        {
            for (int x = 0; x <= xSize; x++)
            {
                float y = Mathf.PerlinNoise(x * .5f, z * .5f) + 10/ (Mathf.Sqrt((midPoint.z-z)*(midPoint.z - z) + (midPoint.x - x)*(midPoint.x - x)) + 1);
                vertices[i] = new Vector3(x, y, z);
                i++;
            }
        }

        triangles = new int[xSize * zSize * 6];

        int vertex = 0;
        int tris = 0;

        for (int z = 0; z < zSize; z++)
        {
            for (int x = 0; x < xSize; x++)
            {
                triangles[tris] = vertex;
                triangles[tris + 1] = vertex + xSize + 1;
                triangles[tris + 2] = vertex + 1;
                triangles[tris + 3] = vertex + 1;
                triangles[tris + 4] = vertex + xSize + 1;
                triangles[tris + 5] = vertex + xSize + 2;


                vertex++;
                tris += 6;

            }
            vertex++;
        }
    }

    /// <summary>
    /// Update the mesh in case vertices are changed
    /// </summary>
    void UpdateMesh()
    {
        mesh.Clear();
        mesh.vertices = vertices;
        mesh.triangles = triangles;
        mesh.RecalculateNormals();
    }

    void Update()
    {
        CreateShape();
        UpdateMesh();
    }

}
