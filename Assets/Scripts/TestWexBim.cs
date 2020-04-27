using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Xbim.Ifc;
using System.IO;
using Xbim.Common.Geometry;
using Xbim.Common.XbimExtensions;

public class TestWexBim : MonoBehaviour
{
    //颜色字典
    Dictionary<int, Vector4> styleDictionary = new Dictionary<int, Vector4>();
    //product字典
    Dictionary<int, GameObject> productDictionary = new Dictionary<int, GameObject>();

    public GameObject prefab;
    private float scale; 
   
    void Start()
    {
        //Mesh mesh = new Mesh();
        //List<>
        string path = @"E:\Unity\BIMForUnity\Assets\test1.wexbim";
        using(var fs = new FileStream(path, FileMode.Open, FileAccess.Read))
        {
            using(var br = new BinaryReader(fs))
            {
                var magicNumber = br.ReadInt32();
                var version = br.ReadByte();
                var shapeCount = br.ReadInt32();
                var vertexCount = br.ReadInt32();
                var triangleCount = br.ReadInt32();
                var matrixCount = br.ReadInt32();
                var productCount = br.ReadInt32();
                var styleCount = br.ReadInt32();
                var meter = br.ReadSingle();
                scale = meter;
               
                
                var regionCount = br.ReadInt16();

                
                for (int i = 0; i < regionCount; i++)
                {
                    var population = br.ReadInt32();
                    var centreX = br.ReadSingle();
                    var centreY = br.ReadSingle();
                    var centreZ = br.ReadSingle();
                    var boundsBytes = br.ReadBytes(6 * sizeof(float));
                    
                    var modelBounds = XbimRect3D.FromArray(boundsBytes);
                    
                }

                for (int i = 0; i < styleCount; i++)
                {
                    var styleId = br.ReadInt32();
                    var red = br.ReadSingle();
                    var green = br.ReadSingle();
                    var blue = br.ReadSingle();
                    var alpha = br.ReadSingle();

                    styleDictionary.Add(styleId, new Vector4(red, green, blue, alpha));

                }

                for (int i = 0; i < productCount; i++)
                {
                    var productLabel = br.ReadInt32();
                    var productType = br.ReadInt16();
                    var boxBytes = br.ReadBytes(6 * sizeof(float));
                    XbimRect3D bb = XbimRect3D.FromArray(boxBytes);
                   
                    GameObject go = new GameObject(productLabel.ToString());
                    go.transform.SetParent(GameObject.Find("Model").transform);
                    
                    //储存productLabel
                    productDictionary.Add(productLabel, go);
                    
                }

                for (int i = 0; i < shapeCount; i++)
                {
                    var shapeRepetition = br.ReadInt32();
                   
                    //   Assert.IsTrue(shapeRepetition > 0);
                    if (shapeRepetition > 1)
                    {
                        Material material = new Material(Shader.Find("CrossSection/OnePlaneBSP"));
                        Vector4 color = new Vector4();
                        List<int> ifcProductLabels = new List<int>();
                        List<int> styleIds = new List<int>();
                        List<XbimMatrix3D> transforms = new List<XbimMatrix3D>();

                        for (int j = 0; j < shapeRepetition; j++)
                        {
                            var ifcProductLabel = br.ReadInt32();
                            var instanceTypeId = br.ReadInt16();
                            var instanceLabel = br.ReadInt32();
                            var styleId = br.ReadInt32();
                            var transform = XbimMatrix3D.FromArray(br.ReadBytes(sizeof(double) * 16));
                            styleDictionary.TryGetValue(styleId, out color);
                            ifcProductLabels.Add(ifcProductLabel);
                            styleIds.Add(styleId);
                            transforms.Add(transform);
                            //Debug.Log(transform.ToString());
                        }
                       
                        var triangulation = br.ReadShapeTriangulation();
                        
                        ////    Assert.IsTrue(triangulation.Vertices.Count > 0, "Number of vertices should be greater than zero");
                        for(int j = 0; j < shapeRepetition; j++)
                        {

                            GetModel(triangulation.Transform(transforms[j]), styleIds[j], ifcProductLabels[j]);
                        }

                    }
                    else if (shapeRepetition == 1)
                    {
                        var ifcProductLabel = br.ReadInt32();
                        var instanceTypeId = br.ReadInt16();
                        var instanceLabel = br.ReadInt32();
                        var styleId = br.ReadInt32();
                        XbimShapeTriangulation triangulation = br.ReadShapeTriangulation();
                        //     Assert.IsTrue(triangulation.Vertices.Count > 0, "Number of vertices should be greater than zero"); 
                        //---------------------------------------------
                        //GetModel(triangulation, styleId, ifcProductLabel);
                    }
                }
            }
            GameObject.Find("Model").transform.eulerAngles = new Vector3(-90, 0, 0);
        }
        
    }
    
    private void GetModel(XbimShapeTriangulation shapeTriangulation,int styleId,int ifcProductLabel)
    {
        //生成模型
        GameObject go = Instantiate(prefab);
        Mesh mesh = new Mesh();
        go.GetComponent<MeshFilter>().mesh = mesh;
        List<XbimPoint3D> xbimPoint3Ds = new List<XbimPoint3D>();
        xbimPoint3Ds = (List<XbimPoint3D>)shapeTriangulation.Vertices;

        List<Vector3> vector3s = new List<Vector3>();
        List<int> indices = new List<int>();
        List<Vector2> uvs = new List<Vector2>();
        

        List<float[]> positions = new List<float[]>();
        List<Vector3> vertices = new List<Vector3>();
        List<Vector3> normals = new List<Vector3>();

        foreach (var points in xbimPoint3Ds)
        {
            vector3s.Add(new Vector3((float)points.X, (float)points.Y, (float)points.Z));
        }
        foreach (var face in (List<XbimFaceTriangulation>)shapeTriangulation.Faces)
        {
            foreach (var indice in face.Indices)
            {
                indices.Add(indice);
            }
            foreach (var uv in face.Normals)
            {
                //normals.Add(new Vector3((float)uv.Normal.X, (float)uv.Normal.Y, (float)uv.Normal.Z));
                uvs.Add(new Vector2(uv.U, uv.V));
            }
        }

        
        shapeTriangulation.ToPointsWithNormalsAndIndices(out positions, out indices);
        foreach(var position in positions)
        {
            vertices.Add(new Vector3(position[0]/scale, position[1]/scale, position[2]/scale));
            normals.Add(new Vector3(position[3], position[4], position[5]));
        }
       


        mesh.vertices = vertices.ToArray();
        //mesh.vertices = vector3s.ToArray();
        //mesh.uv = uvs.ToArray();
        mesh.triangles = indices.ToArray();
        //mesh.Optimize();
        //mesh.RecalculateNormals();
        mesh.normals = normals.ToArray();
        
        //赋予材质
        Material material = new Material(Shader.Find("CrossSection/OnePlaneBSP"));
        Vector4 color = new Vector4();
        styleDictionary.TryGetValue(styleId, out color);
        material.color = color;
        go.GetComponent<Renderer>().material = material;
        //设置父子关系
        GameObject parent;
        productDictionary.TryGetValue(ifcProductLabel, out parent);
        go.transform.SetParent(parent.transform);
        go.AddComponent<PlaneCuttingController>();
    }



    
}
