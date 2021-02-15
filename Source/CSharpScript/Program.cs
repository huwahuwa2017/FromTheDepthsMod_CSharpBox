using BrilliantSkies.Common.CarriedObjects;
using BrilliantSkies.Common.ChunkCreators.Chunks;
using BrilliantSkies.Ftd.Constructs.Modules.All.Chunks;
using BrilliantSkies.Modding;
using BrilliantSkies.Modding.Types;
using CSharpBox;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using UnityEngine;

public class Program
{
    public static LuaBinding Lua;

    private static int OutputCount;

    private static void Start(CSharpBoxClass cSharpBox)
    {
        cSharpBox.ClearLogs();

        Lua = new LuaBinding(cSharpBox.MainConstruct as MainConstruct);



        foreach (MaterialDefinition materialDefinition in Configured.i.Materials.Components)
        {
            TextureDefinition textureDefinition = Configured.i.Textures.Find(materialDefinition.ColorTextureReference.Reference.Guid);

            if (textureDefinition != null)
            {
                string texPath = SeparatorClean(textureDefinition.FilenameOrUrl);
                string texName = Path.ChangeExtension(texPath, null);

                cSharpBox.Log(textureDefinition.Source.ToString());
                cSharpBox.Log("newmtl " + texName);
                cSharpBox.Log("map_Kd " + texName + ".jpg");
                cSharpBox.Log(string.Empty);
            }
        }



        MainConstruct mainConstruct = cSharpBox.MainConstruct as MainConstruct;
        List<AllConstruct> allConstructList = new List<AllConstruct>();
        mainConstruct.AllBasicsRestricted.GetAllConstructsBelowUsAndIncludingUs(allConstructList);

        foreach (AllConstruct allConstruct in allConstructList)
        {
            if (!(allConstruct.Chunks is ConstructableMeshMerger))
            {
                continue;
            }

            Dictionary<MaterialDefinition, List<Mesh>> meshListDictionary = new Dictionary<MaterialDefinition, List<Mesh>>();

            foreach (ICarriedObjectReference iCOR in allConstruct.CarriedObjects.Objects)
            {
                MeshRenderer meshRenderer = iCOR.ObjectItself.GetComponent<MeshRenderer>();
                MeshFilter meshFilter = iCOR.ObjectItself.GetComponent<MeshFilter>();

                if (meshRenderer != null && meshFilter != null)
                {
                    KeyValuePair<Guid, MaterialDefinition> item = Configured.i.Materials.DictionaryOfComponents.FirstOrDefault(d => d.Value.Material == meshRenderer.sharedMaterial);

                    if (!item.Equals(default(KeyValuePair<Guid, MaterialDefinition>)))
                    {
                        if (!meshListDictionary.ContainsKey(item.Value))
                        {
                            meshListDictionary.Add(item.Value, new List<Mesh>());
                        }

                        Mesh newMesh = UnityEngine.Object.Instantiate(meshFilter.sharedMesh);
                        IEnumerable<Vector3> newVertexList = newMesh.vertices.Select(d => meshFilter.transform.localToWorldMatrix.MultiplyPoint(d));
                        newVertexList = newVertexList.Select(d => allConstruct.myTransform.worldToLocalMatrix.MultiplyPoint(d));
                        newMesh.SetVertices(newVertexList.ToList());

                        meshListDictionary[item.Value].Add(newMesh);
                    }
                }
            }

            ConstructableMeshMerger meshMerger = allConstruct.Chunks as ConstructableMeshMerger;
            cSharpBox.Log("AllBlockVertex Count : " + meshMerger.VertexCount);

            foreach (KeyValuePair<int, List<ChunkMesh>> chunkMeshListDictionary in meshMerger.D)
            {
                bool found;

                MaterialDefinition materialDefinition = Configured.i.Materials.FindUsingTheRuntimeId(chunkMeshListDictionary.Key, out found);

                bool flag_0 = chunkMeshListDictionary.Value.Count == 0;
                bool flag_1 = chunkMeshListDictionary.Value.All(d => d.VertCount == 0);

                if (!found || flag_0 || flag_1)
                {
                    continue;
                }

                if (!meshListDictionary.ContainsKey(materialDefinition))
                {
                    meshListDictionary.Add(materialDefinition, new List<Mesh>());
                }

                meshListDictionary[materialDefinition].AddRange(chunkMeshListDictionary.Value.Select(d => d.GetMesh()));
            }



            List<Mesh> meshList = new List<Mesh>();

            foreach (KeyValuePair<MaterialDefinition, List<Mesh>> meshListDictionaryData in meshListDictionary)
            {
                List<Vector3> vertices = new List<Vector3>();
                List<Vector3> normals = new List<Vector3>();
                List<Vector2> uv = new List<Vector2>();
                List<int> triangles = new List<int>();
                int loadVertexCount = 0;

                foreach (Mesh mesh in meshListDictionaryData.Value)
                {
                    vertices.AddRange(mesh.vertices);
                    normals.AddRange(mesh.normals);
                    uv.AddRange(mesh.uv);
                    triangles.AddRange(mesh.triangles.Select(d => d + loadVertexCount));
                    loadVertexCount += mesh.vertexCount;
                }

                TextureDefinition textureDefinition = Configured.i.Textures.Find(meshListDictionaryData.Key.ColorTextureReference.Reference.Guid);

                string texName;
                string texPath = SeparatorClean(textureDefinition.FilenameOrUrl);

                if (textureDefinition == null)
                {
                    texName = "None";
                }
                else
                {
                    texName = Path.ChangeExtension(texPath, null);
                }

                cSharpBox.Log("\ntexturePath : " + texPath);



                Mesh newMesh = new Mesh
                {
                    indexFormat = UnityEngine.Rendering.IndexFormat.UInt32,
                    name = texName
                };

                newMesh.SetVertices(vertices);
                newMesh.SetNormals(normals);
                newMesh.SetUVs(0, uv);
                newMesh.SetTriangles(triangles, 0);

                FlipHorizontal(newMesh);
                meshList.Add(newMesh);

                cSharpBox.Log("vertexCount : " + newMesh.vertices.Length);
            }

            //MeshToFile(meshList, @"C:\Users\TUF_Z390\Desktop\TestObj\Test" + $" ({OutputCount})" + ".obj");
            ++OutputCount;
        }
    }

    private static void Update(CSharpBoxClass cSharpBox)
    {
        Transform myTransform = cSharpBox.MainConstruct.GameObject.myTransform;
        Rigidbody rigidbody = cSharpBox.MainConstruct.GameObject.Rigidbody;

        myTransform.position = new Vector3(0, 10, 0);
        myTransform.rotation = Quaternion.Euler(0, 0, 0);
        rigidbody.velocity = Vector3.zero;
        rigidbody.angularVelocity = Vector3.zero;

        //Vector3 position = Lua.GetConstructPosition();
        //Lua.LogToHud("Position : " + position.ToString());
    }

    public static string SeparatorClean(string path)
    {
        char directorySeparatorChar = Path.DirectorySeparatorChar;
        path = path.Replace('\\', directorySeparatorChar).Replace('/', directorySeparatorChar);
        return path;
    }

    public static void FlipHorizontal(Mesh mesh)
    {
        IEnumerable<Vector3> newVertices = mesh.vertices.Select(d => new Vector3(-d.x, d.y, d.z));
        mesh.SetVertices(newVertices.ToList());

        int[] triangles = mesh.triangles;

        for (int index = 0; index < triangles.Length; index += 3)
        {
            var temp = triangles[index];
            triangles[index] = triangles[index + 1];
            triangles[index + 1] = temp;
        }

        mesh.RecalculateBounds();
        mesh.RecalculateNormals();
        mesh.RecalculateTangents();
    }

    public static string MeshToString(List<Mesh> meshList)
    {
        StringBuilder sb = new StringBuilder();
        int importVertexCount = 1;

        sb.Append("mtllib " + "Test.mtl");

        foreach (Mesh mesh in meshList)
        {
            sb.Append("\n");
            sb.Append("\n" + "g " + mesh.name);
            sb.Append("\n" + "usemtl " + mesh.name);

            foreach (Vector3 v in mesh.vertices)
            {
                sb.Append("\n" + $"v {v.x} {v.y} {v.z}");
            }

            foreach (Vector3 v in mesh.normals)
            {
                sb.Append("\n" + $"vn {v.x} {v.y} {v.z}");
            }

            foreach (Vector3 v in mesh.uv)
            {
                sb.Append("\n" + $"vt {v.x} {v.y}");
            }

            for (int material = 0; material < mesh.subMeshCount; material++)
            {
                int[] triangles = mesh.GetTriangles(material);

                for (int i = 0; i < triangles.Length; i += 3)
                {
                    sb.Append(string.Format("\n" + "f {0}/{0}/{0} {1}/{1}/{1} {2}/{2}/{2}",
                        triangles[i] + importVertexCount, triangles[i + 1] + importVertexCount, triangles[i + 2] + importVertexCount));
                }
            }

            importVertexCount += mesh.vertexCount;
        }

        return sb.ToString();
    }

    public static void MeshToFile(List<Mesh> m, string filename)
    {
        using (StreamWriter sw = new StreamWriter(filename))
        {
            sw.Write(MeshToString(m));
        }
    }
}
