// Based on http://wiki.unity3d.com/index.php/CreateIcoSphere

using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;

public class CreateUVSphere : ScriptableWizard
{

    public int numberOfSlices = 10;
    public int numberOfRings = 10;
    public float radius = 0.5f;
    public bool addCollider = false;
    public string optionalName;

    static Camera cam;
    static Camera lastUsedCam;

    [MenuItem("GameObject/Create Other/UvSphere...")]
    static void CreateWizard()
    {
        cam = Camera.current;
        // Hack because camera.current doesn't return editor camera if scene view doesn't have focus
        if (!cam)
            cam = lastUsedCam;
        else
            lastUsedCam = cam;
        ScriptableWizard.DisplayWizard("Create UvSphere", typeof(CreateUVSphere));
    }


    void OnWizardUpdate()
    {
        if (numberOfSlices < 3) numberOfSlices = 3;
        if (numberOfRings < 4) numberOfRings = 4;
    }

    void OnWizardCreate()
    {
        GameObject sphere = new GameObject();

        if (!string.IsNullOrEmpty(optionalName))
            sphere.name = optionalName;
        else
            sphere.name = "UvSphere";

        sphere.transform.position = Vector3.zero;

        MeshFilter filter = (MeshFilter)sphere.AddComponent(typeof(MeshFilter));
        sphere.AddComponent(typeof(MeshRenderer));

        string sphereAssetName = sphere.name + numberOfRings + "_" + numberOfSlices + ".asset";
        Mesh mesh = (Mesh)AssetDatabase.LoadAssetAtPath("Assets/Editor/" + sphereAssetName, typeof(Mesh));

        if (mesh == null)
        {
            mesh = new Mesh();
            mesh.name = sphere.name;

            List<Vector3> vertList = new List<Vector3>();
            List<Vector2> UvList = new List<Vector2>();

            // Note: Topologically, this UV sphere is a grid

            List<int> triList = new List<int>();
            for (var ring = 0; ring < numberOfRings; ++ring)
            {
                for (var slice = 0; slice < numberOfSlices; ++slice)
                {
                    var xzRadius = Mathf.Sin((float)ring / (numberOfRings - 1) * 3.14159265f);
                    vertList.Add(new Vector3(
                        xzRadius * Mathf.Sin((float)slice / (numberOfSlices - 1) * 2 * 3.14159265f), // Minus 1 because we want the last vertices to overlap with the first
                        radius * 2 * Mathf.Cos((float)ring / (numberOfRings - 1) * 3.14159265f),
                        xzRadius * Mathf.Cos((float)slice / (numberOfSlices - 1) * 2 * 3.14159265f)
                    ));
                    UvList.Add(new Vector2(
                        (float)slice / numberOfSlices,
                        (float)ring / numberOfRings
                    ));
                }

                if (ring > 0)
                {
                    // Connect this ring to the last one
                    for (var slice = 0; slice < numberOfSlices - 1; ++slice)
                    {
                        var vertexOffset = (ring - 1) * numberOfSlices + slice;

                        // Face 1
                        triList.Add(1 + vertexOffset);
                        triList.Add(0 + vertexOffset);
                        triList.Add(numberOfSlices + vertexOffset);
                        //faces.Add(new TriangleIndices(1 + vertexOffset, 0 + vertexOffset, numberOfSlices + vertexOffset));

                        // Face 2
                        triList.Add(1 + vertexOffset);
                        triList.Add(numberOfSlices + vertexOffset);
                        triList.Add(numberOfSlices + 1 + vertexOffset);
                        //faces.Add(new TriangleIndices(1 + vertexOffset, numberOfSlices + vertexOffset, numberOfSlices + 1 + vertexOffset));
                    }
                }
            }

            mesh.vertices = vertList.ToArray();
            mesh.triangles = triList.ToArray();
            mesh.uv = UvList.ToArray();

            Vector3[] normals = new Vector3[vertList.Count];
            for (int i = 0; i < normals.Length; i++)
                normals[i] = vertList[i].normalized;

            mesh.normals = normals;

            mesh.RecalculateBounds();

            AssetDatabase.CreateAsset(mesh, "Assets/Editor/" + sphereAssetName);
            AssetDatabase.SaveAssets();
        }

        filter.sharedMesh = mesh;
        mesh.RecalculateBounds();

        if (addCollider)
            sphere.AddComponent(typeof(SphereCollider));

        Selection.activeObject = sphere;
    }
}
