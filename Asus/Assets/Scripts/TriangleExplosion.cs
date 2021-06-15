using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriangleExplosion : MonoBehaviour
{
    Mesh mesh;
    Material[] materials;
    public GameObject destoyObject;
    // Start is called before the first frame update
    void Start()
    {
        mesh = new Mesh();
        if (destoyObject.GetComponent<MeshFilter>())
        {
            mesh = destoyObject.GetComponent<MeshFilter>().mesh;
        }

        materials = new Material[0];
        if (destoyObject.GetComponent<MeshRenderer>())
        {
            materials = destoyObject.GetComponent<MeshRenderer>().materials;
        }
    }

    public void ExplosionMesh()
    {
        if (destoyObject.GetComponent<Collider>())
        {
            destoyObject.GetComponent<Collider>().enabled = false;
        }
        Vector3[] verts = mesh.vertices;
        Vector3[] normals = mesh.normals;
        Vector2[] uvs = mesh.uv;
        for (int submesh = 0; submesh < mesh.subMeshCount; submesh++)
        {
            int[] indices = mesh.GetTriangles(submesh);

            for (int i = 0; i < indices.Length; i += 3)
            {
                Vector3[] newVerts = new Vector3[3];
                Vector3[] newNormals = new Vector3[3];
                Vector2[] newUvs = new Vector2[3];
                for (int n = 0; n < 3; n++)
                {
                    int index = indices[i + n];
                    newVerts[n] = verts[index];
                    newUvs[n] = uvs[index];
                    newNormals[n] = normals[index];
                }

                Mesh mesh = new Mesh();
                mesh.vertices = newVerts;
                mesh.normals = newNormals;
                mesh.uv = newUvs;

                mesh.triangles = new int[] { 0, 1, 2, 2, 1, 0 };

                GameObject triangle = new GameObject("Triangle " + (i / 3));
                triangle.transform.position = transform.position;
                triangle.transform.rotation = transform.rotation;
                triangle.AddComponent<MeshRenderer>().material = materials[submesh];
                triangle.AddComponent<MeshFilter>().mesh = mesh;
                triangle.AddComponent<BoxCollider>();
                Vector3 explosionPos = new Vector3(transform.position.x + Random.Range(-0.5f, 0.5f), transform.position.y + Random.Range(0f, 0.5f), transform.position.z + Random.Range(-0.5f, 0.5f));
                triangle.AddComponent<Rigidbody>().AddExplosionForce(Random.Range(300, 500), explosionPos, 5);
                Destroy(triangle, 5 + Random.Range(0.0f, 5.0f));
            }
        }

        destoyObject.GetComponent<Renderer>().enabled = false;

        Destroy(gameObject);
    }
}