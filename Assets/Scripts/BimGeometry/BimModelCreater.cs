using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BimModelCreater 
{
    public static GameObject InstantiateProduct(BimProduct product)
    {
        var go = new GameObject(product.productLabel.ToString());

        //添加空间结构代码
        //-------------
        var data = go.AddComponent<ProductData>();
        data.ProductGeometryData = product;
        SpatialStructureTest.productDatas.Add(data);
        //-------------
        foreach(var shapeInstance in product.bimShapeInstances)
        {
            InstantiateShapeInstance(shapeInstance, go);
        }
        return go;
    }

    private static void InstantiateShapeInstance(BimShapeInstance shapeInstance,GameObject productObj)
    {
        GameObject go = new GameObject(shapeInstance.productLabel.ToString());
        go.transform.parent = productObj.transform;
        MeshFilter meshFilter = go.AddComponent<MeshFilter>();
        Mesh mesh = meshFilter.mesh;
        
        foreach(var tri in shapeInstance.triangulations)
        {
            mesh.vertices = tri.vertices.ToArray();
            mesh.triangles = tri.triangles.ToArray();
            mesh.normals = tri.normals.ToArray();
        }
        MeshRenderer meshRenderer = go.AddComponent<MeshRenderer>();
        Material material = new Material(Shader.Find("Unlit/NormalRender"));
        
        var color = BimGeomorty.styles.Find(c => c.styleLabel == shapeInstance.styleLabel);
        material.color = color.color;
        //meshRenderer.material = material;
        if (color.a < 0.9f)
        {
            material.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
            material.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
            material.SetInt("_ZWrite", 0);
            material.renderQueue = 3000;
        }
        else
        {
            material.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.One);
            material.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.Zero);
            material.SetInt("_ZWrite", 1);
            material.renderQueue = -1;
        }
        meshRenderer.material = material;
        go.tag = "ShapeInstance";
    }

    //空间结构
    //------------------
    /// <summary>
    /// 通过SpatialData来创建层次结构
    /// </summary>
    /// <param name="sd"></param>
    /// <returns></returns>
    private static ISpatialData FindSubSpatialStructure(ISpatialData sd)
    {
        HashSet<string> typeName = new HashSet<string>();
        foreach(var p in sd.SubProducts)
        {
            if (!typeName.Contains(p.TypeName))
            {
                typeName.Add(p.TypeName);
                var go = new GameObject(p.TypeName);
                //Debug.Log(go.name);
                go.transform.parent = sd.TheGameObject.transform;
            }
            p.TheGameObject.transform.parent = sd.TheGameObject.transform.Find(p.TypeName);
            if (p.DecomposedProducts.Count > 0)
            {
                foreach(var dp in p.DecomposedProducts)
                {
                    dp.TheGameObject.transform.parent = p.TheGameObject.transform;
                }
            }
        }
        if(sd.SubSpatials.Count == 0)
        {
            return sd;
        }
        foreach(var ss in sd.SubSpatials)
        {
            ss.TheGameObject.transform.parent = sd.TheGameObject.transform;
            FindSubSpatialStructure(ss);
        }
        return sd;
    }
    /// <summary>
    /// 根据ProjectData创建层次结构
    /// </summary>
    /// <param name="pd"></param>
    /// <returns></returns>
    public static GameObject InstantiateSpatialStructure(IProjectData pd)
    {
        foreach(var sptial in pd.SubSpatials)
        {
            sptial.TheGameObject.transform.parent = pd.TheGameObject.transform;
            FindSubSpatialStructure(sptial);
        }
        return pd.TheGameObject;
    }
}
