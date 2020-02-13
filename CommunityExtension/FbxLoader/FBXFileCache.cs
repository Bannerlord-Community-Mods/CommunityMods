using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaleWorlds.Engine;
using TaleWorlds.Library;

namespace CommunityExtension.FbxLoader
{
    class FBXFileCache
    {
        private static Dictionary<string, List<Mesh>> MeshListByFileName = new Dictionary<string, List<Mesh>>();
        // if (prefab == null)
        public static List<Mesh> GetByFileName(Scene scene, string fileName)
        {
            GameEntity prefab = GameEntity.CreateEmpty(scene, false);


            prefab.EntityFlags |= EntityFlags.DontSaveToScene;

            //prefab.AddMesh(Mesh.GetFromResource("order_arrow_a"), true);

            //   prefab.SetVisibilityExcludeParents(true);

            if (MeshListByFileName.ContainsKey(fileName) == false)
            {

                MeshListByFileName.Add(fileName, CreateMesh(fileName));
            }
            return MeshListByFileName[fileName];

        }

        private static List<(FbxWrapper.Mesh, FbxWrapper.Material[])> meshesFBX = new List<(FbxWrapper.Mesh, FbxWrapper.Material[])>();
        private static void WalkFBXTree(FbxWrapper.Node node)
        {
            if (node.Attribute != null && node.Attribute.Type == FbxWrapper.AttributeType.Mesh)
            {
                meshesFBX.Add((node.Mesh, node.GetMaterials()));
            }
            int childs = node.GetChildCount();
            for (int i = 0; i < childs; i++)
            {
                WalkFBXTree(node.GetChild(i));
            }
        }

        private static Vec3 fromFBX(FbxWrapper.Vector3 vec3)
        {
            return new Vec3((float)vec3.X, (float)vec3.Y, (float)vec3.Z);
        }
        private static Vec2 fromFBX(FbxWrapper.Vector2 vec2)
        {
            return new Vec2((float)vec2.X, (float)vec2.Y);
        }
        private static List<Mesh> CreateMesh(string filename = "test.fbx")
        {
            List<Mesh> meshes = new List<Mesh>();
            FbxWrapper.Manager wrapper = new FbxWrapper.Manager();
            var scene = FbxWrapper.Scene.Import(filename, -1);
            FbxWrapper.Node root = scene.RootNode;
            WalkFBXTree(root);
            foreach ((FbxWrapper.Mesh mesh, FbxWrapper.Material[] Materialist) in meshesFBX)
            {
                Mesh Mesh = Mesh.CreateMesh();
                UIntPtr ptr = Mesh.LockEditDataWrite();
                int numVertices = mesh.ControlPointsCount;
                var textcoords = mesh.GetTextCoords();
                if (numVertices > 0)
                {
                    int vertexIndex = 0;
                    var polygons = mesh.GetPolygons();

                    for (int polyidx = 0; polyidx < polygons.Length; polyidx++)
                    {
                        int polySize = polygons[polyidx].Indices.Length;
                        switch (polySize)
                        {
                            case 3:
                                Mesh.AddTriangle(fromFBX(mesh.GetControlPointAt(polygons[polyidx].Indices[0])),
                                        fromFBX(mesh.GetControlPointAt(polygons[polyidx].Indices[1])),
                                        fromFBX(mesh.GetControlPointAt(polygons[polyidx].Indices[2])),
                                        fromFBX(textcoords[vertexIndex]),
                                        fromFBX(textcoords[vertexIndex + 1]),
                                        fromFBX(textcoords[vertexIndex + 2]),
                                        536918784U,
                                        ptr);
                                vertexIndex += 3;
                                break;
                            case 4:
                                Mesh.AddTriangle(fromFBX(mesh.GetControlPointAt(polygons[polyidx].Indices[0])),
                                        fromFBX(mesh.GetControlPointAt(polygons[polyidx].Indices[1])),
                                        fromFBX(mesh.GetControlPointAt(polygons[polyidx].Indices[2])),
                                        fromFBX(textcoords[vertexIndex]),
                                        fromFBX(textcoords[vertexIndex + 1]),
                                        fromFBX(textcoords[vertexIndex + 2]),
                                        536918784U,
                                        ptr);
                                Mesh.AddTriangle(fromFBX(mesh.GetControlPointAt(polygons[polyidx].Indices[0])),
                                    fromFBX(mesh.GetControlPointAt(polygons[polyidx].Indices[2])),
                                    fromFBX(mesh.GetControlPointAt(polygons[polyidx].Indices[3])),
                                     fromFBX(textcoords[vertexIndex]),
                                        fromFBX(textcoords[vertexIndex + 2]),
                                        fromFBX(textcoords[vertexIndex + 3]),
                                    536918784U,
                                    ptr);
                                vertexIndex += 4;
                                break;

                        }

                    }

                }
                foreach (FbxWrapper.Material material in Materialist)
                {

                    Material mat = Material.GetDefaultMaterial().CreateCopy();
                    var diffuseList = material.TexturePaths(FbxWrapper.LayerElementType.TextureDiffuse);
                    var diffuse = diffuseList.Length > 0 ? diffuseList[0] : null;

                    var bumpList = material.TexturePaths(FbxWrapper.LayerElementType.TextureBump);
                    var bump = bumpList.Length > 0 ? bumpList[0] : null;

                    var specularList = material.TexturePaths(FbxWrapper.LayerElementType.TextureSpecular);
                    var specular = specularList.Length > 0 ? specularList[0] : null;
                    if (!diffuse.IsStringNoneOrEmpty())
                    {

                        mat.SetTexture(Material.MBTextureType.DiffuseMap, Texture.CreateTextureFromPath(System.IO.Path.GetDirectoryName(diffuse), System.IO.Path.GetFileName(diffuse)));
                    }
                    if (!bump.IsStringNoneOrEmpty())
                    {
                        mat.SetTexture(Material.MBTextureType.BumpMap, Texture.CreateTextureFromPath(System.IO.Path.GetDirectoryName(bump), System.IO.Path.GetFileName(bump)));
                    }
                    if (!specular.IsStringNoneOrEmpty())
                    {
                        mat.SetTexture(Material.MBTextureType.SpecularMap, Texture.CreateTextureFromPath(System.IO.Path.GetDirectoryName(specular), System.IO.Path.GetFileName(specular)));
                    }

                    Mesh.SetMaterial(mat);

                }

                Mesh.ComputeNormals();
                Mesh.ComputeTangents();
                Mesh.RecomputeBoundingBox();
                Mesh.UnlockEditDataWrite(ptr);
                meshes.Add(Mesh);
            }



            meshesFBX.Clear();
            return meshes;
        }
    }
}
