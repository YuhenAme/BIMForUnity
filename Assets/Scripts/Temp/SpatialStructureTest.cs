using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Xbim.Ifc;
using Xbim.Ifc4.Interfaces;

public class SpatialStructureTest : MonoBehaviour
{
    public static IProjectData project;
    public static List<ISpatialData> spatialDatas = new List<ISpatialData>();
    public static List<IProductData> productDatas = new List<IProductData>();


    public static IProjectData GetIfcSpatialStructure(string fileName)
    {
        using(var model = IfcStore.Open(fileName))
        {
            var proj = model.Instances.FirstOrDefault<IIfcProject>();
            var go = new GameObject();
            var projectData = go.AddComponent<ProjectData>();
            projectData.Name = proj.Name;
            projectData.TheProject = proj;
            project = projectData;
            go.name = projectData.Name;
            //Debug.Log(project.Name);
            //利用 IfcRelAggregares 获取空间结构元素的空间分解
            foreach (var item in proj.IsDecomposedBy.SelectMany(r => r.RelatedObjects))
            {
                projectData.AddSpatial(GetSpatialSturctureData(proj, item));
            }
            var spatialProducts = productDatas.FindAll(p => p.HaveSpatialStructure == false);
            AddGeoProductToSpatialStructure(spatialDatas, spatialProducts);
            return projectData;
        }
    }

    /// <summary>
    /// 递归获取IfcObject的空间结构
    /// </summary>
    /// <param name="parent"></param>
    /// <param name="cur"></param>
    /// <returns></returns>
    private static ISpatialData GetSpatialSturctureData(IIfcObjectDefinition parent, IIfcObjectDefinition cur) 
    {
        ISpatialData sp = default;

        var spatialElement = cur as IIfcSpatialStructureElement;
        if(spatialElement != null)
        {
            //获得spatialElement的spatialData
            sp = InstaniateCurSpatial(spatialElement);
            if(sp != null)
            {
                spatialDatas.Add(sp);
                //使用 IfcRelContainedInSpatialElement 获取包含的元素
                var containedElements = spatialElement.ContainsElements.SelectMany(rel => rel.RelatedElements);
                if (containedElements.Count() > 0)
                {
                    foreach(var element in containedElements)
                    {
                        //查找到相应的productData
                        var prod = productDatas.Find(p => p.ProductGeometryData.productLabel == element.EntityLabel);

                        if(prod == null)
                        {
                            var go = new GameObject();
                            var pd = go.AddComponent<ProductData>();
                            pd.ProductGeometryData = new BimProduct(element.EntityLabel, (short)element.EntityLabel);
                            SetProductData(pd, element);
                            sp.AddProduct(pd);
                            SetDecomposeProduct(pd, element.IsDecomposedBy);
                        }
                        else
                        {
                            SetProductData((ProductData)prod, element);
                            sp.AddProduct(prod);
                        }
                    }
                }
            }
        }
        foreach (var item in cur.IsDecomposedBy.SelectMany(r => r.RelatedObjects))
        {
            sp.AddSpatial(GetSpatialSturctureData(cur, item));
        }
        return sp;
    }

    /// <summary>
    /// 实例化SpatialData
    /// </summary>
    /// <param name="s"></param>
    /// <returns></returns>
    private static ISpatialData InstaniateCurSpatial(IIfcSpatialStructureElement s)
    {
        var go = new GameObject();
        ISpatialData sp;
        
        if (s is IIfcSite)
        {
            sp = go.AddComponent<SiteData>();
        }
        else if(s is IIfcBuilding)
        {
            sp = go.AddComponent<BuildingData>();
        }
        else if(s is IIfcBuildingStorey)
        {
            sp = go.AddComponent<BuildingStoreyData>();
        }
        else if(s is IIfcSpace)
        {
            sp = go.AddComponent<SpaceData>();
        }
        else
        {
            sp = null;
        }
        if(sp != null)
        {
            sp.Name = s.Name;
            sp.TheStructure = s;
            go.name = sp.Name + "[" + sp.TheStructure.GetType().Name + "]" + sp.EntityLabel;
            spatialDatas.Add(sp);

        }
        return sp;
    }
    //初始化ProductData
    private static void SetProductData(ProductData pd,IIfcProduct p)
    {
        pd.ProductName = p.Name;
        pd.TypeName = p.GetType().Name;
        pd.TheGameObject.name = pd.ProductName + "[" + pd.TypeName + "]" + pd.ProductGeometryData.productLabel;
        pd.TheProduct = p;
        pd.HaveSpatialStructure = true;
    }

    private static List<IProductData> SetDecomposeProduct(IProductData productData, IEnumerable<IIfcRelAggregates> connects)
    {
        List<IProductData> pds = new List<IProductData>();
        foreach(var c in connects)
        {
            foreach(var prod in c.RelatedObjects)
            {
                var pd = productDatas.Find(p => p.ProductGeometryData.productLabel == prod.EntityLabel);
                SetProductData((ProductData)pd, (IIfcProduct)prod);
                pds.Add(pd);
            }
        }
        productData.DecomposedProducts = pds;
        return pds;
    }

    private static void AddGeoProductToSpatialStructure(List<ISpatialData> sds, List<IProductData> pds)
    {
        foreach(var sd in sds)
        {
            var pd = pds.Find(p => p.ProductGeometryData.productLabel == sd.EntityLabel);
            if(pd != null)
            {
                pds.Remove(pd);
                productDatas.Remove(pd);
                var spd = sd.TheGameObject.AddComponent<ProductData>();
                spd.ProductGeometryData = pd.ProductGeometryData;
                spd.TheProduct = pd.TheProduct;

                var children = pd.TheGameObject.GetComponentsInChildren<MeshRenderer>();
                //将一些本不该出现的空间结构隐藏
                if (sd.TheStructure is IIfcSpace)
                {
                    foreach(var c in children)
                    {
                        c.transform.parent = sd.TheGameObject.transform;
                        c.gameObject.SetActive(false);
                    }
                }
                else
                {
                    foreach(var c in children)
                    {
                        c.transform.parent = sd.TheGameObject.transform;
                    }
                }
                Destroy(pd.TheGameObject);
            }
        }
    }
}
